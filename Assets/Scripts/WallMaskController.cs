using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DefaultNamespace
{
    [RequireComponent(typeof(Rigidbody2D), typeof(PolygonCollider2D))]
    public class WallMaskController : MonoBehaviour
    {
        public GameObject mask;

        public Tilemap wallMap;

        [Tooltip("A tilemap without a renderer to store the fact that hidden walls exist")]
        public Tilemap hiddenWallMap;
        
        public List<Vector3Int> hiddenWallList;

        private Vector3 curPos;

        private Collider2D _collider2D;

        private Grid _grid;

        private RoomList _list;

        private Tile hiddenTile;

        private Tile normalTile;

        private void Start()
        {
            _collider2D = GetComponent<Collider2D>();
            _grid = FindObjectOfType<Grid>();

            _list = _grid.GetComponent<RoomList>();
            
            hiddenWallList = new List<Vector3Int>();

            normalTile = _list.WallTile;
            
            hiddenTile = new Tile();
            
            hiddenTile.sprite = null;
            hiddenTile.colliderType = Tile.ColliderType.Grid;
        }

        private void Update()
        {
            RaycastHit2D left = Physics2D.Raycast(transform.position, new Vector2(-1, 0.5f), Mathf.Infinity, LayerMask.GetMask("Wall", "door"));
            RaycastHit2D right = Physics2D.Raycast(transform.position, new Vector2(1, 0.5f), Mathf.Infinity, LayerMask.GetMask("Wall", "door"));
            //print("left: " + left.point + " right: " + right.point);

            Vector2 leftPoint = left.point;
            Vector2 rightPoint = right.point;

            
            if (((Vector3) leftPoint - transform.position).magnitude > 7)
            {
                leftPoint = new Vector3(-7, 4.5f) + transform.position;
            }
            
            if (((Vector3) rightPoint - transform.position).magnitude > 7)
            {
                rightPoint = new Vector3(7, 4.5f) + transform.position;
            }

            Debug.DrawLine(leftPoint, transform.position, Color.red);
            Debug.DrawLine(rightPoint, transform.position, Color.red);

            Vector2 intersect = findIntersect(new Vector3(leftPoint.x, leftPoint.y, 0.5f),
                new Vector3(rightPoint.x, rightPoint.y, -0.5f));
            Debug.DrawLine(intersect, transform.position, Color.green);

            //intersect.x = ((int) intersect.x * (int) 64) / 64f;
            //intersect.y = ((int) intersect.y * (int) 64) / 64f;
            Collider2D[] contacts =  new Collider2D[8];
            if (_collider2D.GetContacts(contacts) == 0)
            {
                mask.transform.position = intersect;
                
            }


            if ((mask.transform.position - curPos).sqrMagnitude > 2)
            {
                curPos = intersect;
                foreach (Vector3Int wall in hiddenWallList.ToArray())
                {
                    if (wall != new Vector3Int(Int32.MinValue, Int32.MinValue))
                    {
                        
                        //print("updating edge walls");
                        TileChangeData data = new TileChangeData();
                        data.transform = wallMap.GetTransformMatrix(wall);
                        data.position = wall;
                        data.tile = _list.WallTile;
                        
                        wallMap.SetTile(data, false);

                        hiddenWallList.Remove(wall);
                    }
                }

                RaycastHit2D leftDown = Physics2D.Raycast(leftPoint - new Vector2(-0.25f, 0.125f), new Vector2(-1, -0.5f), Mathf.Infinity,
                LayerMask.GetMask("Wall", "door"));
            
                RaycastHit2D rightDown = Physics2D.Raycast(rightPoint  - new Vector2(0.25f, 0.125f), new Vector2(1, -0.5f), Mathf.Infinity,
                LayerMask.GetMask("Wall", "door"));
                
                //left down walls
                for (int i = 0; i < 3; ++i)
                {
                    Vector2 point = leftDown.point - new Vector2(0.25f, 0.125f) + (new Vector2(0.5f, -0.25f) * i);
                    Debug.DrawLine(point, point + new Vector2(0, 0.1f), Color.blue, 100);

                    //print("right down " + i);
                
                    Vector3Int gridPoint = _grid.WorldToCell(point);
                
                    if (wallMap.HasTile(gridPoint))
                    {
                        TileChangeData data = new TileChangeData();
                        data.transform = wallMap.GetTransformMatrix(gridPoint);
                        data.position = gridPoint;
                        data.tile = hiddenTile;

                        wallMap.SetTile(data, false);

                        hiddenWallList.Add(gridPoint);
                    }
                    
                    gridPoint += new Vector3Int(-1, 0);
                    
                    if (wallMap.HasTile(gridPoint))
                    {
                        TileChangeData data = new TileChangeData();
                        data.transform = wallMap.GetTransformMatrix(gridPoint);
                        data.position = gridPoint;
                        data.tile = hiddenTile;

                        wallMap.SetTile(data, false);

                        hiddenWallList.Add(gridPoint);
                    }
                }
                
                
                //right down walls
                for (int i = 0; i < 3; ++i)
                {
                    Vector2 point = rightDown.point - new Vector2(-0.25f, 0.125f) + (new Vector2(-0.5f, -0.25f) * i);
                    Debug.DrawLine(point, point + new Vector2(0, 0.1f), Color.blue, 100);

                    //print("right down " + i);
                
                    Vector3Int gridPoint = _grid.WorldToCell(point);
                
                    if (wallMap.HasTile(gridPoint))
                    {
                        TileChangeData data = new TileChangeData();
                        data.transform = wallMap.GetTransformMatrix(gridPoint);
                        data.position = gridPoint;
                        data.tile = hiddenTile;

                        wallMap.SetTile(data, false);

                        hiddenWallList.Add(gridPoint);
                    }

                    gridPoint += new Vector3Int(0, -1);
                    
                    if (wallMap.HasTile(gridPoint))
                    {
                        TileChangeData data = new TileChangeData();
                        data.transform = wallMap.GetTransformMatrix(gridPoint);
                        data.position = gridPoint;
                        data.tile = hiddenTile;

                        wallMap.SetTile(data, false);

                        hiddenWallList.Add(gridPoint);
                    }
                }
            }
        }

        Vector2 findIntersect(Vector3 vec1, Vector3 vec2)
        {
            float i1 = vec1.y - (vec1.x * vec1.z);
            float i2 = vec2.y - (vec2.x * vec2.z);
            
            Vector2 intersect = Vector2.zero;

            intersect.x = (i2 - i1) / (vec1.z - vec2.z);
            intersect.y = vec1.z * ((i2 - i1) / (vec1.z - vec2.z)) + i1;

            return intersect;
        }
    }
}