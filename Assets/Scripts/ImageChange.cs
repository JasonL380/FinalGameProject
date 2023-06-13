using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
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

    public float initBattery;

    public GameObject candle;

    public int holdingItem;

    public Sprite[] flashlightImages;

    public GameObject flashlight;

    public GameObject player;

    public Sprite[] candleImages;


    private void Start()
    {
        image = gameObject.GetComponent<Image>();
        //pr = gameObject.GetComponent<ParticleSystem>();
        //ps = pr.shape;
        //initLightPos = pr.shape.position;
        battery = flashlight.GetComponent<Flashlight>().currentBattery;
        initBattery = flashlight.GetComponent<Flashlight>().totalBattery;

    }
    // Update is called once per frame
    void Update()
    {
        holdingItem = player.GetComponent<PlayerController>().holdingItem;

        if (holdingItem == 0 )
        {
            image.enabled = false;
        }
        
        if (holdingItem == 1)
        {
            image.enabled = true;
            image.sprite = candleImages[candle.GetComponent<spriteChange>().currentSprite];
        }

        else if (holdingItem == 2)
        {
            image.enabled = true;
            battery = flashlight.GetComponent<Flashlight>().currentBattery - 0.001f;
            image.sprite = flashlightImages[(int) (battery/(initBattery/9))];   
        }
    }
}
