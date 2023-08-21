using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KitTraden.Abeyance.VFX.FERM
{
    public class FERMRandomWalkVector3Animator : MonoBehaviour
    {
        [SerializeField] FERM_Component componentToAnimate;
        [SerializeField] string paramName;
        [SerializeField] Vector3 initialValue = new Vector3(0.5f, 0.5f, 0.5f);
        [SerializeField] Vector3 initialSpeed = new Vector3(0f, 0f, 0f);
        [SerializeField] Vector3 maxSpeedDelta = new Vector3(0.1f, 0.1f, 0.1f);
        [SerializeField] Vector3 minValue = new Vector3(0f, 0f, 0f);
        [SerializeField] Vector3 maxValue = new Vector3(1f, 1f, 1f);
        [SerializeField] Vector3 minSpeed = new Vector3(-0.25f, -0.25f, -0.25f);
        [SerializeField] Vector3 maxSpeed = new Vector3(0.25f, 0.25f, 0.25f);

        private RandomWalkVector3 randomWalk;

        // Start is called before the first frame update
        void Start()
        {
            randomWalk = new RandomWalkVector3(
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