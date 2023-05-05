// Name: Jason Leech
// Date: 03/03/2023
// Desc:

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Serialization;

namespace Utils
{ 
    /*unsafe struct Node
    {
        public int x;
        public int y;
        public int neighborCount;
        public Node** neighbors;
        public int** costs;
    }*/

    public struct Path
    {
        public float pos;
        public JumpInfo jumpInfo;
        public Vector2 end;
        public Platform dest;
    }
    
    public struct Platform
    {
        public int id;
        public Vector2 start;
        public Vector2 end;
        public List<Path> paths;
    }

    public struct JumpInfo
    {
        public Vector2 start;
        public Vector2 end;
        public bool canJump;
        public Vector2 jumpSpeed;
        public Vector2 endSpeed;
    }

    public class PlatformingPathfinder : MonoBehaviour
    {
        private Dictionary<Collider2D, Platform> platforms;
        [Tooltip("The center of the pathfinding area")]
        public Vector2 boxCenter;

        [Tooltip("The size of the pathfinding area")]
        public Vector2 boxSize;

        public float colliderSize;

        [FormerlySerializedAs("wallLayers")] public LayerMask platformLayers;

        public float jumpHeight;

        public float moveSpeed;

        public float jumpSpeed;

        public Vector2 end;

        public List<Path> waypoints;

        public int currentWaypoint;

        public Rigidbody2D myRB2D;

        public bool jumping = false;
        
        private void Start()
        {
            platforms = new Dictionary<Collider2D, Platform>();
            jumpSpeed = Mathf.Sqrt(Mathf.Abs(2 * Physics2D.gravity.y * jumpHeight));
            //print(2 * Physics2D.gravity.y * jumpHeight);

            myRB2D = GetComponent<Rigidbody2D>();
            
            GenerateGraph();

            /*a_star_job job = new a_star_job();
            job.start = get_platform(transform.position);
            job.end = get_platform(end);
            JobHandle handle = job.Schedule();*/
            waypoints = a_star_search(get_platform(transform.position), get_platform(end));
        }

        private void Update()
        {
            pace();
        }

