using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class roomStats : MonoBehaviour
{
    public Vector2Int doorPos;
    [Tooltip("the way into the next room through the door up right = 0, up left = 1, down left = 2, down right = 3")]
    public int facing;
    public Vector3 pos;

    private void OnDrawGizmos()
    {
        if (Application.isEditor)
        {
            Gizmos.color = Color.red;
            Grid grid = FindObjectOfType<Grid>();
            pos = grid.CellToWorld(grid.WorldToCell(transform.position) + (Vector3Int)doorPos);

            switch (facing)
            {
                case 0:
                    pos += new Vector3(0.5f, 0.5f, 0);
                    break;
                case 1:
                    pos += new Vector3(-0.5f, 0.5f, 0);
                    break;
            }

            Gizmos.DrawLine(pos, pos + new Vector3(0, 0.1f, 0));
        }
    }
}
