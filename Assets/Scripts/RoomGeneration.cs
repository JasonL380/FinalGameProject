using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RoomGeneration : MonoBehaviour
{

    public GameObject[] roomTypes;

    private bool roomGend;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(/*!roomGend &&*/ collision.name.Equals("Player"))
        {
            int rand = UnityEngine.Random.Range(0, roomTypes.Length);
            if (true/*check room size to make sure it doesn't overlap*/)
            {
                Vector2 box;
                box.x = 2;
                box.y = 2;
                RaycastHit2D bc = Physics2D.BoxCast(collision.transform.position, box, 0, box, 10000f, LayerMask.NameToLayer("door"));
                print(LayerMask.NameToLayer("door"));
                print(bc.collider.bounds.center);
                print(bc.collider.name);
                GameObject roomClone = Instantiate(roomTypes[rand]);
                roomClone.transform.parent = GameObject.Find("Grid").transform;
                roomClone.SetActive(true);
                Vector3Int doorPos = GetComponentInParent<Grid>().WorldToCell(bc.transform.position);
                //print(collision.transform.position);
                //print(doorPos);
                doorPos.x -= (int)(roomClone.transform.position.x) + (int)roomClone.GetComponent<roomStats>().doorPos.x;
                doorPos.y -= (int)(roomClone.transform.position.y) + (int)roomClone.GetComponent<roomStats>().doorPos.y;
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
                roomGend = true;
            }
            else
            {
                rand = UnityEngine.Random.Range(0, roomTypes.Length);
            }
        }
    }
}