        void GenerateGraph()
        {
            int i = 0;
            Collider2D[] colliders = Physics2D.OverlapAreaAll(boxCenter - boxSize, boxCenter + boxSize, platformLayers);
            foreach (var collider in colliders)
            {
                if (collider is BoxCollider2D)
                {
                    Bounds bounds = ((BoxCollider2D)collider).bounds;
                    Platform platform = new Platform();
                    platform.start = (Vector2)bounds.center - new Vector2(bounds.extents.x, (bounds.extents.y * -1) - colliderSize);
                    platform.end = (Vector2)bounds.center - new Vector2((bounds.extents.x) * -1, (bounds.extents.y * -1) - colliderSize);
                    platform.paths = new List<Path>();
                    platform.id = i;
                    i++;
                    platforms.Add(collider, platform);
                    Debug.DrawLine(platform.start, platform.end, Color.red, 100);
                }
            }

            foreach (var platform in platforms)
            {
                //start
                Collider2D[] platform_collisions = Physics2D.OverlapCircleAll(platform.Value.start, 10, platformLayers);
                foreach (var other_platform in platform_collisions)
                {
                    if (other_platform != platform.Key)
                    {
                        JumpInfo info = CanJumpBetweenPoints(platform.Value.start, platforms[other_platform], true, jumpSpeed);
                        if (info.canJump)
                        {
                            
                            //print("info: " + info.start + " " + info.end + " " + info.canJump);
                            Path path = new Path();
                            path.dest = platforms[other_platform];
                            path.pos = 0;
                            path.jumpInfo = info;
                            path.end = platforms[other_platform].start;
                            platform.Value.paths.Add(path);
                            
                            JumpInfo info2 = CanJumpBetweenPoints(platform.Value.start, platforms[other_platform], true, Mathf.Sqrt(Mathf.Abs(2 * Physics2D.gravity.y *  (jumpHeight + (info.end.y - info.start.y))))); 
                            
                            Vector2 temp = info2.start;
                            info2.start = info2.end;
                            info2.end = temp;

                            info2.jumpSpeed = info2.endSpeed * -1;
                            
                            Path path1 = new Path();
                            path1.dest = platform.Value;
                            path1.pos = info2.end.x;
                            path1.jumpInfo = info2;
                            path1.end = platform.Value.start;
                        
                            platforms[other_platform].paths.Add(path1);

                                //Debug.DrawLine(platform.Value.end, raycast.point, Color.green, 100);
                        }
                    }
                }
                
                //end
                platform_collisions = Physics2D.OverlapCircleAll(platform.Value.end, jumpHeight, platformLayers);
                foreach (var other_platform in platform_collisions)
                {
                    if (other_platform != platform.Key)
                    {
                        JumpInfo info = CanJumpBetweenPoints(platform.Value.end, platforms[other_platform], false, jumpSpeed);
                        if (info.canJump)
                        {
                            //print("info: " + info.start + " " + info.end + " " + info.canJump);
                            Path path = new Path();
                            path.dest = platforms[other_platform];
                            path.pos = platform.Value.end.x - platform.Value.start.x;
                            path.jumpInfo = info;
                            path.end = platforms[other_platform].start;
                            platform.Value.paths.Add(path);
                            //float newJumpSpeed = 
                            JumpInfo info2 = CanJumpBetweenPoints(platform.Value.end, platforms[other_platform], false, Mathf.Sqrt(Mathf.Abs(2 * Physics2D.gravity.y * (jumpHeight + (info.end.y - info.start.y)))));

                            Vector2 temp = info2.start;
                            info2.start = info2.end;
                            info2.end = temp;

                            info2.jumpSpeed = info2.endSpeed * -1;
                            
                            Path path1 = new Path();
                            path1.dest = platform.Value;
                            path1.pos = info2.end.x;
                            path1.jumpInfo = info2;
                            path1.end = platform.Value.end;
                        
                            platforms[other_platform].paths.Add(path1);
                            //print("info2: " + info2.start + " " + info2.end + " " + info2.canJump);
                            
                            
                            //Debug.DrawLine(platform.Value.end, raycast.point, Color.green, 100);
                        }
                        
                    }
                }
            }
        }

        struct ParabolaCastResult
        {
            public RaycastHit2D hit;
            public Vector2 velocity;
        }
        
