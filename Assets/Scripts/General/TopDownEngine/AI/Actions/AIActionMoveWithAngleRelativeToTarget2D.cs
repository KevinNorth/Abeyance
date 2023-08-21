using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using UnityEngine.Serialization;
using MoreMountains.TopDownEngine;

namespace KitTraden.Abeyance.TopDownEngine.Characters.AI.Actions
{
    /// <summary>
    /// Requires a CharacterMovement ability. Makes the character move up to the specified MinimumDistance in the direction of the target, offset by a random angle. 
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionMoveWithAngleRelativeToTarget2D")]
    //[RequireComponent(typeof(CharacterMovement))]
    public class AIActionMoveWithAngleRelativeToTarget2D : AIAction
    {
        [Range(0, 360)] public float MinimumAngle = 45;
        [Range(0, 360)] public float MaximumAngle = 90;

        protected Vector2 _direction;
        protected CharacterMovement _characterMovement;
        protected bool alreadyRan;

        /// <summary>
        /// On init we grab our CharacterMovement ability
        /// </summary>
        public override void Initialization()
        {
            alreadyRan = false;

            if (!ShouldInitialize) return;
            base.Initialization();
            _characterMovement = this.gameObject.GetComponentInParent<Character>()?.FindAbility<CharacterMovement>();

            if (MaximumAngle < MinimumAngle)
            {
                float temp = MaximumAngle;
                MaximumAngle = MinimumAngle;
                MinimumAngle = temp;
            }
        }

        /// <summary>
        /// On PerformAction we move
        /// </summary>
        public override void PerformAction()
        {
            if (!alreadyRan)
            {
                if (_brain.Target == null)
                {
                    return;
                }

                _direction = GetDirection();
                _characterMovement.SetMovement(_direction);
                alreadyRan = true;
            }
        }

        protected Vector2 GetDirection()
        {
            Vector2 vectorToTarget = (_brain.Target.position - this.transform.position).normalized;
            float angleToTarget = Vector2.SignedAngle(Vector2.right, vectorToTarget);

            float randomAngle = Random.Range(MinimumAngle, MaximumAngle) * (Random.value < 0.5f ? -1f : 1f);

            float newAngle = angleToTarget + randomAngle;
            float newRadians = newAngle * Mathf.Deg2Rad;

            return new Vector2(Mathf.Cos(newRadians), Mathf.Sin(newRadians));
        }

        /// <summary>
        /// On exit state we stop our movement
        /// </summary>
        public override void OnExitState()
        {
            base.OnExitState();

            _characterMovement?.SetHorizontalMovement(0f);
            _characterMovement?.SetVerticalMovement(0f);
            alreadyRan = false;
        }
    }
}