using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ImageChange : MonoBehaviour
{
    public Image image;

    //public ParticleSystem pr;
    //private ParticleSystem.ShapeModule ps;

    public float[] decrease;

    //public Vector3 initLightPos;

    //0 = candle, 1 = flashlight

    public float battery;

    public GameObject candle;

    public int holdingItem;

    public Sprite[] flashlightImages;

    public GameObject flashlight;

    public GameObject player;

    private void Start()
    {
        image = gameObject.GetComponent<Image>();
        //pr = gameObject.GetComponent<ParticleSystem>();
        //ps = pr.shape;
        //initLightPos = pr.shape.position;
        battery = flashlight.GetComponent<Flashlight>().currentBattery;
    }
    // Update is called once per frame
    void Update()
    {
        holdingItem = player.GetComponent<PlayerController>().holdingItem;

        if (holdingItem == 1)
        {
            image.sprite = candle.GetComponent<SpriteRenderer>().sprite;
        }

        else if (holdingItem == 2)
        {
            battery = flashlight.GetComponent<Flashlight>().currentBattery - 0.001f;
            image.sprite = flashlightImages[(int)battery];
        }
    }
}
