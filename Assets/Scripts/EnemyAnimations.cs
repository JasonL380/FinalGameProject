using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimations : MonoBehaviour
{
    Animator myAnim;
    private Rigidbody2D myRB2D;

    //public int currentDir;

    public Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        myAnim = GetComponent<Animator>();
        myRB2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        velocity = myRB2D.velocity;
        myAnim.SetFloat("Move_X", velocity.x > 0 ? 1 : -1);
        myAnim.SetFloat("Move_Y", velocity.y > 0 ? 1 : -1);
    }
}
