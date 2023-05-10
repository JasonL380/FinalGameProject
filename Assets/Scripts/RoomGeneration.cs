using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RoomGeneration : MonoBehaviour
{

    public GameObject[] roomTypes;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int rand = UnityEngine.Random.Range(0, roomTypes.Length);
        if (true/*check room size to make sure it doesn't overlap*/)
        {
            GameObject roomClone = Instantiate(roomTypes[rand]);
            roomClone.transform.parent = GameObject.Find("Grid").transform;
            roomClone.SetActive(true);
            Vector3Int doorPos = GetComponentInParent<Grid>().WorldToCell(collision.transform.position);
            doorPos.x -= (int)(roomClone.transform.position.x + 1) + (int)roomClone.GetComponent<roomStats>().doorPos.x;
            doorPos.y -= (int)(roomClone.transform.position.y + 1) + (int)roomClone.GetComponent<roomStats>().doorPos.y;
            doorPos.z -= (int)(roomClone.transform.position.z);
            Vector3 finalPos = GetComponentInParent<Grid>().CellToWorld(doorPos);
            /*
            Vector3 pos;
            pos.x = collision.transform.position.x - 1;
            pos.y = collision.transform.position.y + 1;
            pos.z = 0;*/
            roomClone.transform.position = finalPos;
            //roomClone.transform.position = collision.transform.position - (Vector3) roomClone.GetComponent<roomStats>().doorPos;
            //print(collision.transform.position + (Vector3)roomClone.GetComponent<roomStats>().doorPos);
        }
        else
        {
            rand = UnityEngine.Random.Range(0, roomTypes.Length);
        }
    }
}
