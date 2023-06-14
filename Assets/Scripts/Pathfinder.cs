/*
 * Liam Kikin-Gil
 * Jason Leech
 * 
 * 12/7
 * 
 * An algorithm to take in a map/area, and create points on it in order to traverse the area as easily as possible.
 */
using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using Utils;

[RequireComponent(typeof(CircleCollider2D), typeof(Rigidbody2D))]
[ExecuteInEditMode]
public class Pathfinder : MonoBehaviour
{
    public float speed; //the speed that this should move at, do not set this too high or it won't work

    [Tooltip("list of points for the AI follow")]
    public Vector2[] waypoints;
    private List<Vector3Int> pathfindingWaypoints = new List<Vector3Int>();

    [Tooltip("select all layers that contain objects which should be treated as walls")]
    public LayerMask wallLayers;

    //Current waypoint while chasing
    private int currentWaypoint = 0;
    private int currentPathWaypoint = 0;
    private Rigidbody2D myRB2D;
    private CircleCollider2D myCirc;
    public bool displayDebug = true;
    //private Vector2 currentTarget;
    [Tooltip("The center of the pathfinding area")]
    public Vector2 boxCenter;

    [Tooltip("The size of the pathfinding area")]
    public Vector2 boxSize;

    [Tooltip(
        "The density of nodes in the pathfinding area, higher values will allow for more accurate pathfinding but will take longer to process")]
    public float nodeDensity;

    //The spacing between nodes in the graph
    private Vector2 gridSize = new Vector2();

    //the bottom left (negative, negative) corner of the pathfinding area
    private Vector2 gridStart = new Vector2();

    //stores 5 boolean values in the order right, down, left, up, isPoint
    //public byte[,] graph;

    private Vector2Int graphDimensions = new Vector2Int();

    private float lastDensity;

    public float closeEnough;

    private RoomList _list;
    
    private Grid _grid;

    private GameObject player;

    private bool chasing = false;

    [SerializeField] private float followRange = 5;
    
    
    private void Start()
    {
        if (Application.isPlaying)
        {
            _list = FindObjectOfType<RoomList>();
            _grid = FindObjectOfType<Grid>();
            print("initializing pathfinder");
            myRB2D = GetComponent<Rigidbody2D>();
            myCirc = GetComponent<CircleCollider2D>();
            player = GameObject.FindGameObjectWithTag("Player");
            //target = waypoints[0];
            //generateGraph();
            //print(graph[graphDimensions.x/2,graphDimensions.y/2]);
            pathfindingWaypoints = a_star_search(actualToGrid(transform.position),
                actualToGrid(waypoints[currentPathWaypoint]));
            //print(gridToActual(graphDimensions));
            //print(actualToGrid(transform.position));
            //print(gridToActual(actualToGrid(transform.position)));
        }
    }

