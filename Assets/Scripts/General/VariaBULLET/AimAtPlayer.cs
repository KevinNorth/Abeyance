using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ND_VariaBULLET;
using MoreMountains.TopDownEngine;

namespace KitTraden.Abeyance.VariaBULLET
{
    public class AimAtPlayer : MonoBehaviour
    {
        public BasePattern BulletPattern;

        protected LevelManager levelManager;
        protected Transform patternTransform;
        protected Transform playerTransform;

        void Start()
        {
            Init();
        }

        void OnEnable()
        {
            Init();
        }

        protected void Init()
        {
            levelManager = GameObject.FindObjectOfType<LevelManager>();
            patternTransform = BulletPattern.transform;
            playerTransform = levelManager?.Players?[0]?.transform;
        }

        void Update()
        {
            if (patternTransform == null || playerTransform == null)
            {
                Init();
            }
            else
            {
                Vector2 vectorToTarget = patternTransform.position - playerTransform.position;
                float angle = Vector2.SignedAngle(Vector2.left, vectorToTarget);
                BulletPattern.CenterRotation = angle;
            }
        }
    }
}