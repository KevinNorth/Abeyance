using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KitTraden.Abeyance.VFX.FERM
{
    public class FERMRandomWalkFloatAnimator : MonoBehaviour
    {
        [SerializeField] FERM_Component componentToAnimate;
        [SerializeField] string paramName;
        [SerializeField] float initialValue = 0.5f;
        [SerializeField] float initialSpeed = 0f;
        [SerializeField] float maxSpeedDelta = 0.1f;
        [SerializeField] float minValue = 0f;
        [SerializeField] float maxValue = 1f;
        [SerializeField] float minSpeed = -0.25f;
        [SerializeField] float maxSpeed = 0.25f;

        private RandomWalkFloat randomWalk;

        // Start is called before the first frame update
        void Start()
        {
            randomWalk = new RandomWalkFloat(
                initialValue,
                initialSpeed,
                maxSpeedDelta,
                minValue,
                maxValue,
                minSpeed,
                maxSpeed
            );
        }

        // Update is called once per frame
        void Update()
        {
            randomWalk.Update(Time.deltaTime);
            componentToAnimate.SetParam(paramName, randomWalk.value);
        }
    }
}