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

    int currentSprite = 0;
    private void Start()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        pr = gameObject.GetComponent<ParticleSystemRenderer>();
        currentTime = tick;
    }
    // Update is called once per frame
    void Update()
    {
        if (currentTime < 0 && currentSprite < sprites.Length)
        {
            ++currentSprite;
            sprite.sprite = sprites[currentSprite];
            currentTime = tick;
            Vector3 py = pr.pivot;
            py.y -= decrease[currentSprite];
            pr.pivot = py;
        }
        currentTime -= Time.deltaTime;
    }
}
