using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DefaultNamespace;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;


[ExecuteInEditMode]
public class RoomGeneration : MonoBehaviour
{

    private int count;
    public LayerMask roomLayer;

    //Some magical values to transform the collider to isometric, do not change
    private Matrix4x4 transformMatrix = new Matrix4x4(new Vector4(1, 0, 0, 0), new Vector4(0, 1, 0, 0),
        new Vector4(0, 0, 1, 0), new Vector4(0, 0, 0, 1.41422f));

    public bool fits = false;

    public bool debug = false;

    private Grid grid;
    private RoomList _list;

    public SpriteRenderer _renderer;

    public bool exitOnly = false;

    private bool isBorderDoor;
    
    
    [Tooltip("the way into the next room through the door up right = 0, up left = 1, down left = 2, down right = 3"), Range(0, 3)]
    public int facing;

    public bool open;

    //has the room generated
    public bool roomGend;

    private void Start()
    {
        //roomLayer = LayerMask.GetMask("room");

        grid = FindObjectOfType<Grid>();
        _list = grid.gameObject.GetComponent<RoomList>();
        isBorderDoor = _list.inBorder(grid.WorldToCell(transform.position));
        //_renderer = GetComponentInChildren<SpriteRenderer>();
        _list.ungeneneratedDoors += 1;
        closeDoor();

        //print(isBorderDoor);
    }


    private void Update()
    {
        if (!roomGend && !_list.generated && Application.isPlaying)
        {
            if (generateRoom())
            {
                _list.generated = true;
                _list.roomCount += 1;
                _list.ungeneneratedDoors -= 1;
            }
            else
            {
              //  print("replacing self with wall at " + transform.position);
                _list.ungeneneratedDoors -= 1;
                Debug.DrawLine(transform.position, transform.position + new Vector3(0.1f, 0), Color.blue);
                TileChangeData data = new TileChangeData();
                data.tile = _list.WallTile;
                data.transform = _list.wallTransform;
                data.position = grid.WorldToCell(transform.position);
                _list.walls.SetTile(data, false);
                
                Destroy(gameObject);
            }


            roomGend = true;
        }
        if (Application.isEditor && !Application.isPlaying)
        {
            //_renderer = GetComponentInChildren<SpriteRenderer>();
            //print("update");
            if (open)
            {
                openDoor();
            }
            else
            {
                closeDoor();
            }
        }
        
    }
    
    /*private void Update()
    {
        if (Application.isEditor)
        {
            
        }
        
        if (Application.isPlaying && !roomGend)
        {
            if (!_list.generated)
            {

                
                Tile tile = new Tile();

                tile.sprite = _renderer.sprite;
                tile.colliderType = Tile.ColliderType.Grid;

                
                
                if (generateRoom())
                {
                    _list.generated = true;
                    _list.roomCount += 1;
                
                    _list.doors.SetTile(grid.WorldToCell(transform.position), tile);
                }
                else
                {
                    print("replacing self with wall at " + transform.position);
                    Debug.DrawLine(transform.position, transform.position + new Vector3(0.1f, 0), Color.blue);
                    TileChangeData data = new TileChangeData();
                    data.tile = _list.WallTile;
                    data.transform = _list.wallTransform;
                    data.position = grid.WorldToCell(transform.position);
                    _list.walls.SetTile(data, false);
                }
                

                roomGend = true;

                
                Destroy(gameObject);
            }
            
            /*else if (_list.roomCount >= _list.maxRooms)
            {
                roomGend = true;
            }*/

    //    }

    //fits = checkRoomFits(roomTypes[0], transform.position);
    //  }

