// Name: Jason Leech
// Date: 05/30/2023
// Desc:

using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    [ExecuteInEditMode]
    public class ItemRenderOrder : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;

        public ParticleSystemRenderer particleSystem;

        public ItemHolder holder;

        public bool hasHolder = false;
        
        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            particleSystem = GetComponent<ParticleSystemRenderer>();
            holder = GetComponentInParent<ItemHolder>();

            if (holder != null)
            {
                hasHolder = true;
            }
        }

        private void Update()
        {
            if (hasHolder)
            {
                if(particleSystem != null)
                {
                    particleSystem.sortingOrder = holder.order + 1;
                }
                spriteRenderer.sortingOrder = holder.order;
            }
        }
    }
}