using System;
using System.Collections;
using UnityEngine;

namespace KitTraden.Abeyance.VFX.AllIn1Shader
{
    public class OverlayTextureScroller : MonoBehaviour
    {
        public Material material;
        public float xSpeed = 0f;
        public float ySpeed = 0f;
        public bool useUnscaledTime = false;

        void Update()
        {
            var currentOffset = material.GetTextureOffset("_OverlayTex");
            var newOffset = new Vector2(
                currentOffset.x + (xSpeed * GetDeltaTime()),
                currentOffset.y + (ySpeed * GetDeltaTime())
            );
            material.SetTextureOffset("_OverlayTex", newOffset);
        }

        private float GetDeltaTime()
        {
            if (useUnscaledTime)
            {
                return Time.unscaledDeltaTime;
            }
            else
            {
                return Time.deltaTime;
            }
        }
    }
}