    private void OnDrawGizmos()
    {

        /*if (this.enabled)
        {
            Vector2[] points = roomTypes[0].GetComponent<PolygonCollider2D>().points;

            Gizmos.color = fits ? Color.green : Color.red;
            Gizmos.DrawLine(points[0] + (Vector2) transform.position, points[1] + (Vector2) transform.position);
            Gizmos.DrawLine(points[1] + (Vector2) transform.position, points[2] + (Vector2) transform.position);
            Gizmos.DrawLine(points[2] + (Vector2) transform.position, points[3] + (Vector2) transform.position);
            Gizmos.DrawLine(points[3] + (Vector2) transform.position, points[0] + (Vector2) transform.position);
        }*/
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //TODO: add open animation here
        
        openDoor();
        //if no room has generated and the player collides
        /*if (!roomGend && collision.name.Equals("Player"))
        {
            //Tile tile = new Tile();

           // tile.sprite = _renderer.sprite;
           // tile.colliderType = Tile.ColliderType.Grid;



            if (generateRoom())
            {
                _list.generated = true;
                _list.roomCount += 1;
                
                
            }
            else
            {
                print("replacing self with wall at " + transform.position);
                Debug.DrawLine(transform.position, transform.position + new Vector3(0.1f, 0), Color.blue);
                TileChangeData data = new TileChangeData();
                data.tile = _list.WallTile;
                data.transform = _list.wallTransform;
                data.position = grid.WorldToCell(transform.position);
                _list.walls.SetTile(data, false);
                
                Destroy(gameObject);
            }
            
            

            roomGend = true;
        }*/
    }

    private void openDoor()
    {
        open = true;
        _renderer.sprite = _list.openDoors[facing];
        _renderer.sortingOrder = 3;
    }

    private void closeDoor()
    {
        open = false;
        _renderer.sprite = _list.closedDoors[facing];
        if (facing < 2)
        {
            _renderer.sortingOrder = 3;
        }
        else
        {
            _renderer.sortingOrder = 1;
        }
    }
    
