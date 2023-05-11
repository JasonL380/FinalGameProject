// Name: Jason Leech
// Date: 05/09/2023
// Desc:

using System;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DefaultNamespace
{
    public class WallController : MonoBehaviour
    {
        public Tilemap wallMap;
        public TileBase tallTile;
        public TileBase shortTile;
        public Grid grid;

        public LayerMask mask;

        private TileChangeData tallData;

        private TileChangeData shortData;

        public Matrix4x4 transformMatrix = new Matrix4x4(new Vector4(1.0f, 0.0f, 0.0f, 0.0f),
                                                         new Vector4(0.0f, 1.0f, 0.0f, 0.0f),
                                                         new Vector4(0.0f, 0.0f, 1.0f, 0.0f),
                                                         new Vector4(0.0f, -0.25f, 0.0f, 1.0f));
        
        private void Start()
        {
            /*tallData = new TileChangeData();
            tallData.transform = transformMatrix;
            tallData.tile = tallTile;
            
            shortData = new TileChangeData();
            shortData.colliderType = Tile.ColliderType.Grid;
            shortData.sprite = shortTile;
            shortData.transform = transformMatrix;*/

            //TileChangeData data = new TileChangeData();
            
            //data.
        }

        private void Update()
        {
            switchWalls(new Vector3(0.25f, -0.125f), true);
            switchWalls(new Vector3(-0.25f, -0.125f), true);
            switchWalls(new Vector3(0.25f, 0.125f), false);
            switchWalls(new Vector3(-0.25f, 0.125f), false);
        }

        private void switchWalls(Vector3 direction, bool isShort)
        {
            RaycastHit2D hit = Physics2D.Linecast(transform.position, transform.position + (direction * 20f), mask);

            Debug.DrawLine(transform.position, transform.position + (direction * 20f), isShort ? Color.green : Color.magenta);
            
            if (hit.collider != null)
            {
                //Debug.DrawLine(transform.position, hit.point + (new Vector2(2, -1) * 0.125f), Color.red);
                Debug.DrawLine(transform.position, hit.point, isShort ? Color.red : Color.blue);
                Vector3Int gridPos = grid.WorldToCell(hit.point + (Vector2) direction);
                

                for (int i = -15; i < 15; ++i)
                {
                    Debug.DrawLine(hit.point + (Vector2)direction + (rotate(direction) * i * 2), hit.point + (Vector2)direction + (rotate(direction) * i * 2) + new Vector2(0,0.1f), Color.cyan);
                    Vector3Int currentGridPos = grid.WorldToCell(hit.point + (Vector2)direction + (rotate(direction) * i * 2));
                    
                    
                    
                    if (wallMap.GetTile(currentGridPos) == (isShort ? tallTile : shortTile))
                    {
                        //print();
                        //print(transformMatrix);
                
                        print("changing tile");
                        TileChangeData data = new TileChangeData();
                        data.transform = wallMap.GetTransformMatrix(currentGridPos);
                        data.tile = isShort ? shortTile : tallTile;
                        data.position = currentGridPos;
                
                        wallMap.SetTile(data, false);
                    }
                }

                    

                
            }
        }

        private Vector2 rotate(Vector2 vec)
        {
            return new Vector2(vec.x * -1, vec.y);
        }
    }
}