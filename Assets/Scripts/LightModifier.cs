using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightModifier : MonoBehaviour
{
    private Light2D light;
    private float initLight;

    private float random;

    private float timer;
    public float freq;
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
            random = Random.Range(initLight - 1, initLight + 1);
            light.pointLightOuterRadius = random;
            timer = freq;
        }
        timer -= Time.deltaTime;
    }
}