    private bool generateRoom()
    {
        
        //print("[RoomGeneration.cs] Attempting to generate room");

        
        //get a random room


        Shuffle(_list.generatableRooms);

        List<GameObject> list = _list.generatableRooms;

        //randomly picks from the list until it's exhausted or it finds a matching one
        int i;
        

        bool needOneTime = _list.ungeneneratedDoors <= _list.ungeneratedOneTimeRooms && _list.ungeneratedOneTimeRooms > 0;
        for (i = 0; i < list.Count; ++i)
        {
            roomStats stats = list[i].GetComponent<roomStats>();
            if ((!stats.endRoom && !isBorderDoor) || (stats.endRoom && isBorderDoor))
            {
                RoomGeneration[] doors = list[i].GetComponentsInChildren<RoomGeneration>();
                foreach (RoomGeneration d in doors)
                {
                    if (invertFacing(d.facing) != facing)
                    {
                        if (debug)
                        {
                            // print("[RoomGeneration.cs] Failed to place " + _list.rooms[i].name + ": wrong facing dir");
                        }

                        continue;
                    }

                    Vector3Int doorPos = grid.WorldToCell(transform.position);
                    //some math to properly align the door

                    Vector3Int doorOffset =
                        (grid.WorldToCell(d.transform.position + list[i].transform.position + getDoorOffset(d.facing)) -
                         grid.WorldToCell(list[i].transform.position));

                    doorPos.x -= (int)(list[i].transform.position.x) + doorOffset.x;
                    doorPos.y -= (int)(list[i].transform.position.y) + doorOffset.y;
                    doorPos.z -= (int)(list[i].transform.position.z);
                    Vector3 finalPos = grid.CellToWorld(doorPos);

                    //check if the room fits or override the check if a one time room is necessary
                    if ((_list.checkFit(stats.min, stats.max, doorPos) && !needOneTime) || (stats.oneTime && needOneTime))
                    {
                        if (stats.oneTime)
                        {
                            print("Generating one time room " + list[i].name + " at " + finalPos + " needed: " + needOneTime);
                        }
                        //print("[RoomGeneration.cs] Generating " + _list.rooms[i].name + " at " + grid.WorldToCell(transform.position));

                        Tilemap[] tilemaps = list[i].transform.GetComponentsInChildren<Tilemap>();

                        ;

                        for (int j = 0; j < list[i].transform.childCount; ++j)
                        {
                            GameObject child = list[i].transform.GetChild(j).gameObject;

                            Tilemap t = child.GetComponent<Tilemap>();

                            RoomGeneration r = child.GetComponent<RoomGeneration>();

                            Pathfinder p = child.GetComponent<Pathfinder>();
                            
                            if (t != null)
                            {
                                if (child.name.Equals("Wall"))
                                {
                                    _list.tilemapCopy(t, _list.walls, stats.min, stats.max, doorPos, 2, false);
                                }
                                else if(child.name.Equals("Floor"))
                                {
                                    _list.tilemapCopy(t, _list.floor, stats.min, stats.max, doorPos, 1, needOneTime);
                                }
                            }
                            else if(r != null)
                            {
                                if (r.facing != d.facing)
                                {
                                    Instantiate(r.gameObject, r.transform.position + finalPos, Quaternion.identity).transform.parent = grid.gameObject.transform;
                                }
                            }
                            else if(p != null)
                            {
                                GameObject pathfinder = Instantiate(child, child.transform.position + finalPos, Quaternion.identity);
                                pathfinder.transform.parent = grid.gameObject.transform;

                                Pathfinder p1 = pathfinder.GetComponent<Pathfinder>();

                                for (int k = 0; k < p1.waypoints.Length; ++k)
                                {
                                    p1.waypoints[k] += (Vector2) finalPos;
                                }
                            }
                            else
                            {
                                Instantiate(child, child.transform.position + finalPos, Quaternion.identity).transform.parent = grid.gameObject.transform;
                            }
                        }

                        //generate drawers
                        //get a list of places a drawer can generate
                        List<RoomList.DrawerPos> drawerPosList = _list.getEdges(stats.min, stats.max, doorPos);

                        foreach (RoomList.DrawerPos drawerPos in drawerPosList)
                        {
                            int rand = Random.Range(0, _list.drawerRarity);
                            if (rand == 0)
                            {
                                GameObject[] objects = drawerPos.side == 0 ? _list.rightObjects : _list.leftObjects;

                                rand = Random.Range(0, objects.Length);

                                Instantiate(objects[rand], grid.CellToWorld(drawerPos.pos) + new Vector3(0, 0.4f), Quaternion.identity,
                                    grid.gameObject.transform);
                            }
                        }

                        //if room is one time remove it
                        if (stats.oneTime)
                        {
                            _list.ungeneratedOneTimeRooms -= 1;
                            _list.generatableRooms.Remove(list[i]);
                        }
                        
                        //make the room
                        /*GameObject roomClone = Instantiate(_list.rooms[i]);
                        roomClone.transform.parent = grid.transform;
                        roomClone.SetActive(true);
                    
                        Debug.DrawLine(transform.position, d.transform.position + finalPos, Color.magenta);
                        //put the room in it's proper place
                        roomClone.transform.position = finalPos;
                        
                        RoomGeneration[] doors1 = roomClone.GetComponentsInChildren<RoomGeneration>();

                        foreach (RoomGeneration d1 in doors)
                        {
                            if (d1.facing == d.facing)
                            {
                                d1.enabled = false;
                                break;
                            }
                        }*/

                        //don't let any other rooms generate
                        roomGend = true; 
                        return true;
                    }
                    else
                    {
                        if (debug)
                        {
                            //print("[RoomGeneration.cs] Failed to place " + _list.rooms[i].name + ": room does not fit");
                        }

                        return false;
                    }
                }
            }
        }
        return false;
    }
    
