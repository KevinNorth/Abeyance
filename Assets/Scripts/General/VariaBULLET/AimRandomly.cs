using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ND_VariaBULLET;
using MoreMountains.TopDownEngine;

namespace KitTraden.Abeyance.VariaBULLET
{
    public class AimRandomly : MonoBehaviour
    {
        public BasePattern BulletPattern;

        void Update()
        {
            BulletPattern.CenterRotation = Random.Range(-360f, 360f);

            foreach (var emitter in BulletPattern.FireScripts)
            {
                emitter.LocalPitch = Random.Range(-180f, 180f);
            }
        }
    }
}