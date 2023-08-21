using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ND_VariaBULLET;
using MoreMountains.TopDownEngine;

namespace KitTraden.Abeyance.VariaBULLET
{
    public class AimInDirectionOfMovement : MonoBehaviour
    {
        public BasePattern BulletPattern;
        public TopDownController2D CharacterController;

        void Update()
        {
            Vector2 movementVector = CharacterController.Speed;

            if (movementVector != Vector2.zero)
            {
                float angle = Vector2.SignedAngle(Vector2.left, movementVector);
                BulletPattern.CenterRotation = angle;
            }
        }
    }
}