    /*void generateGraph()
    {
        //calculate the grid size and dimensions from nodeDensity
        gridSize = new Vector2(1 / nodeDensity, 1 / nodeDensity); //boxSize / nodeDensity;
        gridStart = boxCenter - (boxSize / 2);
        //print(gridSize + ", " + gridStart);
        int sizeX = (int) (nodeDensity * boxSize.x);
        int sizeY = (int) (nodeDensity * boxSize.y);
        graphDimensions.x = sizeX;
        graphDimensions.y = sizeY;
        //print(sizeX + "," + sizeY);

        //initialize the graph array
        graph = new byte[sizeX, sizeY];

        //fill entire walkable space with points
        for (int x = 0; x < sizeX; ++x)
        {
            for (int y = 0; y < sizeY; ++y)
            {
                //the absolute position of this grid point
                Vector2 position = (gridSize * new Vector2(x, y)) + gridStart;

                //create a circle overlap with the same size as this object's collider to detect any nearby walls, aka determine if the object is able to exist at this position
                Collider2D collision = Physics2D.OverlapCircle(position, myCirc.radius, wallLayers);

                //if the circle didn't collide with anything add a node here
                if (collision == null)
                {
                    graph[x, y] = 1;

                    //draw the point on screen if enabled
                    if (displayDebug)
                    {
                        Debug.DrawLine(position, position - new Vector2(0, 0.1F), Color.red, 12000);
                    }
                }
                else
                {
                    graph[x, y] = 0;
                }
            }
        }

        //loop through the graph to fill in data about neighbors
        for (int x = 0; x < sizeX; ++x)
        {
            for (int y = 0; y < sizeY; ++y)
            {
                //if there isn't a point here do nothing
                if (graph[x, y] == 1)
                {
                    //right
                    if (x + 1 < sizeX && (graph[x + 1, y] & 1) == 1)
                    {
                        graph[x, y] = Convert.ToByte(1 << 4 | graph[x, y]);
                    }

                    //down
                    if (y - 1 > -1 && (graph[x, y - 1] & 1) == 1)
                    {
                        graph[x, y] = Convert.ToByte(1 << 3 | graph[x, y]);
                    }

                    //left
                    if (x - 1 > -1 && (graph[x - 1, y] & 1) == 1)
                    {
                        graph[x, y] = Convert.ToByte(1 << 2 | graph[x, y]);
                    }

                    //up
                    if (y + 1 < sizeY && (graph[x, y + 1] & 1) == 1)
                    {
                        graph[x, y] = Convert.ToByte(1 << 1 | graph[x, y]);
                    }
                }
            }
        }
    }*/

