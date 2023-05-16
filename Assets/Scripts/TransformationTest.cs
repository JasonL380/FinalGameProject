// Name: Jason Leech
// Date: 05/12/2023
// Desc:

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace DefaultNamespace
{
    [ExecuteInEditMode, RequireComponent(typeof(PolygonCollider2D), typeof(BoxCollider2D))]
    public class TransformationTest : MonoBehaviour
    {
        private Matrix4x4 transformMatrix;
        private BoxCollider2D _collider2D;

        private PolygonCollider2D _polygonCollider2D;
        
        public Vector3[] corners;

        private float angle = 45;
        
        private void Start()
        {
            _collider2D = GetComponent<BoxCollider2D>();
            _polygonCollider2D = GetComponent<PolygonCollider2D>();

            //disable the box collider in play mode
            if (Application.isPlaying)
            {
                _collider2D.enabled = false;
            }
        }

        private void Update()
        {
            if (Application.isEditor)
            {
                Bounds bounds = _collider2D.bounds;

                // Vector3 transform.position = bounds.min + bounds.extents;

                bounds.min -= transform.position;
                bounds.max -= transform.position;

                Quaternion rot = Quaternion.Euler(0, 0, 45);
                
                
                /*Gizmos.color = Color.red;
                
                Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, 0.1f,0));
                
                Gizmos.DrawLine(new Vector3(bounds.min.x, bounds.max.y) + transform.position, bounds.max + transform.position);
                Gizmos.DrawLine(new Vector3(bounds.max.x, bounds.min.y) + transform.position, bounds.max + transform.position);
                Gizmos.DrawLine(new Vector3(bounds.max.x, bounds.min.y) + transform.position, bounds.min + transform.position);
                Gizmos.DrawLine(new Vector3(bounds.min.x, bounds.max.y) + transform.position, bounds.min + transform.position);

                Vector3 halfY = new Vector3(1, 0.5f, 1);

                Gizmos.color = Color.blue;
                Gizmos.DrawLine(divideY(rot * transformMatrix.MultiplyPoint(new Vector3(bounds.min.x, bounds.max.y))) + transform.position, divideY(rot * transformMatrix.MultiplyPoint(bounds.max)) + transform.position);
                Gizmos.DrawLine(divideY(rot * transformMatrix.MultiplyPoint(new Vector3(bounds.max.x, bounds.min.y))) + transform.position, divideY(rot * transformMatrix.MultiplyPoint(bounds.max)) + transform.position);
                Gizmos.DrawLine(divideY(rot * transformMatrix.MultiplyPoint(new Vector3(bounds.max.x, bounds.min.y))) + transform.position, divideY(rot * transformMatrix.MultiplyPoint(bounds.min)) + transform.position);
                Gizmos.DrawLine(divideY(rot * transformMatrix.MultiplyPoint(new Vector3(bounds.min.x, bounds.max.y))) + transform.position, divideY(rot * transformMatrix.MultiplyPoint(bounds.min)) + transform.position);
                */
                List<Vector2> points = new List<Vector2>();
                
                transformMatrix = new Matrix4x4(new Vector4(1, 0, 0, 0), new Vector4(0, 1, 0, 0), new Vector4(0, 0, 1, 0), new Vector4(0, 0, 0, 1.41422f));

                points.Add(divideY(rot * transformMatrix.MultiplyPoint(new Vector3(bounds.min.x, bounds.max.y))));
                points.Add(divideY(rot * transformMatrix.MultiplyPoint(bounds.max)));
                points.Add(divideY(rot * transformMatrix.MultiplyPoint(new Vector3(bounds.max.x, bounds.min.y))));
                points.Add(divideY(rot * transformMatrix.MultiplyPoint(bounds.min)));

                
                
                if (_polygonCollider2D == null)
                {
                    _polygonCollider2D = GetComponent<PolygonCollider2D>();
                }
                
                _polygonCollider2D.SetPath(0, points);
            }
        }

        Vector3 divideY(Vector3 vec)
        {
            return new Vector3(vec.x, vec.y * 0.5f, vec.z);
        }
    }
}