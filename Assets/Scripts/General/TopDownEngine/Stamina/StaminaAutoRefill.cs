using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using MoreMountains.TopDownEngine;
using KitTraden.Abeyance.Core.Cycles;

namespace KitTraden.Abeyance.TopDownEngine.Characters.Stamina
{
    /// <summary>
    /// Add this class to a character or object with a Stamina class, and its stamina will auto refill based on the settings here
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/Stamina/Stamina Auto Refill")]
    public class StaminaAutoRefill : TopDownMonoBehaviour
    {
        /// the possible refill modes :
        /// - linear : constant stamina refill at a certain rate per second
        /// - bursts : periodic bursts of stamina
        public enum RefillModes { Linear, Bursts, CycleTurnoverConstant, CycleTurnoverFraction, CycleTurnoverFractionOfMissing }

        [Header("Mode")]
        /// the selected refill mode 
        [Tooltip("the selected refill mode ")]
        public RefillModes RefillMode;

        [Header("Cooldown")]
        /// how much time, in seconds, should pass before the refill kicks in
        [Tooltip("how much time, in seconds, should pass before the refill kicks in")]
        public float CooldownAfterExpenditure = 1f;

        [Header("Refill Settings")]
        /// if this is true, stamina will refill itself when not at full staina
        [Tooltip("if this is true, stamina will refill itself when not at full stamina")]
        public bool RefillStamina = true;
        /// the amount of stamina per second to restore when in linear mode
        [MMEnumCondition("RefillMode", (int)RefillModes.Linear)]
        [Tooltip("the amount of stamina per second to restore when in linear mode")]
        public float StaminaPerSecond;
        /// the amount of stamina to restore per burst when in burst mode
        [MMEnumCondition("RefillMode", (int)RefillModes.Bursts)]
        [Tooltip("the amount of stamina to restore per burst when in burst mode")]
        public float StaminaPerBurst = 5;
        /// the duration between two health stamina, in seconds
        [MMEnumCondition("RefillMode", (int)RefillModes.Bursts)]
        [Tooltip("the duration between two stamina bursts, in seconds")]
        public float DurationBetweenBursts = 2f;
        /// the amount of stamina to restore at cycle turnover when in cycle turnover constant mode
        [MMEnumCondition("RefillMode", (int)RefillModes.CycleTurnoverConstant)]
        [Tooltip("the amount of stamina to restore at cycle turnover when in cycle turnover constant mode")]
        public float StaminaPerTurnover = 25f;
        /// the portion of stamina to restore at cycle turnover when in cycle turnover fraction mode
        [MMEnumCondition("RefillMode", (int)RefillModes.CycleTurnoverFraction)]
        [Tooltip("the portion of stamina to restore at cycle turnover when in cycle turnover fraction mode")]
        [Range(0f, 1f)] public float StaminaFractionPerTurnover = 0.5f;
        /// the fraction of missing stamina to restore at cycle turnover when in cycle turnover fraction of empty mode
        [MMEnumCondition("RefillMode", (int)RefillModes.CycleTurnoverFractionOfMissing)]
        [Tooltip("the fraction of missing stamina to restore at cycle turnover when in cycle turnover fraction of empty mode")]
        [Range(0f, 1f)] public float StaminaFractionOfMissingPerTurnover = 0.5f;

        protected Stamina _stamina;
        protected CycleManager _cycleManager;
        protected float _lastExpenditureTime = 0f;
        protected float _staminaToGive = 0f;
        protected float _lastBurstTimestamp;

        /// <summary>
        /// On Awake we do our init
        /// </summary>
        protected virtual void Awake()
        {
            Initialization();
        }

        /// <summary>
        /// On init we grab our Stamina component
        /// </summary>
        protected virtual void Initialization()
        {
            _stamina = this.gameObject.GetComponent<Stamina>();
            _cycleManager = GameObject.FindObjectOfType<CycleManager>();
        }

        /// <summary>
        /// On Update we refill
        /// </summary>
        protected virtual void Update()
        {
            ProcessRefillStamina();
        }

        protected virtual void OnCycleTurnover(int _nextCycle)
        {
            ProcessRefillStaminaAtCycleTurnover();
        }

        /// <summary>
        /// Tests if a refill is needed and processes it
        /// </summary>
        protected virtual void ProcessRefillStamina()
        {
            if (!RefillStamina)
            {
                return;
            }

            if (Time.time - _lastExpenditureTime < CooldownAfterExpenditure)
            {
                return;
            }

            if (_stamina.CurrentStamina < _stamina.MaximumStamina)
            {
                switch (RefillMode)
                {
                    case RefillModes.Bursts:
                        if (Time.time - _lastBurstTimestamp > DurationBetweenBursts)
                        {
                            _stamina.ReceiveStamina(StaminaPerBurst, this.gameObject);
                            _lastBurstTimestamp = Time.time;
                        }
                        break;

                    case RefillModes.Linear:
                        _staminaToGive += StaminaPerSecond * Time.deltaTime;
                        if (_staminaToGive > 1f)
                        {
                            float givenStamina = _staminaToGive;
                            _staminaToGive -= givenStamina;
                            _stamina.ReceiveStamina(givenStamina, this.gameObject);
                        }
                        break;
                    default:
                        {
                            // All other cases are dealt with in event handlers
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Tests if a refill is needed during a cycle turnover and processes it
        /// </summary>
        protected virtual void ProcessRefillStaminaAtCycleTurnover()
        {
            if (!RefillStamina)
            {
                return;
            }

            if (_stamina.CurrentStamina < _stamina.MaximumStamina)
            {
                switch (RefillMode)
                {
                    case RefillModes.CycleTurnoverConstant:
                        {
                            _stamina.ReceiveStamina(StaminaPerTurnover, this.gameObject);
                            break;
                        }
                    case RefillModes.CycleTurnoverFraction:
                        {
                            float staminaToGive = _stamina.MaximumStamina * StaminaFractionPerTurnover;
                            _stamina.ReceiveStamina(staminaToGive, this.gameObject);
                            break;
                        }
                    case RefillModes.CycleTurnoverFractionOfMissing:
                        {
                            float missingStamina = _stamina.MaximumStamina - _stamina.CurrentStamina;
                            float staminaToGive = missingStamina * StaminaFractionOfMissingPerTurnover;
                            _stamina.ReceiveStamina(staminaToGive, this.gameObject);
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
        /// On expenditure we store our time
        /// </summary>
        public virtual void OnExpenditure()
        {
            _lastExpenditureTime = Time.time;
        }

        /// <summary>
        /// On enable we start listening for expenditure
        /// </summary>
        protected virtual void OnEnable()
        {
            _stamina.OnExpenditure += OnExpenditure;
            _cycleManager.AddCycleTurnoverListener(OnCycleTurnover);
        }

        /// <summary>
        /// On disable we stop listening for hits
        /// </summary>
        protected virtual void OnDisable()
        {
            _stamina.OnExpenditure -= OnExpenditure;
            _cycleManager.RemoveCycleTurnoverListener(OnCycleTurnover);
        }
    }
}