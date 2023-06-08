// Name: Jason Leech
// Date: 05/17/2023
// Desc:

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

namespace DefaultNamespace
{
    
    public struct gridBlock
    {
        public byte[,] data;
    }

    public class RoomList : MonoBehaviour
    {
        public GameObject[] rooms;

        public GameObject[] endRooms;

        public GameObject[] goalRooms;
        
        public List<GameObject> generatableRooms;
        
        public int roomCount;

        public int maxRooms;

        public bool generated = false;

        public byte[,] mapData;

        public Tilemap floor;
        
        public Tilemap walls;

        public Tilemap doors;
        

        [Tooltip("The max size of the map, this is one dimension of a square area")]
        public int maxSize;

        [Tooltip("The width of the border that goes around the edge of the map which only end rooms can generate in.")]
        public int borderSize = 128;
        
        private Grid _grid;

        [NonSerialized] public int arraySize;

        public Tile WallTile;
        [NonSerialized] public Matrix4x4 wallTransform = Matrix4x4.zero;

        public Sprite[] openDoors;

        public Sprite[] closedDoors;

        public int ungeneneratedDoors = 0;

        public int ungeneratedOneTimeRooms = 0;

        public int drawerRarity = 10;

        public GameObject[] leftObjects;
        public GameObject[] rightObjects;
        
        /*public void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            int max = (arraySize - borderSize) - arraySize / 2;

            int min = (arraySize / 2) * -1 + borderSize;
            
            Gizmos.DrawLine( _grid.CellToWorld(new Vector3Int(max, max)),  _grid.CellToWorld(new Vector3Int(max,min)));
            Gizmos.DrawLine( _grid.CellToWorld(new Vector3Int(min, min)),  _grid.CellToWorld(new Vector3Int(max,min)));
            Gizmos.DrawLine( _grid.CellToWorld(new Vector3Int(min, min)),  _grid.CellToWorld(new Vector3Int(min,max)));
            Gizmos.DrawLine( _grid.CellToWorld(new Vector3Int(max, max)),  _grid.CellToWorld(new Vector3Int(min,max)));
        }*/

        public void Awake()
        {
            generatableRooms = new List<GameObject>();
            
            foreach (GameObject g in rooms)
            {
                generatableRooms.Add(g);
            }

            foreach (GameObject g in endRooms)
            {
                generatableRooms.Add(g);
            }
            
            arraySize = maxSize + borderSize * 2;
            mapData = new Byte[arraySize, arraySize];

            //mapDataSet(120, 483, 8);

            // mapDataSet(2, 34, 3);

            //print(mapDataGet(2, 34));

            //print(mapDataGet(120, 483));
            //print(mapDataGet(120, 484));

            _grid = GetComponent<Grid>();


            Tilemap[] tilemaps = GetComponentsInChildren<Tilemap>();


            Grid grid = FindObjectOfType<Grid>();

            bool hasTile = false;
            for (int x = -256; x < 256; ++x)
            {
                for (int y = -256; y < 256; ++y)
                {
                    hasTile = false;
                    foreach (Tilemap t in tilemaps)
                    {
                        if (t.HasTile(new Vector3Int(x, y)))
                        {
                            mapDataSet(x, y, (byte) (t.name == "Floor" ? 1 : 2));
                        }
                    }
                }
            }
        }

        private bool addOneTime = false;
        public void FixedUpdate()
        {
            generated = false;
            
            if (roomCount >= 20 && !addOneTime)
            {
                addOneTime = true;
                
                    
                ungeneratedOneTimeRooms = goalRooms.Length;
                foreach (GameObject g in goalRooms)
                {
                    generatableRooms.Add(g);
                }
            }
        }

        public struct DrawerPos
        {
            public Vector3Int pos;
            //right = 0, left = 1
            public int side;
        }

        public List<DrawerPos> getEdges(Vector3Int min, Vector3Int max, Vector3Int offset)
        {
            List<DrawerPos> edges = new List<DrawerPos>();

            Vector3Int currentPos = Vector3Int.zero;

            
            for (currentPos.x = min.x + offset.x; currentPos.x <= max.x + offset.x; ++currentPos.x)
            {
                for (currentPos.y = min.y + offset.y; currentPos.y <= max.y + offset.y; ++currentPos.y)
                {
                    if (floor.HasTile(currentPos))
                    {
                        if (walls.HasTile(currentPos + new Vector3Int(0, 1)))
                        {
                            DrawerPos e = new DrawerPos();
                            e.pos = currentPos;
                            e.side = 0;
                            edges.Add(e);
                        }
                        else if(walls.HasTile(currentPos + new Vector3Int(1, 0)))
                        {
                            DrawerPos e = new DrawerPos();
                            e.pos = currentPos;
                            e.side = 1;
                            edges.Add(e);
                        }
                    }
                }
            }
            return edges;
        }
        
