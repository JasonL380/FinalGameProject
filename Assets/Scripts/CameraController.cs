/*
 * Name: Liam Kikin-Gil
 * Date: 9/28
 * Desc: A script to cause the camera to follow the player smoothly and has screenshake available (When added to camera)
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Tooltip("Will center on specified GameObject")]
    public GameObject target;

    [Tooltip("Keep between 0 and 1. Centers faster as it approaches 1."), Range(0,1)]
    public float smoothVal = 0.5f;

    //Screenshake variables
    [SerializeField, Tooltip("How long it will shake in seconds if it starts immediately")]
    private static float shakeDuration = 0;
    [SerializeField, Tooltip("How violently it will shake if it starts immediately")]
    private static float shakeMagnitute = 0;

    static float startShakeDuration;
    // Start is called before the first frame update
    void Start()
    {
        startShakeDuration = shakeDuration;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //make sure target exists
        if (target != null)
        {
            //grab the targets location
            Vector3 targetPos = target.transform.position;
            //adjust z value correctly
            targetPos.z = transform.position.z;

            //screen shake effect stuff
            if(shakeDuration > 0)
            {
                shakeDuration -= Time.fixedDeltaTime;

                //setup a random shake amount
                Vector2 randShake = Random.insideUnitCircle * Mathf.Lerp(shakeMagnitute, 0, 1 - (shakeDuration / startShakeDuration));

                transform.position += (Vector3)randShake;
            }

            //move towards that position each fixed update
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothVal);
        }
    }
    //call to start screenshake
    public static void StartShake(float duration, float magnitude)
    {
        //only set if greater than previous values
        if(duration > shakeDuration)
        {
            shakeDuration = duration;
            startShakeDuration = duration;
        }
        if(magnitude > shakeMagnitute)
        {
            shakeMagnitute = magnitude;
        }
    }
}
