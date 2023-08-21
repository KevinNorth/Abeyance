using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using MoreMountains.TopDownEngine;
using KitTraden.Abeyance.Core.Cycles;
using MMHealth = MoreMountains.TopDownEngine.Health;

namespace KitTraden.Abeyance.TopDownEngine.Characters.Health
{
    /// <summary>
    /// Add this class to a character or object with a Health class, and its health will auto refill based on the settings here
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/Health/Health Auto Refill on Cycle Turnover")]
    public class HealthAutoRefillOnCycleTurnover : TopDownMonoBehaviour
    {
        public enum RefillModes { Constant, Fraction, FractionOfMissing }

        [Header("Mode")]
        /// the selected refill mode 
        [Tooltip("the selected refill mode ")]
        public RefillModes RefillMode;

        [Header("Refill Settings")]
        /// if this is true, health will refill itself when not at full health
        [Tooltip("if this is true, health will refill itself when not at full health")]
        public bool RefillHealth = true;
        /// the amount of health to restore at cycle turnover when in cycle turnover constant mode
        [MMEnumCondition("RefillMode", (int)RefillModes.Constant)]
        [Tooltip("the amount of health to restore at cycle turnover when in cycle turnover constant mode")]
        public float HealthPerTurnover = 25f;
        /// the portion of health to restore at cycle turnover when in cycle turnover fraction mode
        [MMEnumCondition("RefillMode", (int)RefillModes.Fraction)]
        [Tooltip("the portion of health to restore at cycle turnover when in cycle turnover fraction mode")]
        [Range(0f, 1f)] public float HealthFractionPerTurnover = 0.5f;
        /// the fraction of missing health to restore at cycle turnover when in cycle turnover fraction of empty mode
        [MMEnumCondition("RefillMode", (int)RefillModes.FractionOfMissing)]
        [Tooltip("the fraction of missing health to restore at cycle turnover when in cycle turnover fraction of empty mode")]
        [Range(0f, 1f)] public float HealthFractionOfMissingPerTurnover = 0.5f;

        protected CycleManager _cycleManager;
        protected MMHealth _health;

        /// <summary>
        /// On Awake we do our init
        /// </summary>
        protected virtual void Awake()
        {
            Initialization();
        }

        /// <summary>
        /// On init we grab our Health component
        /// </summary>
        protected virtual void Initialization()
        {
            _health = this.gameObject.GetComponent<MMHealth>();
            _cycleManager = GameObject.FindObjectOfType<CycleManager>();
        }

        /// <summary>
        /// On cycle turnover we refill
        /// </summary>

        protected virtual void OnCycleTurnover(int _nextCycle)
        {
            ProcessRefillHealth();
        }

        /// <summary>
        /// Tests if a refill is needed during a cycle turnover and processes it
        /// </summary>
        protected virtual void ProcessRefillHealth()
        {
            if (!RefillHealth)
            {
                return;
            }

            if (_health.CurrentHealth < _health.MaximumHealth)
            {
                switch (RefillMode)
                {
                    case RefillModes.Constant:
                        {
                            _health.ReceiveHealth(HealthPerTurnover, this.gameObject);
                            break;
                        }
                    case RefillModes.Fraction:
                        {
                            float healthToGive = _health.MaximumHealth * HealthFractionPerTurnover;
                            _health.ReceiveHealth(healthToGive, this.gameObject);
                            break;
                        }
                    case RefillModes.FractionOfMissing:
                        {
                            float missingHealth = _health.MaximumHealth - _health.CurrentHealth;
                            float healthToGive = missingHealth * HealthFractionOfMissingPerTurnover;
                            _health.ReceiveHealth(healthToGive, this.gameObject);
                            break;
                        }
                    default:
                        {
                            // All other cases are dealt with in the Update() handler
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// On enable we start listening for hits
        /// </summary>
        protected virtual void OnEnable()
        {
            _cycleManager.AddCycleTurnoverListener(OnCycleTurnover);
        }

        /// <summary>
        /// On disable we stop listening for hits
        /// </summary>
        protected virtual void OnDisable()
        {
            _cycleManager.RemoveCycleTurnoverListener(OnCycleTurnover);
        }
    }
}