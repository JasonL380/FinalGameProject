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
    public ParticleSystem pr;
    public Sprite[] images;

    private ParticleSystem.ShapeModule ps;

    public float[] decrease;
    public float tick;
    public float currentTime;

    public Vector3 initLightPos;

    //0 = candle, 1 = flashlight
    public int typeLight;

    public int currentImage = 0;

    public GameObject player;
    private void Start()
    {
        image = gameObject.GetComponent<Image>();
        pr = gameObject.GetComponent<ParticleSystem>();
        ps = pr.shape;
        initLightPos = pr.shape.position;
        currentTime = tick;
    }
    // Update is called once per frame
    void Update()
    {
        if (currentTime < 0 && currentImage < images.Length - 1)
        {
            ++currentImage;
            image.sprite = images[currentImage];
            currentTime = tick;
            Vector3 py = ps.position;
            py.y -= decrease[currentImage];
            ps.position = py;
        }
        else if (currentTime < 0 && currentImage >= images.Length - 1 && player.GetComponent<PlayerController>().numLights[typeLight] > 0)
        {
            currentImage = 0;
            currentTime = tick;
            image.sprite = images[currentImage];
            --player.GetComponent<PlayerController>().numLights[typeLight];
            ps.position = initLightPos;
        }
        else if (currentTime < 0 && currentImage >= images.Length - 1 && player.GetComponent<PlayerController>().numLights[typeLight] <= 0)
        {
            //GetComponent<Light2D>().enabled = false;
            GetComponent<ParticleSystemRenderer>().enabled = false;
        }
        currentTime -= Time.deltaTime;
    }
}
