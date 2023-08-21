using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using MoreMountains.TopDownEngine;
using KitTraden.Abeyance.Core.Cycles;

/*
 * Created by copy-pasting TopDown Engine's Health class and making
 * the corresponding changes.
 */

namespace KitTraden.Abeyance.TopDownEngine.Characters.Stamina
{
    /// <summary>
    /// An event triggered every time stamina values change, for other classes to listen to
    /// </summary>
    public struct StaminaChangeEvent
    {
        public Stamina AffectedStamina;
        public float NewStamina;

        public StaminaChangeEvent(Stamina affectedStamina, float newStamina)
        {
            AffectedStamina = affectedStamina;
            NewStamina = newStamina;
        }

        static StaminaChangeEvent e;
        public static void Trigger(Stamina affectedStamina, float newStamina)
        {
            e.AffectedStamina = affectedStamina;
            e.NewStamina = newStamina;
            MMEventManager.TriggerEvent(e);
        }
    }

    /// The different ways a character can handle being exhausted
    public enum ExhaustionBehaviors
    {
        /// The character is never exhausted and can immediately use abilities
        /// again as long as thy have enough stamina. Setting CanOverspendBeforeExhaustion
        /// to true is highly recommended to avoid unlimited stamina usage.
        NoExhaustion,
        /// The character cannot use stamina-consuming abilities for a set time.
        RecoverAfterCooldownTime,
        /// The character cannot use stamina-consuming abilities until their current stamina
        /// is a set fraction of their max stamina.
        RecoverAtRegeneratedStaminaThreshold,
        /// The character cannot use stamina-consuming abilities until their current stamina
        /// is equal to their maximum stamina.
        RecoverAtMaxStamina,
        /// The character cannot use stamina-consuming abilities until the next cycle turnover.
        RecoverAtCycleTurnover
    }

    /// <summary>
    /// This class manages the stamina of an object, pilots its potential stamina bar, handles what happens when it uses stamina,
    /// and what happens when it runs out of stamina.
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/Core/Stamina")]
    public class Stamina : MMMonoBehaviour
    {
        [MMInspectorGroup("Bindings", true, 3)]
        /// the health bar to use to display stamina
        [Tooltip("the health bar to use to display stamina")]
        public MMHealthBar StaminaBar;

        // /// the model to disable (if set so)
        // [Tooltip("the model to disable (if set so)")]
        // public GameObject Model;

        [MMInspectorGroup("Status", true, 29)]

        /// the current stamina of the character
        [MMReadOnly]
        [Tooltip("the current stamina of the character")]
        public float CurrentStamina;
        /// If this is true, this object won't spend or run out of stamina at this time
        [MMReadOnly]
        [Tooltip("If this is true, this object won't spend or run out of stamina at this time")]
        public bool Inexhaustible = false;

        [MMInspectorGroup("Stamina", true, 5)]

        [MMInformation("Add this component to an object and it'll have stamina, which can be used to limit other abilities' usage.", MoreMountains.Tools.MMInformationAttribute.InformationType.Info, false)]
        /// the initial amount of stamina of the object
        [Tooltip("the initial amount of stamina of the object")]
        public float InitialStamina = 10;
        /// the maximum amount of stamina of the object
        [Tooltip("the maximum amount of sta,oma of the object")]
        public float MaximumStamina = 10;
        /// if this is true, stamina values will be reset everytime this character is enabled (usually at the start of a scene)
        [Tooltip("if this is true, stamina values will be reset everytime this character is enabled (usually at the start of a scene)")]
        public bool ResetStaminaOnEnable = true;

        [MMInspectorGroup("Damage", true, 6)]