    void Shuffle (List<GameObject> deck) {
        for (int i = 0; i < deck.Count; i++) {
            GameObject temp = deck[i];
            int randomIndex = UnityEngine.Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    
    bool checkRoomFits(GameObject room, Vector3 position)
    {
        Vector3 pos = position;
        //pos.x = (float)Math.Round(position.x);
        //pos.y = (float)Math.Round(position.y);

        //get the position of the door for centering it
        Vector3Int doorPos = grid.WorldToCell(pos);
        //some math to properly align the door
        doorPos.x -= (int)(room.transform.position.x) + (int)room.GetComponent<roomStats>().doorPos.x;
        doorPos.y -= (int)(room.transform.position.y) + (int)room.GetComponent<roomStats>().doorPos.y;
        doorPos.z -= (int)(room.transform.position.z);
        Vector2 finalPos = pos;//grid.CellToWorld(doorPos);
        
        PolygonCollider2D collider = room.GetComponent<PolygonCollider2D>();

        ContactFilter2D filter2D = new ContactFilter2D();

        RaycastHit2D[] hit = new []{new RaycastHit2D(), new RaycastHit2D(), new RaycastHit2D(), new RaycastHit2D(), new RaycastHit2D(), new RaycastHit2D(), new RaycastHit2D(), new RaycastHit2D()};

        Collider2D point = Physics2D.OverlapPoint(finalPos, roomLayer);

        if (point != null && point.transform != transform.parent)
        {
            
            Debug.DrawLine(finalPos, finalPos + new Vector2(0, 0.1f), Color.magenta);
            if (debug)
            {
                print("[RoomGeneration.cs] Failed to place " + room.name + ": room is inside " + point.name);
            }

            for (int i = 0; i < collider.points.Length; ++i)
            {
                //print(i);
                //Physics2D.Lin
                if (i + 1 == collider.points.Length)
                {
                    Debug.DrawLine(collider.points[i] + finalPos, collider.points[0] + finalPos, Color.magenta);
                }
                else
                {
                    Debug.DrawLine(collider.points[i] + finalPos, collider.points[i + 1] + finalPos, Color.magenta);
                }
            }

            return false;
        }
        
        for (int i = 0; i < collider.points.Length; ++i)
        {
            //print(i);
            //Physics2D.Lin
            if (i + 1 == collider.points.Length)
            {
                Physics2D.LinecastNonAlloc(collider.points[i] + finalPos, collider.points[0] + finalPos, hit, roomLayer);
            }
            else
            {
                Physics2D.LinecastNonAlloc(collider.points[i] + finalPos, collider.points[i + 1] + finalPos, hit,roomLayer);
            }

            foreach (RaycastHit2D h in hit)
            {
                if (h.collider != null)
                {
                    
                    if (debug)
                    {
                        print("[RoomGeneration.cs] Failed to place " + room.name + ": room collides with " + h.collider.name + " " + this.transform.parent.name);
                    }
                    
                    if (h.collider.gameObject.transform.position == this.transform.parent.position)
                    {
                        print("not self colliding...");
                        Debug.DrawLine(collider.points[i] + finalPos, i + 1 == collider.points.Length ? collider.points[0] + finalPos : collider.points[i + 1] + finalPos, Color.blue);
                    }
                    else
                    {
                        Debug.DrawLine(collider.points[i] + finalPos, i + 1 == collider.points.Length ? collider.points[0] + finalPos : collider.points[i + 1] + finalPos, Color.red);
                        return false;
                    }
                    
                }
                else
                {
                    Debug.DrawLine(collider.points[i] + finalPos, i + 1 == collider.points.Length ? collider.points[0] + finalPos : collider.points[i + 1] + finalPos, Color.green);
                }
            }
            
            
        }

        return true;
        
        Quaternion rot = Quaternion.Euler(0, 0, 45);

        ContactFilter2D filter = new ContactFilter2D();
        
        filter.layerMask = LayerMask.NameToLayer("room");

        List<RaycastHit2D> collider2Ds = new List<RaycastHit2D>();

        collider.offset += (Vector2) finalPos;

        collider.Cast(new Vector2(1,1), filter, collider2Ds, distance: 0.1f);

        collider.offset -= (Vector2) finalPos;
        return collider2Ds.Count == 0;
    }
    
    Vector3 divideY(Vector3 vec)
    {
        return new Vector3(vec.x, vec.y * 0.5f, vec.z);
    }

    int invertFacing(int f)
    {
        switch (f)
        {
            case 0:
                return 2;
            case 1:
                return 3;
            case 2:
                return 0;
            case 3:
                return 1;
            default:
                return 0;
        }
    }

    Vector3 getDoorOffset(int facing)
    {
        switch (facing)
        {
            case 0:
                return new Vector3(0.5f, 0, 0);
            case 1:
                return new Vector3(-0.5f, 0, 0);
            case 2:
                return new Vector3(-0.5f, -0.5f, 0);
            case 3:
                return new Vector3(0.5f, -0.5f, 0);
        }

        return Vector3.zero;
    }
}
