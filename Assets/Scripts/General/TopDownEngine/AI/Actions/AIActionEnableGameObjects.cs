using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;

namespace KitTraden.Abeyance.TopDownEngine.Characters.AI.Actions
{
    /// <summary>
    /// Enables other GameObjects when the AI Brain state is entered and disables them when the
    /// AI Brain state is exited. This allows other systems to control a character by setting
    /// up their objects to automatically run when they're enabled, then turning them on and
    /// letting them do their own thing.
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/AI/Actions/AIActionEnableGameObjects")]
    public class AIActionEnableGameObjects : AIAction
    {
        [Tooltip("These objects will be enabled while the action is being performed")]
        [SerializeField] List<GameObject> gameObjectsToEnable;
        [Tooltip("These objects will be enabled while the action is NOT being performed")]
        [SerializeField] List<GameObject> gameObjectsToDisable;

        public override void PerformAction()
        {
            // Do nothing
            // The intent is that the AI behavior will be implemented by whatever game objects
            // are enabled upon entering the state
        }

        public override void OnEnterState()
        {
            base.OnEnterState();

            foreach (var gameObject in gameObjectsToEnable)
            {
                gameObject.SetActive(true);
            }

            foreach (var gameObject in gameObjectsToDisable)
            {
                gameObject.SetActive(false);
            }
        }

        public override void OnExitState()
        {
            base.OnExitState();

            foreach (var gameObject in gameObjectsToEnable)
            {
                gameObject.SetActive(false);
            }

            foreach (var gameObject in gameObjectsToDisable)
            {
                gameObject.SetActive(true);
            }
        }
    }
}