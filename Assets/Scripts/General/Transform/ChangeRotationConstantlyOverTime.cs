using System;
using UnityEngine;

namespace KitTraden.Abeyance.General.Transform
{
    public class ChangeRotationConstantlyOverTime : MonoBehaviour
    {
        [Range(-7200f, 7200f)] public float Speed = 10f;
        public bool UseUnscaledTime = false;
        new private UnityEngine.Transform transform;

        void Start()
        {
            transform = GetComponent<UnityEngine.Transform>();
        }

        void Update()
        {
            float deltaTime = UseUnscaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
            float anglesToRotate = Speed * deltaTime;
            transform.Rotate(new Vector3(0, 0, anglesToRotate), Space.World);
        }
    }
}