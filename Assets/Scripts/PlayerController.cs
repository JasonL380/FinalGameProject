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
        }

        private void Update()
        {
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
                if (Input.GetButton("Jump"))
                {
                    onGround = false; 
                    
                    mySpriteRenderer.rendererPriority = 2;
                    gameObject.layer = 8;
                    targetCol2D.isTrigger = false;
                    velocity3d.z = jumpSpeed;
                    pos3d.y = transform.position.y;
                    transform.position = new Vector3(transform.position.x, transform.position.y + 0.25f);
                    pos3d.z = 0;
                }
                else
                {
                    pos3d.y = transform.position.y;
                }

                velocity3d.x = Input.GetAxis("Horizontal") * moveSpeed;

                velocity3d.y = Input.GetAxis("Vertical") * moveSpeed;

                

                //LandingTarget.transform.position = transform.position;

            }
            
            pos3d.x = transform.position.x;
            pos3d += velocity3d * Time.deltaTime;

            LandingTarget.transform.position = new Vector2(pos3d.x, pos3d.y);
            
            myRB2D.velocity = new Vector2(velocity3d.x, velocity3d.y + velocity3d.z);
        }
        
        
        ParabolaCastResult ParabolaCast(Vector2 velocity, Vector2 start)
        {
            ParabolaCastResult result = new ParabolaCastResult();
            RaycastHit2D hit;
            Vector2 nextPos;

            int count = 0;
            
            do
            {
                nextPos = start + (velocity / 20);
                velocity += Physics2D.gravity / 20;
                hit = Physics2D.CircleCast(start, 0.5f, nextPos - start, (nextPos - start).magnitude, 0);
                Debug.DrawLine(start, nextPos, Color.magenta, 30);
                start = nextPos;
                count++;
            } while (hit.collider == null && count < 100);

            result.hit = hit;
            result.velocity = velocity;
            
            return result;
        }
    }
}