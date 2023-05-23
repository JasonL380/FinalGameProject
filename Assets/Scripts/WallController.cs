// Name: Jason Leech
// Date: 05/09/2023
// Desc:

using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DefaultNamespace
{
    public class WallController : MonoBehaviour
    {
        public Tilemap wallMap;
        public Tilemap hiddenWallMap;
        public Grid grid;

        public LayerMask mask;

        private TileChangeData tallData;

        private TileChangeData shortData;

        public Matrix4x4 transformMatrix = new Matrix4x4(new Vector4(1.0f, 0.0f, 0.0f, 0.0f),
                                                         new Vector4(0.0f, 1.0f, 0.0f, 0.0f),
                                                         new Vector4(0.0f, 0.0f, 1.0f, 0.0f),
                                                         new Vector4(0.0f, -0.25f, 0.0f, 1.0f));

        private Vector3 lastPos = Vector2.zero;
        
        private void Start()
        {
            
            lastPos = transform.position;
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
            if ((transform.position - lastPos).sqrMagnitude > 2)
            {
                switchWalls(new Vector3(0.25f, -0.125f), true);
                switchWalls(new Vector3(-0.25f, -0.125f), true);
                switchWalls(new Vector3(0.25f, 0.125f), false);
                switchWalls(new Vector3(-0.25f, 0.125f), false);
                lastPos = transform.position;
            }
        }

        private void switchWalls(Vector3 direction, bool isShort)
        {
            RaycastHit2D hit = Physics2D.Linecast(transform.position, transform.position + (direction * 20f), mask);

            //foreach (RaycastHit2D h in hit)
           // {
                if (hit.collider != null)
                {
                    //Debug.DrawLine(transform.position, hit.point + (new Vector2(2, -1) * 0.125f), Color.red);
                    Debug.DrawLine(transform.position, hit.point, isShort ? Color.red : Color.blue);
                    Vector3Int gridPos = grid.WorldToCell(hit.point + (Vector2) direction);

                    wallMap = hit.collider.gameObject.GetComponent<Tilemap>();

                    //print(h.collider.gameObject.name);
                
                    for (int i = 2; i < 2; ++i)
                    {
                        Debug.DrawLine(hit.point + (Vector2)direction + (rotate(direction) * i * 2), hit.point + (Vector2)direction + (rotate(direction) * i * 2) + new Vector2(0,0.1f), Color.cyan);
                        Vector3Int currentGridPos = wallMap.WorldToCell(hit.point + (Vector2)direction + (rotate(direction) * i * 2));
                        
                       // print(currentGridPos);

                        //wallMap.getTile(currentGridPos);
                        
                        if (wallMap.HasTile(currentGridPos))
                        {
                            //print();
                            //print(transformMatrix);
                
                            //print("changing tile");
                            TileChangeData data = new TileChangeData();
                            data.transform = wallMap.GetTransformMatrix(currentGridPos);
                            data.position = currentGridPos;
                            data.tile = wallMap.GetTile(currentGridPos);

                            hiddenWallMap.SetTile(data, false);

                            data.tile = null;
                            
                            wallMap.SetTile(data, false);
                        }
                        else if (hiddenWallMap.HasTile(currentGridPos))
                        {
                            //print(wallMap.GetTile(currentGridPos).name);
                        }
                    }
                } 
        //  }
        
            Debug.DrawLine(transform.position, transform.position + (direction * 20f), isShort ? Color.green : Color.magenta);
        }

        private Vector2 rotate(Vector2 vec)
        {
            return new Vector2(vec.x * -1, vec.y);
        }
    }
}