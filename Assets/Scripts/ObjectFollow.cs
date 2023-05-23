/*
 * Name: Liam Kikin-Gil
 * Date: October 4 2022
 * Desc: Allows objects to follow the specified object
 */
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ObjectFollow : MonoBehaviour
{
    public GameObject follow;
    private new ParticleSystem particleSystem;

    private SpriteRenderer spriteRenderer;
    private ParticleSystemRenderer particleRenderer;

    public Vector3 leftOffset;
    public Vector3 rightOffset;
    public Vector3 topOffset;
    public Vector3 bottomOffset;
    public bool changeLayer = true;
    public int leftLayer;
    public int rightLayer;
    public int topLayer;
    public int bottomLayer;
    private Vector3 lastOffset;

    public bool will_flip = true;

    [Range(0, 1)]
    public float smoothVal;
    

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        particleSystem = GetComponent<ParticleSystem>();
        particleRenderer = GetComponent<ParticleSystemRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (follow != null)
        {
            //Offsets the object

            Vector2 movement = Vector2.zero;

            //Get the input to see what way the player is movingn using Unity's old input system
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

 
            if (movement.x != 0)
            {
                if (movement.x > 0)
                {
                    //set offset right
                    if (changeLayer)
                    {
                        spriteRenderer.sortingOrder = rightLayer;
                        particleRenderer.sortingOrder = rightLayer;
                    }

                    lastOffset = rightOffset;
                    spriteRenderer.flipX = true;


                }
                else if (movement.x < 0)
                {
                    //set offset left
                    if (changeLayer)
                    {
                        spriteRenderer.sortingOrder = leftLayer;
                        particleRenderer.sortingOrder = leftLayer;
                    }
                    lastOffset = leftOffset;
                    spriteRenderer.flipX = false;
                }
            }
            if (movement.y != 0)
            {
                if (movement.y > 0)
                {
                    //set offset top
                    if (changeLayer)
                    {
                        spriteRenderer.sortingOrder = topLayer;
                        particleRenderer.sortingOrder = topLayer;
                    }
                    lastOffset = topOffset;

                }
                else if (movement.y < 0)
                {
                    //set offset bottom
                    if (changeLayer)
                    {
                        spriteRenderer.sortingOrder = bottomLayer;
                        particleRenderer.sortingOrder = bottomLayer;
                    }
                    lastOffset = bottomOffset;
                }
            }

            //grab the targets location
            Vector3 targetPos = follow.transform.position + lastOffset;

            //adjust z value correctly
            targetPos.z = transform.position.z;


            //move towards that position each fixed update
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothVal);
        }
    }

    //Sets the target to follow
    public void setFollow(GameObject target)
    {
        follow = target;
    }
}
