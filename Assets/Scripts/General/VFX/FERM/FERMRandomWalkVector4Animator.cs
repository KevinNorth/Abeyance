using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KitTraden.Abeyance.VFX.FERM
{
    public class FERMRandomWalkVector4Animator : MonoBehaviour
    {
        [SerializeField] FERM_Component componentToAnimate;
        [SerializeField] string paramName;
        [SerializeField] Vector4 initialValue = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
        [SerializeField] Vector4 initialSpeed = new Vector4(0f, 0f, 0f, 0f);
        [SerializeField] Vector4 maxSpeedDelta = new Vector4(0.1f, 0.1f, 0.1f, 0.1f);
        [SerializeField] Vector4 minValue = new Vector4(0f, 0f, 0f, 0f);
        [SerializeField] Vector4 maxValue = new Vector4(1f, 1f, 1f, 1f);
        [SerializeField] Vector4 minSpeed = new Vector4(-0.25f, -0.25f, -0.25f, -0.25f);
        [SerializeField] Vector4 maxSpeed = new Vector4(0.25f, 0.25f, 0.25f, 0.25f);

        private RandomWalkVector4 randomWalk;

        // Start is called before the first frame update
        void Start()
        {
            randomWalk = new RandomWalkVector4(
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