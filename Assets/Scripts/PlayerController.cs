using System;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

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

        //0 = nothing, 1 = candle, 2 = flashlight
        public int holdingItem;
        public GameObject[] lights;
        public int[] numLights = {0, 0};

        public TMP_Text candlestxt;
        public TMP_Text batteriestxt;

        public GameObject[] lightIndicator;

        public GameObject flashlight;

        //up right = 0, up left = 1, down left = 2, down right = 3
        public int walkDir = 0;

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
            if (flashlight != null)
            {
                //numLights[1] = flashlight.GetComponent<Flashlight>().numBatteries;
            }
            else
            {
                numLights[1] = 0;
            }
        }

        private void Update()
        {
            if (flashlight != null)
            {
                numLights[1] = flashlight.GetComponent<Flashlight>().numBatteries;
            }
            else
            {
                numLights[1] = 0;
            }

            candlestxt.text = "x" + numLights[0];
            batteriestxt.text = "x" + numLights[1];


            if (holdingItem > 0)
            {
                myAnim.SetBool("Holding", true);
            }
            else
            {
                myAnim.SetBool("Holding", false);
            }

            if(Input.GetKeyDown(KeyCode.Q))
            {
                holdingItem = (holdingItem + 1) % (numLights.Length + 1);
                switch(holdingItem)
                {
                    case 0: //candle off flashlight off
                        lights[0].SetActive(false);
                        lights[1].SetActive(false);
                        break;
                    case 1: //candle on flashlight off
                        lights[0].SetActive(true);
                        lights[1].SetActive(false);
                        break;
                    case 2: //candle off flashlight on
                        lights[0].SetActive(false);
                        lights[1].SetActive(true);
                        break;
                }
            }
            
            pos3d.y = transform.position.y;
            velocity3d.x = Input.GetAxis("Horizontal") * moveSpeed;

            velocity3d.y = Input.GetAxis("Vertical") * moveSpeed;

            Vector2 velocity = Vector2.zero;
           
            velocity += Input.GetAxis("Horizontal") * moveSpeed * new Vector2(-1, 0.5f);

            velocity += Input.GetAxis("Vertical") * moveSpeed * new Vector2(-1, -0.5f);
            velocity *= new Vector2(-1, -1);
            pos3d.x = transform.position.x;
            pos3d += velocity3d * Time.deltaTime;

            //LandingTarget.transform.position = new Vector2(pos3d.x, pos3d.y);

            velocity = velocity.normalized;
            
            myRB2D.velocity = velocity * moveSpeed;//new Vector2(velocity3d.x, (velocity3d.y * 0.5f) + (velocity3d.z / 2));
            

            //animations

            if (velocity == Vector2.zero) //if not walking
            {
                myAnim.SetBool("Walking", false);
            }
            else //if walking
            {
                myAnim.SetBool("Walking", true);

                myAnim.SetFloat("Move_X", velocity.x > 0 ? 1 : -1);
                myAnim.SetFloat("Move_Y", velocity.y > 0 ? 1 : -1);
            }
            
            //walking direction
            //up right = 0, up left = 1, down left = 2, down right = 3
            if (Input.GetAxis("Horizontal") > 0)
            {
                walkDir = 0;
            }
            else if (Input.GetAxis("Vertical") > 0)
            {
                walkDir = 1;
            }
            else if (Input.GetAxis("Horizontal") < 0)
            {
                walkDir = 2;
            }
            else if (Input.GetAxis("Vertical") < 0)
            {
                walkDir = 3;
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