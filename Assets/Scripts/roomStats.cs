using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class roomStats : MonoBehaviour
{
    public Vector2Int doorPos;
    [Tooltip("the way into the next room through the door up right = 0, up left = 1, down left = 2, down right = 3")]
    public int facing;
    public Vector3 pos;
    public Vector3Int min;
    public Vector3Int max;

    public bool endRoom = false;
    public bool oneTime = false;
    
    [Tooltip("Trigger this to determine the bounds of this room automatically")]
    public bool generate;

    
    private Grid _grid;

    private void Start()
    {
        _grid = FindObjectOfType<Grid>();
    }

    private void Update()
    {
        if (generate)
        {
            Tilemap[] tilemaps = GetComponentsInChildren<Tilemap>();
            
            
            Grid grid = FindObjectOfType<Grid>();
            min = Vector3Int.zero;
            max = Vector3Int.zero;
            
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
                            hasTile = true;
                            break;
                        }
                    }

                    if (hasTile)
                    {
                        Vector3Int gridPos = new Vector3Int(x, y);

                        if (gridPos.x > max.x)
                        {
                            max.x = gridPos.x;
                        }
                        else if(gridPos.x < min.x)
                        {
                            min.x = gridPos.x;
                        }
                
                        if (gridPos.y > max.y)
                        {
                            max.y = gridPos.y;
                        }
                        else if(gridPos.y < min.y)
                        {
                            min.y = gridPos.y;
                        }
                    }
                }
            }
            
            print("generated thing");
            
            generate = false;
        }
        Vector3 worldMin = _grid.CellToWorld(min);

        Vector3 worldMax = _grid.CellToWorld(max);
            
        Debug.DrawLine(new Vector3(worldMax.x, worldMax.y) + transform.position, new Vector3(worldMin.x, worldMin.y) + transform.position, Color.red);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Grid grid = FindObjectOfType<Grid>();
            /*min = Vector3Int.zero;
            max = Vector3Int.zero;
            
            foreach (Vector2 p in GetComponent<PolygonCollider2D>().points)
            {
                Vector3Int gridPos = grid.WorldToCell(p);

                if (gridPos.x > max.x)
                {
                    max.x = gridPos.x;
                }
                else if(gridPos.x < min.x)
                {
                    min.x = gridPos.x;
                }
                
                if (gridPos.y > max.y)
                {
                    max.y = gridPos.y;
                }
                else if(gridPos.y < min.y)
                {
                    min.y = gridPos.y;
                }
            }
            
            //max += new Vector3Int(1, 1);
            //min -= new Vector3Int(1, 1);
            
            Vector3 worldMin = grid.CellToWorld(min);

            Vector3 worldMax = grid.CellToWorld(max);
            
            Debug.DrawLine(new Vector3(worldMax.x, worldMax.y) + transform.position, new Vector3(worldMin.x, worldMin.y) + transform.position, Color.red);*/
            
            pos = grid.CellToWorld(grid.WorldToCell(transform.position) + (Vector3Int)doorPos);

            switch (facing)
            {
                case 0:
                    pos += new Vector3(0.5f, 0.5f, 0);
                    break;
                case 1:
                    pos += new Vector3(-0.5f, 0.5f, 0);
                    break;
                case 2:
                    pos += new Vector3(-0.5f, 0.0f, 0);
                    break;
                case 3:
                    pos += new Vector3(0.5f, -0.5f, 0);
                    break;
                
            }

            Gizmos.DrawLine(pos, pos + new Vector3(0, 0.1f, 0));
        }
    }
}