        JumpInfo CanJumpBetweenPoints(Vector2 start, Platform end, bool atStart, float maxJumpSpeed)
        {
            Debug.DrawLine(start, new Vector2(start.x, end.start.y), Color.yellow, 1000);
            JumpInfo info = new JumpInfo();
            info.canJump = false;
            info.start = start;
            
            if (end.start.y - start.y < jumpHeight)
            {
                float jump_time = ((maxJumpSpeed) / Mathf.Abs(Physics2D.gravity.y)) +
                                  (Mathf.Sqrt(Mathf.Abs(2 * Physics2D.gravity.y * ((start.y + jumpHeight) - end.start.y))) /
                                   Mathf.Abs(Physics2D.gravity.y));
               // print("jump time: " + jump_time);
                //print((Mathf.Sqrt(Mathf.Abs(2 * Physics2D.gravity.y * ((start.y + jumpHeight) - end.start.y))) / Physics2D.gravity.y));

                //float horizontal_distance = Mathf.Abs(end.x - start.x);

                float endX = (jump_time * moveSpeed * (atStart ? -1 : 1)) + start.x;
            
                //print("endX: " + endX);

                ParabolaCastResult result = ParabolaCast(new Vector2(moveSpeed * (start.x > endX ? -1 : 1), maxJumpSpeed), start);//Physics2D.Linecast(start, new Vector2(endX, end.start.y));
                info.jumpSpeed = new Vector2(moveSpeed * (start.x > endX ? -1 : 1), maxJumpSpeed);
                
                if (result.velocity.y > 0)
                {
                    float newJumpSpeed = Mathf.Sqrt(Mathf.Abs(2 * Physics2D.gravity.y * (result.hit.point.y - 1 - start.y)));
                    info.jumpSpeed.y = newJumpSpeed;
                    result = ParabolaCast(new Vector2(moveSpeed * (start.x > endX ? -1 : 1), newJumpSpeed), start);
                }
                
                if (result.hit.collider != null)
                {
                    Platform hitPlatform = platforms[result.hit.collider.gameObject.GetComponent<Collider2D>()];

                    if (hitPlatform.id == end.id)
                    {
                        info.end = new Vector2(result.hit.point.x, end.start.y);
            
                        if (endX >= end.start.x && endX <= end.end.x)
                        {
                            Debug.DrawLine(start, new Vector2(endX, end.start.y), Color.green, 1000);
                            info.canJump = true;
                        }
                    }
                    else
                    {
                        float newJumpSpeed = Mathf.Sqrt(Mathf.Abs(2 * Physics2D.gravity.y * (result.hit.point.y - result.hit.collider.bounds.size.y - 1 - start.y)));
                        info.jumpSpeed.y = newJumpSpeed;
                        result = ParabolaCast(new Vector2(moveSpeed * (start.x > endX ? -1 : 1), newJumpSpeed), start);
                        if (result.hit.collider != null)
                        {
                            hitPlatform = platforms[result.hit.collider.gameObject.GetComponent<Collider2D>()];

                            if (hitPlatform.id == end.id)
                            {
                                info.end = new Vector2(result.hit.point.x, end.start.y);
            
                                if (endX >= end.start.x && endX <= end.end.x)
                                {
                                    Debug.DrawLine(start, new Vector2(endX, end.start.y), Color.green, 1000);
                                    info.canJump = true;
                                }
                            }
                        }
                    }
                }

                info.endSpeed = result.velocity;
            }
            

            return info;
        }

        ParabolaCastResult ParabolaCast(Vector2 velocity, Vector2 start)
        {
            ParabolaCastResult result = new ParabolaCastResult();
            RaycastHit2D hit;
            Vector2 nextPos;

            int count = 0;
            
            do
            {
                nextPos = start + (velocity / 20);
                velocity += Physics2D.gravity / 20;
                hit = Physics2D.CircleCast(start, 0.5f, nextPos - start, (nextPos - start).magnitude, platformLayers);
                Debug.DrawLine(start, nextPos, Color.magenta, 30);
                start = nextPos;
                count++;
            } while (hit.collider == null && count < 100);

            result.hit = hit;
            result.velocity = velocity;
            
            return result;
        }

        public List<Path> a_star_search(Platform start, Platform goal)
        {
            PriorityQueue<Platform, float> queue = new PriorityQueue<Platform, float>();
            queue.Enqueue(start, 0);

            Dictionary<Platform, Platform?> cameFrom = new Dictionary<Platform, Platform?>();

            Dictionary<Platform, float> cost_so_far = new Dictionary<Platform, float>();
            cameFrom[start] = null;
            cost_so_far[start] = 0;

            //print("running search " + queue.Count);
            while (queue.Count != 0)
            {
                Platform current = queue.Dequeue();

                if (current.id == goal.id)
                {
                    break;
                }

                print("Visiting " + current.id);
                //Platform currentPoint = current.dest;
                foreach (var next in current.paths)
                {
                    if (!next.jumpInfo.canJump)
                    {
                        continue;
                    }
                    Platform next_platform = next.dest;
                    float new_cost = cost_so_far[current] + 1;
                    
                    Debug.DrawLine(next.jumpInfo.end, next.jumpInfo.start, Color.blue, 30);
                    if (!cost_so_far.ContainsKey(next_platform) || new_cost < cost_so_far[next_platform])
                    {
                        cost_so_far[next_platform] = new_cost;

                        queue.Enqueue(next_platform, new_cost + heuristic(next_platform, goal));
                        cameFrom[next_platform] = current;
                       // print(((gridSize * next) - gridStart).ToString() + ", " + (gridSize * current) + gridStart);
                    }
                }
            }

            return reconstruct_path(cameFrom, start, goal);
        }

