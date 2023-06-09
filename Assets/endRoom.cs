using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class endRoom : MonoBehaviour
{
    RoomList roomList;

    private void Start()
    {
        roomList = GetComponentInParent<RoomList>();
    }
    // Update is called once per frame
    void Update()
    {
        if(roomList.hasKey != true)
        {
            GetComponent<PolygonCollider2D>().isTrigger = false;
        }
        else
        {
            GetComponent<PolygonCollider2D>().isTrigger = true;
        }
    }
}
