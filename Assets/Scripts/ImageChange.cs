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

    public GameObject candle;
    private void Start()
    {
        image = gameObject.GetComponent<Image>();
        //pr = gameObject.GetComponent<ParticleSystem>();
        //ps = pr.shape;
        //initLightPos = pr.shape.position;
        
    }
    // Update is called once per frame
    void Update()
    {
        image.sprite = candle.GetComponent<SpriteRenderer>().sprite;
    }
}
