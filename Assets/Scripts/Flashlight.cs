using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{

    public GameObject player;

    public GameObject fLight;

    public int totalBattery = 9;
    public float currentBattery;

    // Start is called before the first frame update
    void Start()
    {
        currentBattery = totalBattery;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBattery > 0 + Time.deltaTime)
        {
            fLight.SetActive(true);
            currentBattery -= Time.deltaTime;
        }
        else
        {
            if (player.GetComponent<PlayerController>().numLights[1] > 0)
            {
                currentBattery = totalBattery;
                player.GetComponent<PlayerController>().numLights[1]--;
            }
        }

        if (player.GetComponent<PlayerController>().numLights[1] == 0 && currentBattery < 1)
        {
            fLight.SetActive(false);
        }
    }
}
