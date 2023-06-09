using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InteractableObject : MonoBehaviour
{

    public int randLight;

    public bool beenRansacked;

    RoomList roomList;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Collider2D>().isTrigger = true;
        randLight = Random.Range(0, 51);
        beenRansacked = false;
        roomList = GetComponentInParent<RoomList>();
    }

    // Update is called once per frame
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetKeyDown(KeyCode.E) && collision.name.Equals("Player") && !beenRansacked)
        {
            if(roomList.hasKey == false && roomList.numInteractableObjects - roomList.numRansackedObjects == 1)
            {
                roomList.hasKey = true;
            }
            else
            {
                if (randLight == 51 && roomList.hasKey != true)
                {
                    //give key
                    roomList.hasKey = true;
                    roomList.numRansackedObjects++;
                    beenRansacked = true;
                }
                else
                {
                    if (randLight % 2 == 0)
                    {
                        //give candle
                        ++collision.GetComponent<PlayerController>().numLights[0];
                    }
                    else if (randLight % 2 == 1)
                    {
                        //give battery
                        ++collision.GetComponent<PlayerController>().numLights[1];
                    }
                    roomList.numRansackedObjects++;
                }
            }
            beenRansacked = true;
        }
    }
    
}
