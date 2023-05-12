using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class RoomGeneration : MonoBehaviour
{
    //kinds of rooms
    public GameObject[] roomTypes;
    //which rooms its tried to generate
    private int[] tried;
    private int count;

    private void Start()
    {
        tried = new int[roomTypes.Length];
    }

    //the way into the next room through the door up right = 0, up left = 1, down left = 2, down right = 3
    public int facing;
    //has the room generated
    private bool roomGend;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if no room has generated and the player collides
        if(!roomGend && collision.name.Equals("Player") )
        {
            //get a random room
            int rand = UnityEngine.Random.Range(0, roomTypes.Length);
            //randomly picks from the list until it's exhausted or it finds a matching one
            while(roomTypes[rand].GetComponent<roomStats>().facing != facing/*TODO check room size to make sure it doesn't overlap*/)
            {
                if (tried[rand] != 1)
                {
                    ++count;
                    tried[rand] = 1;
                }
                rand = UnityEngine.Random.Range(0, roomTypes.Length);
                if(count == roomTypes.Length)
                {
                    return;
                }
            }
            //get the rounded position so it's consistent
            Vector3 pos = collision.transform.position;
            pos.x = (float)Math.Round(collision.transform.position.x);
            pos.y = (float)Math.Round(collision.transform.position.y);

            //make the room 
            GameObject roomClone = Instantiate(roomTypes[rand]);
            roomClone.transform.parent = GameObject.Find("Grid").transform;
            roomClone.SetActive(true);

            //get the position of the door for centering it
            Vector3Int doorPos = GetComponentInParent<Grid>().WorldToCell(pos);
            //some math to properly align the door
            doorPos.x -= (int)(roomClone.transform.position.x) + (int)roomClone.GetComponent<roomStats>().doorPos.x;
            doorPos.y -= (int)(roomClone.transform.position.y) + (int)roomClone.GetComponent<roomStats>().doorPos.y;
            doorPos.z -= (int)(roomClone.transform.position.z);
            Vector3 finalPos = GetComponentInParent<Grid>().CellToWorld(doorPos);
            //put the room in it's proper place
            roomClone.transform.position = finalPos;
            //don't let any other rooms generate
            roomGend = true; 
        }
    }
}
