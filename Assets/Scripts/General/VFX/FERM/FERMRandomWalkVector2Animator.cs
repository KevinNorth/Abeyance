using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KitTraden.Abeyance.VFX.FERM
{
    public class FERMRandomWalkVector2Animator : MonoBehaviour
    {
        [SerializeField] FERM_Component componentToAnimate;
        [SerializeField] string paramName;
        [SerializeField] Vector2 initialValue = new Vector2(0.5f, 0.5f);
        [SerializeField] Vector2 initialSpeed = new Vector2(0f, 0f);
        [SerializeField] Vector2 maxSpeedDelta = new Vector2(0.1f, 0.1f);
        [SerializeField] Vector2 minValue = new Vector2(0f, 0f);
        [SerializeField] Vector2 maxValue = new Vector2(1f, 1f);
        [SerializeField] Vector2 minSpeed = new Vector2(-0.25f, -0.25f);
        [SerializeField] Vector2 maxSpeed = new Vector2(0.25f, 0.25f);

        private RandomWalkVector2 randomWalk;

        // Start is called before the first frame update
        void Start()
        {
            randomWalk = new RandomWalkVector2(
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