        [MMInformation("Here you can specify an effect and a sound FX to instantiate when the object uses stamina.", MoreMountains.Tools.MMInformationAttribute.InformationType.Info, false)]
        /// whether or not this Stamina object can be spent 
        [Tooltip("whether or not this Stamina object can expend stamina")]
        public bool ImmuneToExpending = false;
        /// the feedback to play when spending stamina
        [Tooltip("the feedback to play when using stamina")]
        public MMFeedbacks ExpendMMFeedbacks;
        /// if this is true, the stamina expense value will be passed to the MMFeedbacks as its Intensity parameter, letting you trigger more intense feedbacks as stamina usage increases
        [Tooltip("if this is true, the stamina expense value will be passed to the MMFeedbacks as its Intensity parameter, letting you trigger more intense feedbacks as stamina usage increases")]
        public bool FeedbackIsProportionalToExpenditure = false;

        [MMInspectorGroup("Exhaustion", true, 53)]

        [MMInformation("Here you can set an effect to instantiate when the object runs out of stamina, whether the character can go past max stamina once before getting exhausted, and how becoming unexhausted should be handled.", MoreMountains.Tools.MMInformationAttribute.InformationType.Info, false)]
        /// whether the player can overspend stamina once before becoming exhausted
        [Tooltip("whether the player can overspend stamina once before becoming exhausted")]
        public bool CanOverspendBeforeExhaustion = false;
        /// how the character acts when exhausted
        [Tooltip("how the character acts when exhausted")]
        public ExhaustionBehaviors ExhaustionBehavior;
        /// how long recovery takes if ExhaustionBehavior is ExhaustionBehaviors.RecoverAfterCooldownTime
        [Tooltip("how long recovery takes if ExhaustionBehavior is ExhaustionBehaviors.RecoverAfterCooldownTime")]
        [MMEnumCondition("ExhaustionBehavior", (int)ExhaustionBehaviors.RecoverAfterCooldownTime)]
        public float ExhaustionRecoveryTime = 10f;
        /// the threshold fraction to reach if ExhaustionBehavior is ExhaustionBehaviors.RecoverAtRegeneratedStaminaThreshold
        [Tooltip("how long recovery takes if ExhaustionBehavior is ExhaustionBehaviors.RecoverAtRegeneratedStaminaThreshold")]
        [MMEnumCondition("ExhaustionBehavior", (int)ExhaustionBehaviors.RecoverAtRegeneratedStaminaThreshold)]
        [Range(0f, 1f)] public float ExhaustionRecoveryThreshold = 0.5f;
        /// whether or not this object should change layer on exhaustion
        [Tooltip("whether or not this object should change layer on exhaustion")]
        public bool ChangeLayerOnExhaustion = false;
        /// whether or not this object should change layer on exhaustion
        [Tooltip("whether or not this object should change layer on exhaustion")]
        public bool ChangeLayersRecursivelyOnExhaustion = false;
        /// the layer we should move this character to on exhaustion
        [Tooltip("the layer we should move this character to on exhaustion")]
        public MMLayer LayerOnExhaustion;
        /// the feedback to play when dying
        [Tooltip("the feedback to play when exhausting")]
        public MMFeedbacks ExhaustionMMFeedbacks;
        /// the feedback to play when an ability is prevented due to exhaustion or a lack of stamina
        [Tooltip("the feedback to play when an ability is prevented due to exhaustion or a lack of stamina")]
        public MMFeedbacks AbilityPreventedDueToExhaustionFeedbacks;

        /// if this is true, color will be reset on revive
        [Tooltip("if this is true, color will be reset on revive")]
        public bool ResetColorOnRevive = true;
        /// the name of the property on your renderer's shader that defines its color 
        [Tooltip("the name of the property on your renderer's shader that defines its color")]
        [MMCondition("ResetColorOnRevive", true)]
        public string ColorMaterialPropertyName = "_Color";
        /// if this is true, this component will use material property blocks instead of working on an instance of the material.
        [Tooltip("if this is true, this component will use material property blocks instead of working on an instance of the material.")]
        public bool UseMaterialPropertyBlocks = false;

