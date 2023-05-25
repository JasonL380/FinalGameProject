using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightModifier : MonoBehaviour
{
    private Light2D light;
    private float initLight;
    private float lightRandom;

    private float timer;
    public float frequency;
    public float margin;
    private float time;
    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light2D>();
        initLight = light.pointLightOuterRadius;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < 0)
        {
            lightRandom = Random.Range(initLight - 1, initLight + 1);
            light.pointLightOuterRadius = lightRandom;
            time = Random.Range(frequency - margin, frequency + margin);
            timer = time;
        }
        timer -= Time.deltaTime;
    }
}
