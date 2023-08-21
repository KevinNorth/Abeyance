using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KitTraden.Abeyance.VFX.FERM
{
    public class FERMRandomWalkTextureScrollAnimator : MonoBehaviour
    {
        [SerializeField] Material materialToAnimate;
        [SerializeField] Vector2 initialSpeed = new Vector2(0f, 0f);
        [SerializeField] Vector2 maxSpeedDelta = new Vector2(0.1f, 0.1f);
        [SerializeField] Vector2 minSpeed = new Vector2(-0.25f, -0.25f);
        [SerializeField] Vector2 maxSpeed = new Vector2(0.25f, 0.25f);

        private RandomWalkVector2 randomWalk;

        // Start is called before the first frame update
        void Start()
        {
            var initialValue = materialToAnimate.GetTextureOffset("_MainTex");
            var maxValue = materialToAnimate.GetTextureScale("_MainTex");

            randomWalk = new RandomWalkVector2(
                initialValue,
                initialSpeed,
                maxSpeedDelta,
                Vector2.zero,
                maxValue,
                minSpeed,
                maxSpeed
            );
        }

        // Update is called once per frame
        void Update()
        {
            randomWalk.Update(Time.deltaTime);
            materialToAnimate.SetTextureOffset("_MainTex", randomWalk.value);
        }
    }
}