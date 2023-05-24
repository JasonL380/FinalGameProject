using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class spriteChange : MonoBehaviour
{
    public SpriteRenderer sprite;
    public ParticleSystemRenderer pr;
    public Sprite[] sprites;

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
        pr = gameObject.GetComponent<ParticleSystemRenderer>();
        initLightPos = pr.pivot;
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
            Vector3 py = pr.pivot;
            py.y -= decrease[currentSprite];
            pr.pivot = py;
        }
        if(currentTime < 0 && currentSprite >= sprites.Length - 1 && GetComponent<ObjectFollow>().follow.GetComponent<PlayerController>().numLights[typeLight] > 0)
        {
            currentSprite = 0;
            currentTime = tick;
            sprite.sprite = sprites[currentSprite];
            --GetComponent<ObjectFollow>().follow.GetComponent<PlayerController>().numLights[typeLight];
            pr.pivot = initLightPos;
        }
        currentTime -= Time.deltaTime;
    }
}
