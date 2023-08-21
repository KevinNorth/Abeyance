using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;

namespace KitTraden.Abeyance.TopDownEngine.Characters.AI.Actions
{
    /// <summary>
    /// Enables other Components when the AI Brain state is entered and disables them when the
    /// AI Brain state is exited. This allows other systems to control a character by setting
    /// up their objects to automatically run when they're enabled, then turning them on and
    /// letting them do their own thing.
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionEnableComponents")]
    public class AIActionEnableComponents : AIAction
    {
        [Tooltip("These components will be enabled while the action is being performed")]
        [SerializeField] List<MonoBehaviour> componentsToEnable;
        [Tooltip("These components will be enabled while the action is NOT being performed")]
        [SerializeField] List<MonoBehaviour> componentsToDisable;

        public override void PerformAction()
        {
            // Do nothing
            // The intent is that the AI behavior will be implemented by whatever components
            // are enabled upon entering the state
        }

        public override void OnEnterState()
        {
            base.OnEnterState();

            foreach (var component in componentsToEnable)
            {
                component.enabled = true;
            }

            foreach (var component in componentsToDisable)
            {
                component.enabled = false;
            }
        }

        public override void OnExitState()
        {
            base.OnExitState();

            foreach (var component in componentsToEnable)
            {
                component.enabled = false;
            }

            foreach (var component in componentsToDisable)
            {
                component.enabled = true;
            }
        }
    }
}