        [MMInspectorGroup("Shared Health and Damage Resistance", true, 12)]
        /// another Stamina component (usually on another character) towards which all stamina will be redirected
        [Tooltip("another Stamina component (usually on another character) towards which all stamina will be redirected")]
        public Stamina MasterStamina;
        /// a DamageResistanceProcessor this Stamina will use to process expenditure when it's received
        [Tooltip("a DamageResistanceProcessor this Stamina will use to process expenditure when it's received")]
        public DamageResistanceProcessor TargetExpenditureResistanceProcessor;

        [MMInspectorGroup("Animator", true, 14)]
        /// the target animator to pass a Death animation parameter to. The Health component will try to auto bind this if left empty
        [Tooltip("the target animator to pass a Death animation parameter to. The Health component will try to auto bind this if left empty")]
        public Animator TargetAnimator;
        /// if this is true, animator logs for the associated animator will be turned off to avoid potential spam
        [Tooltip("if this is true, animator logs for the associated animator will be turned off to avoid potential spam")]
        public bool DisableAnimatorLogs = true;

        public float LastExpenditure { get; set; }
        // public Vector3 LastDamageDirection { get; set; }

        // expenditure delegate
        public delegate void OnExpenditureDelegate();
        public OnExpenditureDelegate OnExpenditure;

        // recover delegate
        public delegate void OnRecoverDelegate();
        public OnRecoverDelegate OnRecover;

        // exhaustion delegate
        public delegate void OnExhaustionDelegate();
        public OnExhaustionDelegate OnExhaustion;

        protected Vector3 _initialPosition;
        protected Renderer _renderer;
        protected Character _character;
        protected CharacterMovement _characterMovement;
        protected TopDownController _controller;
        protected CycleManager _cycleManager;

        protected Collider2D _collider2D;
        protected Collider _collider3D;
        protected CharacterController _characterController;
        protected bool _initialized = false;
        protected Color _initialColor;
        // protected AutoRespawn _autoRespawn;
        protected int _initialLayer;
        protected MaterialPropertyBlock _propertyBlock;
        protected bool _hasColorProperty = false;
        protected float _lastExhaustionTimestamp = 0f;
        protected bool _isExhausted = false;

        protected class InterruptiblesExpenditureOverTimeCoroutine
        {
            public Coroutine ExpenditureOverTimeCoroutine;
            public DamageType ExpenditureOverTimeType;
        }

        protected List<InterruptiblesExpenditureOverTimeCoroutine> _interruptiblesExpenditureOverTimeCoroutines;

        #region Initialization

        /// <summary>
        /// On Start, we initialize our health
        /// </summary>
        protected virtual void Awake()
        {
            Initialization();
            InitializeCurrentStamina();
        }

        /// <summary>
        /// On Start we grab our animator
        /// </summary>
        protected virtual void Start()
        {
            GrabAnimator();
        }

        protected virtual void Update()
        {
            RecoverIfExhaustionHasEnded();
        }

        protected virtual void OnDestroy()
        {
            _cycleManager.RemoveCycleTurnoverListener(OnCycleTurnover);
        }

        protected virtual void OnCycleTurnover(int _nextCycle)
        {
            if (_isExhausted & ExhaustionBehavior == ExhaustionBehaviors.RecoverAtCycleTurnover)
            {
                Recover();
            }
        }

        public bool IsExhausted()
        {
            return _isExhausted;
        }

