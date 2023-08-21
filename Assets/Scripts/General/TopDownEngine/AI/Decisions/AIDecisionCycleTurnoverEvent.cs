using UnityEngine;
using KitTraden.Abeyance.Core.Cycles;

namespace KitTraden.Abeyance.TopDownEngine.Characters.AI.Decisions
{
    /// <summary>
    /// This decision returns true if a cycle turnover has occurred since
    /// entering the state, or false if not.
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/AI/Decisions/AIDecisionCycleTurnoverEvent")]
    public class AIDecisionCycleTurnoverEvent : AIDecisionEvent
    {
        protected CycleManager _cycleManager;

        public override void Initialization()
        {
            base.Initialization();
            _cycleManager = GameObject.FindObjectOfType<CycleManager>();
        }

        protected void OnCycleTurnover(int _)
        {
            OnEvent();
        }

        /// <summary>
		/// Grabs our health component and starts listening for OnHit events
		/// </summary>
		protected virtual void OnEnable()
        {
            if (_cycleManager == null)
            {
                _cycleManager = GameObject.FindObjectOfType<CycleManager>();
            }

            if (_cycleManager != null)
            {
                _cycleManager.AddCycleTurnoverListener(OnCycleTurnover);
            }
        }

        /// <summary>
        /// Stops listening for OnHit events
        /// </summary>
        protected virtual void OnDisable()
        {
            if (_cycleManager != null)
            {
                _cycleManager.RemoveCycleTurnoverListener(OnCycleTurnover);
            }
        }
    }
}