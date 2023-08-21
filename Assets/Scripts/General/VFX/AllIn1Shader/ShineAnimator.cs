using System;
using System.Collections;
using UnityEngine;

namespace KitTraden.Abeyance.VFX.AllIn1Shader
{
    public class ShineAnimator : MonoBehaviour
    {
        public enum Direction
        {
            ZeroToOne,
            OneToZero
        }

        public Material material;
        [Range(0f, 10f)] public float timeToShine = 0.2f;
        [Range(0f, 60f)] public float timeBetweenShines = 1f;
        public Direction shineDirection = Direction.ZeroToOne;
        public bool useUnscaledTime = false;

        private float timestampOfLastShine;

        void Start()
        {
            timestampOfLastShine = GetTime();
        }

        void Update()
        {
            float currentTime = GetTime();

            if (currentTime - timestampOfLastShine > timeBetweenShines)
            {
                timestampOfLastShine = currentTime;
            }

            float timeIntoShine = currentTime - timestampOfLastShine;

            if (timeIntoShine <= timeToShine)
            {
                float shineFraction = timeIntoShine / timeToShine;
                float shineLocation;

                switch (shineDirection)
                {
                    case Direction.ZeroToOne:
                        {
                            shineLocation = Mathf.Clamp(shineFraction, 0f, 1f);
                            break;
                        }
                    case Direction.OneToZero:
                        {
                            shineLocation = Mathf.Clamp(1 - shineFraction, 0f, 1f);
                            break;
                        }
                    default:
                        {
                            throw new Exception($"Don't know how to handle shine direction of {shineDirection}");
                        }
                }

                material.SetFloat("_ShineLocation", shineLocation);
            }
            else
            {
                // Hide the shine until the next animated pass
                material.SetFloat("_ShineLocation", 0f);
            }
        }

        private float GetTime()
        {
            if (useUnscaledTime)
            {
                return Time.unscaledTime;
            }
            else
            {
                return Time.time;
            }
        }
    }
}