        public virtual void RecoverIfExhaustionHasEnded()
        {
            if (_isExhausted)
            {
                switch (ExhaustionBehavior)
                {
                    case ExhaustionBehaviors.NoExhaustion:
                        {
                            Recover();
                            break;
                        }
                    case ExhaustionBehaviors.RecoverAfterCooldownTime:
                        {
                            if (Time.time - _lastExhaustionTimestamp > ExhaustionRecoveryTime)
                            {
                                Recover();
                            }
                            break;
                        }
                    case ExhaustionBehaviors.RecoverAtRegeneratedStaminaThreshold:
                        {
                            if (CurrentStamina / MaximumStamina > ExhaustionRecoveryThreshold)
                            {
                                Recover();
                            }
                            break;
                        }
                    case ExhaustionBehaviors.RecoverAtMaxStamina:
                        {

                            if (CurrentStamina == MaximumStamina)
                            {
                                Recover();
                            }
                            break;
                        }
                    default:
                        {
                            // Do nothing and wait for an external source to indicate
                            // that exhaustion has ended
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Grabs useful components, enables expenditure and gets the inital color
        /// </summary>
        public virtual void Initialization()
        {
            _character = this.gameObject.GetComponentInParent<Character>();

            _cycleManager = GameObject.FindObjectOfType<CycleManager>();
            _cycleManager.AddCycleTurnoverListener(OnCycleTurnover);

            if (gameObject.GetComponentInParent<Renderer>() != null)
            {
                _renderer = GetComponentInParent<Renderer>();
            }
            if (_character != null)
            {
                _characterMovement = _character.FindAbility<CharacterMovement>();
                if (_character.CharacterModel != null)
                {
                    if (_character.CharacterModel.GetComponentInChildren<Renderer>() != null)
                    {
                        _renderer = _character.CharacterModel.GetComponentInChildren<Renderer>();
                    }
                }
            }
            if (_renderer != null)
            {
                if (UseMaterialPropertyBlocks && (_propertyBlock == null))
                {
                    _propertyBlock = new MaterialPropertyBlock();
                }

                if (ResetColorOnRevive)
                {
                    if (UseMaterialPropertyBlocks)
                    {
                        if (_renderer.sharedMaterial.HasProperty(ColorMaterialPropertyName))
                        {
                            _hasColorProperty = true;
                            _initialColor = _renderer.sharedMaterial.GetColor(ColorMaterialPropertyName);
                        }
                    }
                    else
                    {
                        if (_renderer.material.HasProperty(ColorMaterialPropertyName))
                        {
                            _hasColorProperty = true;
                            _initialColor = _renderer.material.GetColor(ColorMaterialPropertyName);
                        }
                    }
                }
            }

            _interruptiblesExpenditureOverTimeCoroutines = new List<InterruptiblesExpenditureOverTimeCoroutine>();
            _initialLayer = gameObject.layer;

            // _autoRespawn = this.gameObject.GetComponentInParent<AutoRespawn>();
            _controller = this.gameObject.GetComponentInParent<TopDownController>();
            _characterController = this.gameObject.GetComponentInParent<CharacterController>();
            _collider2D = this.gameObject.GetComponentInParent<Collider2D>();
            _collider3D = this.gameObject.GetComponentInParent<Collider>();

            ExpendMMFeedbacks?.Initialization(this.gameObject);
            ExhaustionMMFeedbacks?.Initialization(this.gameObject);

            StoreInitialPosition();
            _initialized = true;

            ExpenditureEnabled();
        }

        /// <summary>
        /// Grabs the target animator
        /// </summary>
        protected virtual void GrabAnimator()
        {
            if (TargetAnimator == null)
            {
                BindAnimator();
            }

            if ((TargetAnimator != null) && DisableAnimatorLogs)
            {
                TargetAnimator.logWarnings = false;
            }
        }

        /// <summary>
        /// Finds and binds an animator if possible
        /// </summary>
        protected virtual void BindAnimator()
        {
            if (_character != null)
            {
                if (_character.CharacterAnimator != null)
                {
                    TargetAnimator = _character.CharacterAnimator;
                }
                else
                {
                    TargetAnimator = GetComponent<Animator>();
                }
            }
            else
            {
                TargetAnimator = GetComponent<Animator>();
            }
        }

        /// <summary>
        /// Stores the initial position for further use
        /// </summary>
        public virtual void StoreInitialPosition()
        {
            _initialPosition = this.transform.position;
        }

        /// <summary>
        /// Initializes health to either initial or current values
        /// </summary>
        public virtual void InitializeCurrentStamina()
        {
            if (MasterStamina == null)
            {
                SetStamina(InitialStamina);
            }
            else
            {
                CurrentStamina = MasterStamina.CurrentStamina;
            }
        }

        /// <summary>
        /// When the object is enabled (on respawn for example), we restore its initial stamina levels
        /// </summary>
        protected virtual void OnEnable()
        {
            if (ResetStaminaOnEnable)
            {
                InitializeCurrentStamina();
            }
            ExpenditureEnabled();
        }

        /// <summary>
        /// On Disable, we prevent any delayed destruction from running
        /// </summary>
        protected virtual void OnDisable()
        {
            CancelInvoke();
        }

        #endregion

        /// <summary>
        /// Returns true if this Stamina component can be damaged this frame, and false otherwise
        /// </summary>
        /// <returns></returns>
        public virtual bool CanExpendStaminaThisFrame()
        {
            // if the object is inexhaustible, we do nothing and exit
            if (Inexhaustible || ImmuneToExpending)
            {
                return false;
            }

            if (!this.enabled)
            {
                return false;
            }

            // if we're already below zero, we do nothing and exit
            if ((CurrentStamina <= 0) && (InitialStamina != 0))
            {
                return false;
            }

            if (_isExhausted)
            {
                return false;
            }

            return true;
        }

        public virtual bool CanExpendStaminaAmount(float expenditure)
        {
            if (_isExhausted)
            {
                return false;
            }

            return CanOverspendBeforeExhaustion || (expenditure <= CurrentStamina);
        }

        /// <summary>
        /// Called when the object expends stamina
        /// </summary>
        /// <param name="expenditure">The amount of stamina points that will get lost.</param>
        /// <param name="ability">The ability that caused the expense.</param>
        /// <param name="typedDamage">The damage types to use for the expense. Resistances and weaknesses are applied analagously to damage types and health.</param>
        public virtual void Expend(float expenditure, CharacterAbility ability, List<TypedDamage> typedDamages = null)
        {
            if (!CanExpendStaminaThisFrame())
            {
                return;
            }

            expenditure = ComputeExpenditureOutput(expenditure, typedDamages, true);

            if (!CanExpendStaminaAmount(expenditure))
            {
                AbilityPreventedDueToExhaustionFeedbacks?.PlayFeedbacks(this.transform.position);
            }

            // we decrease the character's health by the damage
            float previousStamina = CurrentStamina;
            if (MasterStamina != null)
            {
                previousStamina = MasterStamina.CurrentStamina;
                MasterStamina.SetStamina(MasterStamina.CurrentStamina - expenditure);
            }
            else
            {
                SetStamina(CurrentStamina - expenditure);
            }

            LastExpenditure = expenditure;

            // we trigger a damage taken event
            MMStaminaExpendedEvent.Trigger(this, ability, CurrentStamina, expenditure, previousStamina);

            // we update our animator
            if (TargetAnimator != null)
            {
                TargetAnimator.SetTrigger("StaminaExpended");
            }

            // we play our feedback
            if (FeedbackIsProportionalToExpenditure)
            {
                ExpendMMFeedbacks?.PlayFeedbacks(this.transform.position, expenditure);
            }
            else
            {
                ExpendMMFeedbacks?.PlayFeedbacks(this.transform.position);
            }

            // we update the health bar
            UpdateStaminaBar(true);

            // we process any condition state change
            ComputeCharacterConditionStateChanges(typedDamages);
            ComputeCharacterMovementMultipliers(typedDamages);

            // if stamina has reached zero we set its health to zero (useful for the healthbar)
            if (MasterStamina != null)
            {
                if (MasterStamina.CurrentStamina <= 0)
                {
                    MasterStamina.CurrentStamina = 0;
                    MasterStamina.Exhaust();
                }
            }
            else
            {
                if (CurrentStamina <= 0)
                {
                    CurrentStamina = 0;
                    Exhaust();
                }

            }
        }

        /// <summary>
        /// Interrupts all expenditure over time, regardless of type
        /// </summary>
        public virtual void InterruptAllExpenditureOverTime()
        {
            foreach (InterruptiblesExpenditureOverTimeCoroutine coroutine in _interruptiblesExpenditureOverTimeCoroutines)
            {
                StopCoroutine(coroutine.ExpenditureOverTimeCoroutine);
            }
        }

        /// <summary>
        /// Interrupts all expenditure over time of the specified type
        /// </summary>
        /// <param name="damageType"></param>
        public virtual void InterruptAllExpenditureOverTimeOfType(DamageType damageType)
        {
            foreach (InterruptiblesExpenditureOverTimeCoroutine coroutine in _interruptiblesExpenditureOverTimeCoroutines)
            {
                if (coroutine.ExpenditureOverTimeType == damageType)
                {
                    StopCoroutine(coroutine.ExpenditureOverTimeCoroutine);
                }
            }
            TargetExpenditureResistanceProcessor?.InterruptDamageOverTime(damageType);
        }

        /// <summary>
        /// Applies expenditure over time, for the specified amount of repeats (which includes the first application of expenditure, makes it easier to do quick maths in the inspector, and at the specified interval).
        /// Optionally you can decide that your expenditure is interruptible, in which case, calling InterruptAllExpenditureOverTime() will stop these from being applied, useful to cure poison for example.
        /// </summary>
        /// <param name="expense"></param>
        /// <param name="ability"></param>
        /// <param name="typedDamages"></param>
        /// <param name="amountOfRepeats"></param>
        /// <param name="durationBetweenRepeats"></param>
        /// <param name="interruptible"></param>
        public virtual void ExpenditureOverTime(float expense, CharacterAbility ability, List<TypedDamage> typedDamages = null,
            int amountOfRepeats = 0, float durationBetweenRepeats = 1f, bool interruptible = true, DamageType damageType = null)
        {
            if (ComputeExpenditureOutput(expense, typedDamages, false) == 0)
            {
                return;
            }

            InterruptiblesExpenditureOverTimeCoroutine expenditureOverTime = new InterruptiblesExpenditureOverTimeCoroutine();
            expenditureOverTime.ExpenditureOverTimeType = damageType;
            expenditureOverTime.ExpenditureOverTimeCoroutine = StartCoroutine(ExpenditureOverTimeCo(expense, ability,
                typedDamages, amountOfRepeats, durationBetweenRepeats, interruptible));

            if (interruptible)
            {
                _interruptiblesExpenditureOverTimeCoroutines.Add(expenditureOverTime);
            }
        }

        /// <summary>
        /// A coroutine used to apply damage over time
        /// </summary>
        /// <param name="expense"></param>
        /// <param name="ability"></param>
        /// <param name="typedDamages"></param>
        /// <param name="amountOfRepeats"></param>
        /// <param name="durationBetweenRepeats"></param>
        /// <param name="interruptible"></param>
        /// <returns></returns>
        protected virtual IEnumerator ExpenditureOverTimeCo(float expense, CharacterAbility ability, List<TypedDamage> typedDamages = null,
            int amountOfRepeats = 0, float durationBetweenRepeats = 1f, bool interruptible = true)
        {
            for (int i = 0; i < amountOfRepeats; i++)
            {
                Expend(expense, ability, typedDamages);
                yield return MMCoroutine.WaitFor(durationBetweenRepeats);
            }
        }

        /// <summary>
        /// Returns the amount of stamina that should be spent after processing potential resistances
        /// </summary>
        /// <param name="expense"></param>
        /// <returns></returns>
        public virtual float ComputeExpenditureOutput(float expense, List<TypedDamage> typedDamages = null, bool expenseApplied = false)
        {
            if (Inexhaustible || ImmuneToExpending)
            {
                return 0;
            }

            float totalDamage = 0f;
            // we process our expense through our potential resistances
            if (TargetExpenditureResistanceProcessor != null)
            {
                if (TargetExpenditureResistanceProcessor.isActiveAndEnabled)
                {
                    totalDamage = TargetExpenditureResistanceProcessor.ProcessDamage(expense, typedDamages, expenseApplied);
                }
            }
            else
            {
                totalDamage = expense;
                if (typedDamages != null)
                {
                    foreach (TypedDamage typedDamage in typedDamages)
                    {
                        totalDamage += typedDamage.DamageCaused;
                    }
                }
            }
            return totalDamage;
        }

        /// <summary>
        /// Goes through resistances and applies condition state changes if needed
        /// </summary>
        /// <param name="typedDamages"></param>
        protected virtual void ComputeCharacterConditionStateChanges(List<TypedDamage> typedDamages)
        {
            if ((typedDamages == null) || (_character == null))
            {
                return;
            }

            foreach (TypedDamage typedDamage in typedDamages)
            {
                if (typedDamage.ForceCharacterCondition)
                {
                    if (TargetExpenditureResistanceProcessor != null)
                    {
                        if (TargetExpenditureResistanceProcessor.isActiveAndEnabled)
                        {
                            bool checkResistance =
                                TargetExpenditureResistanceProcessor.CheckPreventCharacterConditionChange(typedDamage.AssociatedDamageType);
                            if (checkResistance)
                            {
                                continue;
                            }
                        }
                    }
                    _character.ChangeCharacterConditionTemporarily(typedDamage.ForcedCondition, typedDamage.ForcedConditionDuration, typedDamage.ResetControllerForces, typedDamage.DisableGravity);
                }
            }

        }

        /// <summary>
        /// Goes through the resistance list and applies movement multipliers if needed
        /// </summary>
        /// <param name="typedDamages"></param>
        protected virtual void ComputeCharacterMovementMultipliers(List<TypedDamage> typedDamages)
        {
            if ((typedDamages == null) || (_character == null))
            {
                return;
            }

            foreach (TypedDamage typedDamage in typedDamages)
            {
                if (typedDamage.ApplyMovementMultiplier)
                {
                    if (TargetExpenditureResistanceProcessor != null)
                    {
                        if (TargetExpenditureResistanceProcessor.isActiveAndEnabled)
                        {
                            bool checkResistance =
                                TargetExpenditureResistanceProcessor.CheckPreventMovementModifier(typedDamage.AssociatedDamageType);
                            if (checkResistance)
                            {
                                continue;
                            }
                        }
                    }

                    _characterMovement?.ApplyMovementMultiplier(typedDamage.MovementMultiplier,
                        typedDamage.MovementMultiplierDuration);
                }
            }

        }

        /// <summary>
        /// Exhausts the character, instantiates exhaustion effects, etc
        /// </summary>
        public virtual void Exhaust()
        {
            if (ImmuneToExpending)
            {
                return;
            }

            SetStamina(0);
            _lastExhaustionTimestamp = Time.time;

            OnExhaustion?.Invoke();
            MMStaminaCycleEvent.Trigger(this, MMStaminaCycleEventTypes.Exhausted);

            ExhaustionMMFeedbacks?.PlayFeedbacks(this.transform.position);

            if (TargetAnimator != null)
            {
                TargetAnimator.SetTrigger("Exhaustion");
            }

            if (ExhaustionBehavior == ExhaustionBehaviors.NoExhaustion)
            {
                return;
            }

            _isExhausted = true;

            if (TargetAnimator != null)
            {
                TargetAnimator.SetBool("Exhausted", true);
            }

            if (ChangeLayerOnExhaustion)
            {
                gameObject.layer = LayerOnExhaustion.LayerIndex;
                if (ChangeLayersRecursivelyOnExhaustion)
                {
                    this.transform.ChangeLayersRecursively(LayerOnExhaustion.LayerIndex);
                }
            }
        }

        /// <summary>
        /// Revive this object.
        /// </summary>
        public virtual void Recover()
        {
            _isExhausted = false;

            if (!_initialized)
            {
                return;
            }

            if (TargetAnimator != null)
            {
                TargetAnimator.SetBool("Exhausted", false);
            }

            if (ChangeLayerOnExhaustion)
            {
                gameObject.layer = _initialLayer;
                if (ChangeLayersRecursivelyOnExhaustion)
                {
                    this.transform.ChangeLayersRecursively(_initialLayer);
                }
            }
            if (ResetColorOnRevive && (_renderer != null))
            {
                if (UseMaterialPropertyBlocks)
                {
                    _renderer.GetPropertyBlock(_propertyBlock);
                    _propertyBlock.SetColor(ColorMaterialPropertyName, _initialColor);
                    _renderer.SetPropertyBlock(_propertyBlock);
                }
                else
                {
                    _renderer.material.SetColor(ColorMaterialPropertyName, _initialColor);
                }
            }

            OnRecover?.Invoke();
            MMStaminaCycleEvent.Trigger(this, MMStaminaCycleEventTypes.Recover);
        }

        #region HealthManipulationAPIs


        /// <summary>
        /// Sets the current stmaina to the specified new value, and updates the stamina bar
        /// </summary>
        /// <param name="newValue"></param>
        public virtual void SetStamina(float newValue)
        {
            CurrentStamina = newValue;
            UpdateStaminaBar(false);
            StaminaChangeEvent.Trigger(this, newValue);
        }

        /// <summary>
        /// Called when the character gets stamina (from a stimpack for example)
        /// </summary>
        /// <param name="stamina">The stamina the character gets.</param>
        /// <param name="instigator">The thing that gives the character sta,oma.</param>
        public virtual void ReceiveStamina(float stamina, GameObject instigator)
        {
            // this function adds health to the character's Health and prevents it to go above MaxHealth.
            if (MasterStamina != null)
            {
                MasterStamina.SetStamina(Mathf.Min(CurrentStamina + stamina, MaximumStamina));
            }
            else
            {
                SetStamina(Mathf.Min(CurrentStamina + stamina, MaximumStamina));
            }
            UpdateStaminaBar(true);
        }

        /// <summary>
        /// Resets the character's stamina to its max value
        /// </summary>
        public virtual void ResetStaminaToMaxStamina()
        {
            SetStamina(MaximumStamina);
        }

        /// <summary>
        /// Forces a refresh of the character's stamina bar
        /// </summary>
        public virtual void UpdateStaminaBar(bool show)
        {
            if (StaminaBar != null)
            {
                StaminaBar.UpdateBar(CurrentStamina, 0f, MaximumStamina, show);
            }

            if (MasterStamina == null)
            {
                if (_character != null)
                {
                    if (_character.CharacterType == Character.CharacterTypes.Player)
                    {
                        // We update the health bar
                        if (GUIManager.HasInstance)
                        {
                            GUIManager.Instance.UpdateStaminaBar(CurrentStamina, 0f, MaximumStamina, _character.PlayerID);
                        }
                    }
                }
            }
        }

        #endregion

        #region DamageDisablingAPIs

        /// <summary>
        /// Prevents the character from expending any stamina, even when
        /// using abilities that normally do so.
        /// </summary>
        public virtual void ExpenditureDisabled()
        {
            Inexhaustible = true;
        }

        /// <summary>
        /// Allows the character to expend stamina
        /// </summary>
        public virtual void ExpenditureEnabled()
        {
            Inexhaustible = false;
        }

        /// <summary>
        /// makes the character able to expend stamina again after the specified delay
        /// </summary>
        /// <returns>The layer collision.</returns>
        public virtual IEnumerator ExpenditureEnabled(float delay)
        {
            yield return new WaitForSeconds(delay);
            Inexhaustible = false;
        }

        #endregion
    }
}