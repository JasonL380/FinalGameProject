using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class roomStats : MonoBehaviour
{
    public Vector2Int doorPos;
    public int facing;
    public Vector3Int pos;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Grid grid = GetComponentInParent<Grid>();
        pos = GetComponentInParent<Grid>().WorldToCell(transform.position - (Vector3Int)doorPos);
        Gizmos.DrawLine(grid.CellToWorld(grid.WorldToCell(transform.position) + (Vector3Int) doorPos),grid.CellToWorld(grid.WorldToCell(transform.position) + (Vector3Int) doorPos) + new Vector3(0, 0.1f, 0));
    }
}
