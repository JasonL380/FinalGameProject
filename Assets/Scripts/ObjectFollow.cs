/*
 * Name: Liam Kikin-Gil
 * Date: October 4 2022
 * Desc: Allows objects to follow the specified object
 */
using DefaultNamespace;
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

    private int walkDir;

    public bool will_flip = true;

    [Range(0, 1)]
    public float smoothVal;
    

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        particleSystem = GetComponent<ParticleSystem>();
        particleRenderer = GetComponent<ParticleSystemRenderer>();

        walkDir = follow.gameObject.GetComponent<PlayerController>().walkDir;
    }

    // Update is called once per frame
    void Update()
    {
        walkDir = follow.gameObject.GetComponent<PlayerController>().walkDir;
    }

    private void FixedUpdate()
    {
        if (follow != null)
        {
            //Offsets the object

            //down right = 0, up right = 1, up left = 2, down left = 3
            switch (walkDir)
            {

                case 0:
                    //set offset right
                    if (changeLayer)
                    {
                        spriteRenderer.sortingOrder = rightLayer;
                        particleRenderer.sortingOrder = rightLayer;
                    }

                    lastOffset = rightOffset;
                    spriteRenderer.flipX = true;
                    break;
                case 1:
                    //set offset top
                    if (changeLayer)
                    {
                        spriteRenderer.sortingOrder = topLayer;
                        particleRenderer.sortingOrder = topLayer;
                    }
                    lastOffset = topOffset;
                    break;

                case 2:
                    //set offset left
                    if (changeLayer)
                    {
                        spriteRenderer.sortingOrder = leftLayer;
                        particleRenderer.sortingOrder = leftLayer;
                    }
                    lastOffset = leftOffset;
                    spriteRenderer.flipX = false;
                    break;

                case 3:
                    //set offset bottom
                    if (changeLayer)
                    {
                        spriteRenderer.sortingOrder = bottomLayer;
                        particleRenderer.sortingOrder = bottomLayer;
                    }
                    lastOffset = bottomOffset;
                    break;
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
