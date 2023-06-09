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

    public bool playerNear;
    public Collider2D player;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Collider2D>().isTrigger = true;
        randLight = Random.Range(0, 52);
        beenRansacked = false;
        roomList = GetComponentInParent<RoomList>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name.Equals("Player"))
        {
            playerNear = true;
            player = collision;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name.Equals("Player"))
        {
            playerNear = false;
            player = collision;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerNear && !beenRansacked)
        {
            //print("E " + randLight);
            if (roomList.hasKey == false && roomList.numInteractableObjects - roomList.numRansackedObjects == 1)
            {
                //print("Shouldn't show");
                roomList.hasKey = true;
            }
            else
            {
                if (randLight == 51 && roomList.hasKey != true)
                {
                    //print("key should be given");
                    //give key
                    roomList.hasKey = true;
                    roomList.numRansackedObjects++;
                    beenRansacked = true;
                }
                else
                {
                    //print("Light should be given");
                    if (randLight % 2 == 0)
                    {
                        //give candle
                        ++player.GetComponent<PlayerController>().numLights[0];
                    }
                    else if (randLight % 2 == 1)
                    {
                        //give battery
                        ++player.GetComponent<PlayerController>().numLights[1];
                    }
                    roomList.numRansackedObjects++;
                }
            }
            beenRansacked = true;
        }
    }

}
