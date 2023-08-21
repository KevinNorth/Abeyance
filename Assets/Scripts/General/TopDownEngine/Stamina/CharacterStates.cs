
using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

namespace KitTraden.Abeyance.TopDownEngine.Characters.Stamina
{
    public enum MMStaminaCycleEventTypes { Exhausted, Recover }

    public struct MMStaminaCycleEvent
    {
        public Stamina AffectedStamina;
        public MMStaminaCycleEventTypes MMStaminaCycleEventTypes;

        public MMStaminaCycleEvent(Stamina affectedStamina, MMStaminaCycleEventTypes stmainaCycleEventType)
        {
            AffectedStamina = affectedStamina;
            MMStaminaCycleEventTypes = stmainaCycleEventType;
        }

        static MMStaminaCycleEvent e;
        public static void Trigger(Stamina affectedStamina, MMStaminaCycleEventTypes staminaCycleEventType)
        {
            e.AffectedStamina = affectedStamina;
            e.MMStaminaCycleEventTypes = staminaCycleEventType;
            MMEventManager.TriggerEvent(e);
        }
    }

    /// <summary>
    /// An event fired when something spends stamina
    /// </summary>
    public struct MMStaminaExpendedEvent
    {
        public Stamina AffectedStamina;
        public CharacterAbility Ability;
        public float CurrentStamina;
        public float StaminaExpended;
        public float PreviousStamina;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoreMountains.TopDownEngine.MMDamageTakenEvent"/> struct.
        /// </summary>
        /// <param name="affectedStamina">Affected Stamina.</param>
        /// <param name="ability">Ability.</param>
        /// <param name="currentStamina">Current Stamina.</param>
        /// <param name="staminaExpended">Stamina expended.</param>
        /// <param name="previousStamina">Previous stamina.</param>
        public MMStaminaExpendedEvent(Stamina affectedStamina, CharacterAbility ability, float currentStamina, float staminaExpended, float previousStamina)
        {
            AffectedStamina = affectedStamina;
            Ability = ability;
            CurrentStamina = currentStamina;
            StaminaExpended = staminaExpended;
            PreviousStamina = previousStamina;
        }

        static MMStaminaExpendedEvent e;
        public static void Trigger(Stamina affectedStamina, CharacterAbility ability, float currentStamina, float staminaExpended, float previousStamina)
        {
            e.AffectedStamina = affectedStamina;
            e.Ability = ability;
            e.CurrentStamina = currentStamina;
            e.StaminaExpended = staminaExpended;
            e.PreviousStamina = previousStamina;
            MMEventManager.TriggerEvent(e);
        }
    }
}