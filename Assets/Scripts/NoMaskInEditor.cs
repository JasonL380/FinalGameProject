using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DefaultNamespace
{
    public class NoMaskInEditor : MonoBehaviour
    {
        public SpriteMaskInteraction MaskInteraction;
        public void Start()
        {
            GetComponent<TilemapRenderer>().maskInteraction = MaskInteraction;
        }
    }
}