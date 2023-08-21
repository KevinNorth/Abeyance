using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.TopDownEngine;

namespace KitTraden.Abeyance.TopDownEngine.Characters.AI.Decisions
{
    /// <summary>
    /// This decision returns true if an event this AIDecisionEvent is associate with
    /// has fired since entering the state, or false if the event hasn't fired yet.
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/AI/Decisions/AIDecisionEvent")]
    public class AIDecisionEvent : AIDecision
    {
        protected bool _hasEventFired = false;

        public void OnEvent()
        {
            _hasEventFired = true;
        }

        public override void Initialization()
        {
            _hasEventFired = false;
        }

        public override bool Decide()
        {
            return _hasEventFired;
        }

        /// <summary>
        /// On EnterState, resets the hit counter
        /// </summary>
        public override void OnEnterState()
        {
            base.OnEnterState();
            _hasEventFired = false;
        }

        /// <summary>
        /// On exit state, resets the hit counter
        /// </summary>
        public override void OnExitState()
        {
            base.OnExitState();
            _hasEventFired = false;
        }
    }
}