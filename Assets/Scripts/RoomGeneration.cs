using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGeneration : MonoBehaviour
{

    public GameObject[] roomTypes;

    // Start is called before the first frame update
    void Start()
    {
        int rand = Random.Range(0, roomTypes.Length);
        if (false/*check room size to make sure it doesn't overlap*/)
        {
            //place room
        }
        else
        {
            rand = Random.Range(0, roomTypes.Length);
        }
    }
}