    private void FixedUpdate()
    {
        if (Application.isPlaying)
        {
            if ((player.transform.position - transform.position).magnitude < followRange)
            {
                chasing = true;
            }
            else if(chasing)
            {
                chasing = false;
                pathfindingWaypoints = a_star_search(actualToGrid(transform.position), actualToGrid(waypoints[currentPathWaypoint]));
                currentWaypoint = 0;
            }

            if (chasing)
            {
                pathfindingWaypoints = a_star_search(actualToGrid(transform.position), actualToGrid(player.transform.position));
                currentWaypoint = 0;
            }
            
            pace();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < waypoints.Length - 1; ++i)
            { 
                Gizmos.DrawLine((Vector3) waypoints[i] + transform.parent.position, (Vector3) waypoints[i + 1] + transform.parent.position);
            }
        }
    }

    //calculate the shortest path between start and end, return array of waypoints
    List<Vector3Int> a_star_search(Vector3Int start, Vector3Int goal)
    {
        PriorityQueue<Vector3Int, float> queue = new PriorityQueue<Vector3Int, float>();
        queue.Enqueue(start, 0);

        Dictionary<Vector3Int, Vector3Int?> cameFrom = new Dictionary<Vector3Int, Vector3Int?>();

        Dictionary<Vector3Int, float> cost_so_far = new Dictionary<Vector3Int, float>();
        cameFrom[start] = null;
        cost_so_far[start] = 0;

        //print("running search " + queue.Count);
        while (queue.Count != 0)
        {
            Vector3Int current = queue.Dequeue();

            if (current == goal)
            {
                break;
            }

            //print("Visiting " + current);
            int currentPoint = _list.mapDataGet(current.x, current.y);//graph[current.x, current.y];
            for (int i = 1; i <= 8; ++i)
            {
                Vector3Int next = getNeighbor(current, i);
                if (_list.mapDataGet(next.x, next.y) == 1)
                {
                    float new_cost = cost_so_far[current] + 1;
                    if (!cost_so_far.ContainsKey(next) || new_cost < cost_so_far[next])
                    {
                        cost_so_far[next] = new_cost;

                        queue.Enqueue(next, new_cost + heuristic(next, goal));
                        cameFrom[next] = current;
                        //print(((gridSize * next) - gridStart).ToString() + ", " + (gridSize * current) + gridStart);
                        if (displayDebug)
                        {
                            Debug.DrawLine(gridToActual(next), gridToActual(current), Color.magenta, 30);
                        }
                    }
                }
            }
        }

        return reconstruct_path(cameFrom, start, goal);
    }

    float heuristic(Vector3Int pos1, Vector3Int pos2)
    {
        return (Math.Abs(pos1.x - pos2.x) + Math.Abs(pos1.y - pos2.y));
    }
    
    List<Vector3Int> reconstruct_path(Dictionary<Vector3Int, Vector3Int?> cameFrom, Vector3Int start, Vector3Int goal)
    {
        Vector3Int current = goal;
        List<Vector3Int> path = new List<Vector3Int>();
        if (!cameFrom.ContainsKey(goal))
        {
            print("No path found");
            return path;
        }

        while (current != start)
        {
            path.Add(current);
            if (cameFrom[current] != null)
            {
                Vector3Int lastpath = current;
                current = cameFrom[current].Value;
                if (displayDebug)
                {
                    Debug.DrawLine(gridToActual(lastpath), gridToActual(current), Color.blue, 12000);
                }
            }
        }

        path.Reverse(0, path.Count);
	//print(path.Count);
        return path;
    }

    Vector3Int getNeighbor(Vector3Int current, int direction)
    {
        switch (direction)
        {
            //up
            case 1:
                return new Vector3Int(current.x, current.y + 1);
            //left
            case 2:
                return new Vector3Int(current.x - 1, current.y);
            //down
            case 3:
                return new Vector3Int(current.x, current.y - 1);
            //right
            case 4:
                return new Vector3Int(current.x + 1, current.y);
            //up right
            case 5:
                return new Vector3Int(current.x + 1, current.y + 1);
            //down right
            case 6:
                return new Vector3Int(current.x + 1, current.y - 1);
            //down left
            case 7:
                return new Vector3Int(current.x - 1, current.y - 1);
            //up left
            case 8:
                return new Vector3Int(current.x - 1, current.y + 1);
        }

        return current;
    }

    Vector2 gridToActual(Vector3Int gridCoord)
    {
        return _grid.CellToWorld(gridCoord);
        //return (gridSize * gridCoord) + gridStart;
    }

    Vector3Int actualToGrid(Vector2 actual)
    {
        return _grid.WorldToCell(actual);
        /*Vector2 grid = (actual - (boxCenter - (boxSize / 2))) / gridSize;
        grid.x = Math.Min(Math.Max(grid.x, 0), graphDimensions.x - 1);
        grid.y = Math.Min(Math.Max(grid.y, 0), graphDimensions.y - 1);
        return new Vector2Int((int) grid.x, (int) grid.y);*/
    }

    bool hasNodes(byte[,] list)
    {
        for (int x = 0; x < graphDimensions.x; ++x)
        {
            for (int y = 0; y < graphDimensions.y; ++y)
            {
                if ((list[x, y] & 1) == 1)
                {
                    return true;
                }
            }
        }

        return false;
    }

    void pace()
    {
        Vector3 direction = gridToActual(pathfindingWaypoints[currentWaypoint]) - (Vector2) transform.position;
        Vector2 currentPos;
        currentPos.x = transform.position.x;
        currentPos.y = transform.position.y;
        //check if close to target, if so go to next one if possible
        if (direction.magnitude <= closeEnough)
        {
            ++currentWaypoint;
            if (currentWaypoint >= pathfindingWaypoints.Count - 1)
            {
                currentPathWaypoint++;
                if (currentPathWaypoint >= waypoints.Length)
                {
                    currentPathWaypoint = 0;
                }
                pathfindingWaypoints = a_star_search(actualToGrid(currentPos), actualToGrid(waypoints[currentPathWaypoint]));
                currentWaypoint = 0;
            }
            //currentTarget = pathfindingWaypoints[currentWaypoint];
            //a_star_search(actualToGrid(currentPos), actualToGrid(waypoints[currentWaypoint]));
        }
        
        myRB2D.velocity = direction.normalized * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name.Equals("Player"))
        {
            GetComponent<SceneController>().ChangeScene();
        }
    }
}