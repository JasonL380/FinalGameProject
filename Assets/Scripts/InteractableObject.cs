using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InteractableObject : MonoBehaviour
{

    public int randLight;

    private bool beenRansacked;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Collider2D>().isTrigger = true;
        randLight = Random.Range(1, 51);
        beenRansacked = false;
    }

    // Update is called once per frame
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetKeyDown(KeyCode.E) && collision.name.Equals("Player") && !beenRansacked)
        {
            if(randLight == 51)
            {
                //give key
            }
            else
            {
                if(randLight % 2 == 0)
                {
                    //give candle
                    ++collision.GetComponent<PlayerController>().numLights[0];
                }
                else if(randLight % 2 == 1)
                {
                    //give battery
                    ++collision.GetComponent<PlayerController>().numLights[1];
                }
            }
            beenRansacked = true;
        }
    }
    
}
