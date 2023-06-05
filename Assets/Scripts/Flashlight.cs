using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public int numBatteries;

    public GameObject player;

    public GameObject fLight;

    public int totalBattery = 9;
    public float currentBattery;

    // Start is called before the first frame update
    void Start()
    {
        numBatteries = player.GetComponent<PlayerController>().numLights[1];
        currentBattery = totalBattery;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBattery > 0 + Time.deltaTime)
        {
            currentBattery -= Time.deltaTime;
        }
        else
        {
            if (numBatteries > 0)
            {
                currentBattery = totalBattery;
                numBatteries--;
            }
        }

        if (numBatteries == 0 && currentBattery < 1)
        {
            fLight.SetActive(false);
        }
    }
}
