using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using System.Linq;
using MoreMountains.TopDownEngine;

namespace KitTraden.Abeyance.TopDownEngine.Characters.Abilities
{
    public class CharacterAbilityWithStamina : CharacterAbility
    {
        protected Stamina.Stamina _stamina;

        protected override void Initialization()
        {
            base.Initialization();

            _stamina = _character.gameObject.GetComponent<Stamina.Stamina>();
        }

        protected bool CanExpendStamina(float staminaToExpend)
        {
            return _stamina.CanExpendStaminaAmount(staminaToExpend);
        }

        protected void ExpendStamina(float staminaAmount)
        {
            _stamina.Expend(staminaAmount, this);
        }

        /// <summary>
        /// Override this to describe what should happen to this ability when the character respawns
        /// </summary>
        protected virtual void OnStaminaExpenditure()
        {
        }

        /// <summary>
        /// Override this to describe what should happen to this ability when the character respawns
        /// </summary>
        protected virtual void OnExhaustion()
        {

        }

        /// <summary>
        /// Override this to describe what should happen to this ability when the character takes a hit
        /// </summary>
        protected virtual void OnStaminaRecover()
        {

        }

        /// <summary>
		/// On enable, we bind our stamina delegates
		/// </summary>
		protected override void OnEnable()
        {
            base.OnEnable();

            if (_stamina == null)
            {
                _stamina = this.gameObject.GetComponentInParent<Stamina.Stamina>();
            }

            if (_stamina != null)
            {
                _stamina.OnExpenditure += OnStaminaExpenditure;
                _stamina.OnExhaustion += OnExhaustion;
                _stamina.OnRecover += OnStaminaRecover;
            }
        }

        /// <summary>
		/// On disable, we unbind our stamina delegates
		/// </summary>
		protected override void OnDisable()
        {
            base.OnDisable();

            if (_stamina != null)
            {
                _stamina.OnExpenditure -= OnStaminaExpenditure;
                _stamina.OnExhaustion -= OnExhaustion;
                _stamina.OnRecover -= OnStaminaRecover;
            }
        }
    }
}