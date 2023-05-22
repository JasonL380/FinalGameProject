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

        public int roomCount;

        public int maxRooms;

        public bool generated = false;

        public gridBlock[,] mapData;

        public Tilemap floor;
        
        public Tilemap walls;

        public Dictionary<TileBase, TileBase> shortToTall;

        private Grid _grid;
        
        
        public void Start()
        {
            mapData = new gridBlock[256, 256];
            
            //mapDataSet(120, 483, 8);
            
           // mapDataSet(2, 34, 3);
            
            print(mapDataGet(2, 34));

            print(mapDataGet(120, 483));
            print(mapDataGet(120, 484));

            _grid = GetComponent<Grid>();
        }

        public void FixedUpdate()
        {
            generated = false;
        }

        public void tilemapCopy(Tilemap src, Tilemap dest, Vector3Int min, Vector3Int max, Vector3Int offset, byte type)
        {
            print("copying " + src.name + " to " + dest.name + " bounds: " + min + ", " + max + " offset: " + offset);

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
                    TileBase t = src.GetTile(currentPos);
                    if (t != null)
                    {
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
                        
                        data.position = currentPos + offset;
                        data.transform = src.GetTransformMatrix(currentPos);
                        dest.SetTile(data, false);
                        
                        mapDataSet(currentPos.x + offset.x, currentPos.y + offset.y, type);
                    }
                }
            }
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
            int bx = (x / 256) + 128;
            int by = (y / 256) + 128;

            if (mapData[bx, by].data == null)
            {
                print("Allocating new block at " + bx + ", " + by);
                mapData[bx, by] = new gridBlock();
                mapData[bx, by].data = new byte[256, 256];
            }
            
            Debug.DrawLine(_grid.CellToWorld(new Vector3Int(x, y, 0)), _grid.CellToWorld(new Vector3Int(x, y, 0)) + new Vector3(0, 0.1f, 0), Color.magenta);
            
            print( x+", "+y+"  " + ((x + 32768) % 256) + ", " + ((y + 32768)  % 256));
            
            mapData[bx, by].data[(x + 32768) % 256, (y + 32768) % 256] = val;
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
            int bx = (x / 256) + 128;
            int by = (y / 256) + 128;

            if (mapData[bx, by].data == null)
            {
                return 0;
            }

            return mapData[bx, by].data[(x + 32768) % 256, (y + 32768) % 256];
        }
    }
}