        public void tilemapCopy(Tilemap src, Tilemap dest, Vector3Int min, Vector3Int max, Vector3Int offset, byte type, bool overwriteWalls)
        {
            //print("copying " + src.name + " to " + dest.name + " bounds: " + min + ", " + max + " offset: " + offset);

            Vector3 worldMin = _grid.CellToWorld(min);

            Vector3 worldMax = _grid.CellToWorld(max);

            Vector3 worldOff = _grid.CellToWorld(offset);
            
            Debug.DrawLine(new Vector3(worldMax.x, worldMax.y) + worldOff, new Vector3(worldMin.x, worldMin.y) + worldOff, Color.red);
            
            TileChangeData data = new TileChangeData();
            Vector3Int currentPos = Vector3Int.zero;

            //offset = Vector3Int.zero;
            
            for (currentPos.x = min.x; currentPos.x <= max.x; ++currentPos.x)
            {
                for (currentPos.y = min.y; currentPos.y <= max.y; ++currentPos.y)
                {
                    
                    if (src.HasTile(currentPos))
                    {
                        TileBase t = src.GetTile(currentPos);
                        //print(t.GetType());
                        if (t.GetType() == typeof(IsometricRuleTile))
                        {
                            Tile t1 = new Tile();

                            t1.sprite = src.GetSprite(currentPos);

                            data.tile = t1;
                        }
                        else
                        {
                            data.tile = t;
                        }

                        if (type == 2)
                        {
                            data.tile = WallTile;
                        }
                        
                        data.position = currentPos + offset;
                        data.transform = src.GetTransformMatrix(currentPos);
                        dest.SetTile(data, false);

                        if (overwriteWalls)
                        {
                            data.tile = null;
                            walls.SetTile(data, false);
                        }
                        
                        mapDataSet(currentPos.x + offset.x, currentPos.y + offset.y, type);
                    }
                }
            }
            if (wallTransform == Matrix4x4.zero && type == 2)
            {
                wallTransform = data.transform;
            }
        }

        public bool inBorder(Vector3Int pos)
        {
            pos += new Vector3Int(arraySize / 2, arraySize / 2);
            if (pos.x > arraySize - borderSize)
            {
                //print("in border: positive x " + pos.x + " " + arraySize + " - " + borderSize + " = " + (arraySize - borderSize));
                return true;
            }
            
            if (pos.y > arraySize - borderSize)
            {
                //print("in border: positive y " + pos.y + " " + (arraySize - borderSize));
                return true;
            }
            
            if (pos.x < borderSize)
            {
                //print("in border: negative x " + pos.x + " " + (borderSize));
                return true;
            }
            
            if (pos.y < borderSize)
            {
                //print("in border: negative y " + pos.y + " " + (borderSize));
                return true;
            }

            return false;

            //return !(pos.x > ((arraySize / 2) * -1) + borderSize && pos.x < ((arraySize / 2)) - borderSize && pos.y > ((arraySize / 2) * -1) + borderSize && pos.y < ((arraySize / 2)) - borderSize);
        }
        
        public bool checkFit(Vector3Int min, Vector3Int max, Vector3Int offset)
        {
            for (int x = min.x; x < max.x; ++x)
            {
                for (int y = min.y; y < max.y; ++y)
                {
                    if (mapDataGet(x + offset.x, y + offset.y) != 0)
                    {
                        Debug.DrawLine(_grid.CellToWorld(new Vector3Int(x, y, 0)) + _grid.CellToWorld(offset), _grid.CellToWorld(new Vector3Int(x, y, 0)) + new Vector3(0, 0.1f, 0) + _grid.CellToWorld(offset), Color.red);
                        return false;
                    }
                    Debug.DrawLine(_grid.CellToWorld(new Vector3Int(x, y, 0)) + _grid.CellToWorld(offset), _grid.CellToWorld(new Vector3Int(x, y, 0)) + new Vector3(0, 0.1f, 0) + _grid.CellToWorld(offset), Color.green);
                }
            }

            return true;
        }
        
        public void mapDataSet(int x, int y, byte val)
        {
            /*int bx = (x / 256) + 128;
            int by = (y / 256) + 128;

            if (mapData[bx, by].data == null)
            {
                print("Allocating new block at " + bx + ", " + by);
                mapData[bx, by] = new gridBlock();
                mapData[bx, by].data = new byte[256, 256];
                
                for(int i = 0)
            }
            
            Debug.DrawLine(_grid.CellToWorld(new Vector3Int(x, y, 0)), _grid.CellToWorld(new Vector3Int(x, y, 0)) + new Vector3(0, 0.1f, 0), Color.magenta);
            
            //print( x+", "+y+"  " + ((x + 32768) % 256) + ", " + ((y + 32768)  % 256));*/
            mapData[x + (arraySize / 2), y + (arraySize / 2)] = val;
            //mapData[bx, by].data[(x + 32768) % 256, (y + 32768) % 256] = val;
        }

        /*public void OnApplicationQuit()
        {
            //clean up memory
            for (int x = 0; x < 128; ++x)
            {
                for (int y = 0; y < 128; ++y)
                {
                    
                }
            }
        }*/

        public byte mapDataGet(int x, int y)
        {
            /*int bx = (x / 256) + 128;
            int by = (y / 256) + 128;

            if (mapData[bx, by].data == null)
            {
                return 0;
            }

            return mapData[bx, by].data[(x + 32768) % 256, (y + 32768) % 256];*/
            return mapData[x + (arraySize / 2), y + (arraySize / 2)];
        }
    }
}