        float heuristic(Platform pos1, Platform pos2)
        {
            return (Math.Abs(pos1.start.x - pos2.start.x) + Math.Abs(pos1.start.y - pos2.start.y));
        }
        
        List<Path> reconstruct_path(Dictionary<Platform, Platform?> cameFrom, Platform start, Platform goal)
        {
            Platform current = goal;
            List<Path> path = new List<Path>();
            if (!cameFrom.ContainsKey(goal))
            {
                print("No path found");
                return path;
            }
            int i = 0;
            while (current.id != start.id)
            {
                i++;
                if (i > platforms.Count + 10)
                {
                    break;
                }
                //print(i + " id: " + current.id + " camefrom: " + cameFrom[current].Value.id);
                Path p;
                int j = 0;
                foreach (var currentPath in cameFrom[current].Value.paths)
                {
                    //print("path to " + currentPath.dest.id + " camefrom id: " + cameFrom[current].Value.id);
                    j++;
                    if (j > platforms.Count + 10)
                    {
                        break;
                    }

                    if (currentPath.dest.id == current.id)
                    {
                        //print("found path to " + cameFrom[current].Value.id + " start: " + currentPath.jumpInfo.start + " end: " + currentPath.jumpInfo.end);
                        path.Add(currentPath);
                        /*p = currentPath;
                        path.Add(p.jumpInfo.start);
                        Debug.DrawLine(p.jumpInfo.start, p.jumpInfo.end, Color.blue, 100);
                        path.Add(p.jumpInfo.end);*/
                        
                        
//                        Debug.DrawLine(p.jumpInfo.end, path[(path.Count - 1 >= 0 ? path.Count - 3 : 0)], Color.blue, 100);
                        //Debug.DrawLine(path[0], path[path.Count - 1], Color.blue, 100);
                        if (cameFrom[current] != null)
                        {
                            Platform lastpath = current;
                            current = cameFrom[current].Value;
                        }
                        break;
                    }
                }
            }

            path.Reverse(0, path.Count);

            Path endPath = path[path.Count - 1];

            endPath.jumpInfo.canJump = false;
            endPath.jumpInfo.start = new Vector2(end.x, endPath.jumpInfo.end.y);
            
            path.Add(endPath);
            
	        print(path.Count);
            return path;
        }

        Platform get_platform(Vector2 pos)
        {
            RaycastHit2D ray = Physics2D.Raycast(pos, new Vector2(0, -1), 10, platformLayers);
            return platforms[ray.collider];
        }

        void pace()
        {
            if (waypoints.Count > currentWaypoint)
            {
                Vector3 direction = waypoints[currentWaypoint].jumpInfo.start - (Vector2)transform.position;
                Vector2 currentPos;
                currentPos.x = transform.position.x;
                currentPos.y = transform.position.y;
                //check if close to target, if so go to next one if possible
                

                if (direction.magnitude <= 1 && !waypoints[currentWaypoint].jumpInfo.canJump)
                {
                    ++currentWaypoint;
                }
                
                if (!jumping && direction.magnitude <= 0.1 && waypoints[currentWaypoint].jumpInfo.canJump)
                {
                    print(waypoints[currentWaypoint].jumpInfo.jumpSpeed);
                    myRB2D.velocity = waypoints[currentWaypoint].jumpInfo.jumpSpeed;//new Vector2(moveSpeed * (waypoints[currentWaypoint].x > waypoints[currentWaypoint + 1].x ? -1 : 1), jumpSpeed);
                    jumping = true;
                    ++currentWaypoint;
                    //a_star_search(actualToGrid(currentPos), actualToGrid(waypoints[currentWaypoint]));
                }

                if ((jumping && myRB2D.velocity.magnitude < 0.2 && Mathf.Abs(direction.y) < 0.1))
                {
                    jumping = false;
                }

                if (!jumping)
                { 
                    myRB2D.velocity = direction.normalized * moveSpeed;
                }
            }
            else
            {
                print("no path");
            }
        }
    }
}