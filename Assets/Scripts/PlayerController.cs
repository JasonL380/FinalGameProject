using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        public float moveSpeed;
        public float jumpSpeed;
        public float gravity = -9.81f;

        public Vector3 pos3d;
        public Vector3 velocity3d = Vector3.zero;
        private Rigidbody2D myRB2D;
        public bool onGround = true;
        private Collider2D myCol2D;
        public GameObject LandingTarget;
        private Collider2D targetCol2D;

        private SpriteRenderer mySpriteRenderer;

        Animator myAnim;

        public int holdingItem;

        //0 = candle, 1 = flashlight
        public int[] numLights = {0, 0};


        struct ParabolaCastResult
        {
            public RaycastHit2D hit;
            public Vector2 velocity;
        }

        private void Start()
        {
            //print(gameObject.layer);
            pos3d = transform.position;
            myRB2D = GetComponent<Rigidbody2D>();
            myCol2D = GetComponent<Collider2D>();
            targetCol2D = LandingTarget.GetComponent<Collider2D>();
            mySpriteRenderer = GetComponent<SpriteRenderer>();

            myAnim = GetComponent<Animator>();
        }

        private void Update()
        {
            if (holdingItem > 0)
            {
                myAnim.SetBool("Holding", true);
            }
            else
            {
                myAnim.SetBool("Holding", false);
            }
            //fake 3d physics
            if (!onGround)
            {
                if (myCol2D.IsTouching(targetCol2D))
                {
                    onGround = true;
                    gameObject.layer = 7;
                    mySpriteRenderer.rendererPriority = 1;
                    targetCol2D.isTrigger = true;
                    velocity3d.z = 0;
                    transform.position = new Vector3(transform.position.x, pos3d.y);
                }
                else
                {
                    velocity3d.z += gravity * Time.deltaTime;
                }
            }
            else
            {
                /*if (Input.GetButton("Jump"))
                {
                    onGround = false;

                    mySpriteRenderer.rendererPriority = 2;
                    gameObject.layer = 8;
                    targetCol2D.isTrigger = false;
                    velocity3d.z = jumpSpeed;
                    pos3d.y = transform.position.y;
                    transform.position = new Vector3(transform.position.x, transform.position.y + 0.25f);
                    pos3d.z = 0;

                    ParabolaCast(velocity3d, pos3d);
                }*/
                pos3d.y = transform.position.y;
                velocity3d.x = Input.GetAxis("Horizontal") * moveSpeed;

                velocity3d.y = Input.GetAxis("Vertical") * moveSpeed;



                //LandingTarget.transform.position = transform.position;

            }

            pos3d.x = transform.position.x;
            pos3d += velocity3d * Time.deltaTime;

            LandingTarget.transform.position = new Vector2(pos3d.x, pos3d.y);

            myRB2D.velocity = new Vector2(velocity3d.x, (velocity3d.y * 0.5f) + (velocity3d.z / 2));



            //animations
            Vector2 movement = Vector2.zero;

            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            if (movement == Vector2.zero) //if not walking
            {
                myAnim.SetBool("Walking", false);
            }
            else //if walking
            {
                myAnim.SetBool("Walking", true);

                myAnim.SetFloat("Move_X", movement.x);
                myAnim.SetFloat("Move_Y", movement.y);
            }
        }


        ParabolaCastResult ParabolaCast(Vector3 velocity, Vector3 start)
        {
            //Vector2 start2d = new Vector2(start.x, start.y + start.z / 2);

            ParabolaCastResult result = new ParabolaCastResult();
            RaycastHit2D hit;
            Vector3 nextPos;

            int count = 0;

            do
            {
                nextPos = start + (velocity / 20);
                velocity.z += gravity / 20;
                hit = Physics2D.Linecast(to2d(start), to2d(nextPos - start));
                Debug.DrawLine(to2d(start), to2d(nextPos - start), Color.magenta, 30);
                start = nextPos;
                count++;
            } while (hit.collider == null && count < 100);

            print(count);
            
            result.hit = hit;
            result.velocity = velocity;

            return result;
        }
        
        Vector2 to2d(Vector3 vec)
        {
            return new Vector2(vec.x, vec.y + vec.z / 2);
        }
    }
}