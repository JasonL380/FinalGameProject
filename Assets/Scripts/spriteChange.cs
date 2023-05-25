using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class spriteChange : MonoBehaviour
{
    public SpriteRenderer sprite;
    public ParticleSystem pr;
    public Sprite[] sprites;

    private ParticleSystem.ShapeModule ps;

    public float[] decrease;
    public float tick;
    public float currentTime;

    public Vector3 initLightPos;

    //0 = candle, 1 = flashlight
    public int typeLight;

    public int currentSprite = 0;
    private void Start()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        pr = gameObject.GetComponent<ParticleSystem>();
        ps = pr.shape;
        initLightPos = pr.shape.position;
        currentTime = tick;
    }
    // Update is called once per frame
    void Update()
    {
        if (currentTime < 0 && currentSprite < sprites.Length - 1)
        {
            ++currentSprite;
            sprite.sprite = sprites[currentSprite];
            currentTime = tick;
            Vector3 py = ps.position;
            py.y -= decrease[currentSprite];
            ps.position = py;
        }
        else if(currentTime < 0 && currentSprite >= sprites.Length - 1 && GetComponent<ObjectFollow>().follow.GetComponent<PlayerController>().numLights[typeLight] > 0)
        {
            currentSprite = 0;
            currentTime = tick;
            sprite.sprite = sprites[currentSprite];
            --GetComponent<ObjectFollow>().follow.GetComponent<PlayerController>().numLights[typeLight];
            ps.position = initLightPos;
        }
        else if( currentTime < 0 && currentSprite >= sprites.Length - 1 && GetComponent<ObjectFollow>().follow.GetComponent<PlayerController>().numLights[typeLight] <= 0)
        {
            GetComponent<Light2D>().enabled = false;
            GetComponent<ParticleSystemRenderer>().enabled = false;
        }
        currentTime -= Time.deltaTime;
    }
}
