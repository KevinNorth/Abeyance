using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using NaughtyAttributes;
using UnityEngine;

namespace KitTraden.Abeyance.TopDownEngine.Characters.AI.Decisions
{
    [AddComponentMenu("TopDown Engine/Character/AI/Decisions/AIDecisionDistanceToObstacle")]
    public class AIDecisionDistanceToObstacle : AIDecision
    {
        /// The possible comparison modes
        public enum ComparisonModes { LowerThan, GreaterThan }
        /// the comparison mode
        [Tooltip("the comparison mode")]
        public ComparisonModes ComparisonMode = ComparisonModes.GreaterThan;
        /// the distance to compare with
        [Tooltip("the distance to compare with")]
        public float Distance;
        [Layer] public string[] ObstacleLayers = new[] { "Obstacles" };

        /// <summary>
        /// On Decide we check our distance to the Target
        /// </summary>
        /// <returns></returns>
        public override bool Decide()
        {
            return EvaluateDistance();
        }

        /// <summary>
        /// Returns true if the distance conditions are met
        /// </summary>
        /// <returns></returns>
        protected virtual bool EvaluateDistance()
        {
            int layerMask = LayerMask.GetMask(ObstacleLayers);
            Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, Distance, layerMask);

            switch (ComparisonMode)
            {
                case ComparisonModes.LowerThan:
                    {
                        return colliders.Length != 0;
                    }
                case ComparisonModes.GreaterThan:
                    {
                        return colliders.Length == 0;
                    }
                default:
                    {
                        throw new System.Exception($"Don't know how to handle ComparisonMode {ComparisonMode}");
                    }
            }
        }
    }
}