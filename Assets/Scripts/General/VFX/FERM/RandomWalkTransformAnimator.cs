using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KitTraden.Abeyance.VFX.FERM
{
    public class RandomWalkTransformAnimator : MonoBehaviour
    {
        [SerializeField] Transform transformToAnimate;
        [Header("Position")]
        [SerializeField] bool animatePosition = true;
        [SerializeField] bool useWorldSpace = true;
        [SerializeField] Vector3 initialPositionValue = new Vector3(0f, 0f, 0f);
        [SerializeField] Vector3 initialPositionSpeed = new Vector3(0f, 0f, 0f);
        [SerializeField] Vector3 maxPositionSpeedDelta = new Vector3(0.1f, 0.1f, 0.1f);
        [SerializeField] Vector3 minPositionValue = new Vector3(-1f, -1f, -1f);
        [SerializeField] Vector3 maxPositionValue = new Vector3(1f, 1f, 1f);
        [SerializeField] Vector3 minPositionSpeed = new Vector3(-0.25f, -0.25f, -0.25f);
        [SerializeField] Vector3 maxPositionSpeed = new Vector3(0.25f, 0.25f, 0.25f);
        [Header("Rotation")]
        [SerializeField] bool animateRotation = true;
        [SerializeField] Quaternion initialRotationValue = new Quaternion(0f, 0f, 0f, 0f);
        [SerializeField] Vector4 initialRotationSpeed = new Vector4(0f, 0f, 0f, 0f);
        [SerializeField] Vector4 maxRotationSpeedDelta = new Vector4(0.1f, 0.1f, 0.1f, 0.1f);
        [SerializeField] Vector4 minRotationValue = new Vector4(-90f, -90f, -90f, -90f);
        [SerializeField] Vector4 maxRotationValue = new Vector4(90f, 90f, 90f, 90f);
        [SerializeField] Vector4 minRotationSpeed = new Vector4(-0.5f, -0.5f, -0.5f, -0.5f);
        [SerializeField] Vector4 maxRotationSpeed = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
        [Header("Scale")]
        [SerializeField] bool animateScale = true;
        [SerializeField] Vector3 initialScaleValue = new Vector3(1f, 1f, 1f);
        [SerializeField] Vector3 initialScaleSpeed = new Vector3(0f, 0f, 0f);
        [SerializeField] Vector3 maxScaleSpeedDelta = new Vector3(0.01f, 0.01f, 0.01f);
        [SerializeField] Vector3 minScaleValue = new Vector3(-0.75f, -0.75f, -0.75f);
        [SerializeField] Vector3 maxScaleValue = new Vector3(1.5f, 1.5f, 1.5f);
        [SerializeField] Vector3 minScaleSpeed = new Vector3(-0.025f, -0.025f, -0.025f);
        [SerializeField] Vector3 maxScaleSpeed = new Vector3(0.025f, 0.025f, 0.025f);

        private RandomWalkVector3 positionRandomWalk;
        private RandomWalkQuaternion rotationRandomWalk;
        private RandomWalkVector3 scaleRandomWalk;

        // Start is called before the first frame update
        void Start()
        {
            positionRandomWalk = new RandomWalkVector3(
                initialPositionValue,
                initialPositionSpeed,
                maxPositionSpeedDelta,
                minPositionValue,
                maxPositionValue,
                minPositionSpeed,
                maxPositionSpeed
            );

            rotationRandomWalk = new RandomWalkQuaternion(
                initialRotationValue,
                initialRotationSpeed,
                maxRotationSpeedDelta,
                minRotationValue,
                maxRotationValue,
                minRotationSpeed,
                maxRotationSpeed
            );

            scaleRandomWalk = new RandomWalkVector3(
                initialScaleValue,
                initialScaleSpeed,
                maxScaleSpeedDelta,
                minScaleValue,
                maxScaleValue,
                minScaleSpeed,
                maxScaleSpeed
            );
        }

        // Update is called once per frame
        void Update()
        {
            if (animatePosition)
            {
                if (useWorldSpace)
                {
                    transformToAnimate.position = positionRandomWalk.Update(Time.deltaTime);
                }
                else
                {
                    transformToAnimate.localPosition = positionRandomWalk.Update(Time.deltaTime);
                }
            }

            if (animateRotation)
            {
                transformToAnimate.rotation = rotationRandomWalk.Update(Time.deltaTime);
            }

            if (animateScale)
            {
                transformToAnimate.localScale = scaleRandomWalk.Update(Time.deltaTime);
            }
        }
    }
}