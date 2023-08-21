using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using System.Linq;
using MoreMountains.TopDownEngine;

namespace KitTraden.Abeyance.TopDownEngine.Characters.Abilities
{
    public enum StaminaExpenseTypes
    {
        ALL_AT_ONCE,
        PER_SECOND
    }

    public abstract class CharacterAbilityWithStaminaAdded : CharacterAbilityWithStamina
    {
        public StaminaExpenseTypes StaminaExpenseType;
        public float staminaExpense;

        protected bool _isAbilityActive = false;

        public bool IsStaminaReadyForAbility()
        {
            if (_stamina.IsExhausted())
            {
                return false;
            }

            switch (StaminaExpenseType)
            {
                case StaminaExpenseTypes.ALL_AT_ONCE:
                    {
                        return _stamina.CanExpendStaminaAmount(staminaExpense);
                    }
                case StaminaExpenseTypes.PER_SECOND:
                    {
                        return true;
                    }
                default:
                    {
                        throw new System.Exception($"Cannot handle StaminaExpenseType of {StaminaExpenseType}");
                    }
            }
        }

        public bool TryExpendStamina()
        {
            if (!IsStaminaReadyForAbility())
            {
                return false;
            }

            switch (StaminaExpenseType)
            {
                case StaminaExpenseTypes.ALL_AT_ONCE:
                    {
                        _stamina.Expend(staminaExpense, this);
                        return true;
                    }
                case StaminaExpenseTypes.PER_SECOND:
                    {
                        return true;
                    }
                default:
                    {
                        throw new System.Exception($"Cannot handle StaminaExpenseType of {StaminaExpenseType}");
                    }
            }
        }

        public override void EarlyProcessAbility()
        {
            base.EarlyProcessAbility();

            if (_isAbilityActive && StaminaExpenseType == StaminaExpenseTypes.PER_SECOND)
            {
                _stamina.Expend(staminaExpense * Time.deltaTime, this);
            }

        }
    }
}