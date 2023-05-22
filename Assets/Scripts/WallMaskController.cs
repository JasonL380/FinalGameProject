using System;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(Rigidbody2D), typeof(PolygonCollider2D))]
    public class WallMaskController : MonoBehaviour
    {
        public GameObject mask;
            
        private void Update()
        {
            RaycastHit2D left = Physics2D.Raycast(transform.position, new Vector2(-1, 0.5f), Mathf.Infinity, LayerMask.GetMask("Wall", "door"));
            RaycastHit2D right = Physics2D.Raycast(transform.position, new Vector2(1, 0.5f), Mathf.Infinity, LayerMask.GetMask("Wall", "door"));
            print("left: " + left.point + " right: " + right.point);

            Vector2 leftPoint = left.point;
            Vector2 rightPoint = right.point;
            
            if (((Vector3) leftPoint - transform.position).magnitude > 7)
            {
                rightPoint = new Vector3(-7, 4.5f) + transform.position;
            }
            
            if (((Vector3) rightPoint - transform.position).magnitude > 7)
            {
                rightPoint = new Vector3(7, 4.5f)  + transform.position;
            }

                Debug.DrawLine(leftPoint, transform.position, Color.red);
            Debug.DrawLine(rightPoint, transform.position, Color.red);

            Vector2 intersect = findIntersect(new Vector3(leftPoint.x, leftPoint.y, 0.5f),
                new Vector3(rightPoint.x, rightPoint.y, -0.5f));
            Debug.DrawLine(intersect, transform.position, Color.green);

            //intersect.x = ((int) intersect.x * (int) 64) / 64f;
            //intersect.y = ((int) intersect.y * (int) 64) / 64f;
            
            mask.transform.position = intersect;
        }

        Vector2 findIntersect(Vector3 vec1, Vector3 vec2)
        {
            float i1 = vec1.y - (vec1.x * vec1.z);
            float i2 = vec2.y - (vec2.x * vec2.z);
            
            Vector2 intersect = Vector2.zero;

            intersect.x = (i2 - i1) / (vec1.z - vec2.z);
            intersect.y = vec1.z * ((i2 - i1) / (vec1.z - vec2.z)) + i1;

            return intersect;
        }
    }
}