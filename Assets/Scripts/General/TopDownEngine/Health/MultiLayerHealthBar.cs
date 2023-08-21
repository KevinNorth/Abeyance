using System;
using UnityEngine;
using MoreMountains.Tools;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MoreMountains.TopDownEngine;
using KitTraden.Abeyance.UI.HUD;

namespace KitTraden.Abeyance.TopDownEngine
{
    /// <summary>
    /// Add this component to an object and it will show a healthbar above it
    /// You can either use a prefab for it, or have the component draw one at the start
    /// </summary>
    [AddComponentMenu("More Mountains/Tools/GUI/MultiLayerHealthBar")]
    public class MultiLayerHealthBar : MonoBehaviour, MMEventListener<HealthChangeEvent>
    {
        /// the possible timescales the bar can work on
        public enum TimeScales { UnscaledTime, Time }

        [MMInformation("Add this component to an object and it'll control a MultiLayerHealthBar to reflect its health level in real time.", MoreMountains.Tools.MMInformationAttribute.InformationType.Info, false)]
        /// defines whether the bar will work on scaled or unscaled time (whether or not it'll keep moving if time is slowed down for example)
        [Tooltip("defines whether the bar will work on scaled or unscaled time (whether or not it'll keep moving if time is slowed down for example)")]
        public TimeScales TimeScale = TimeScales.UnscaledTime;

        [Header("MultiLayerHealthBar")]
        /// the MultiLayerHealthBar this health bar should update 
        [Tooltip("the MultiLayerHealthBar this health bar should update")]
        public MultiLayerProgressBar TargetProgressBar;
        [Tooltip("the Health to reflect")]
        public Health TargetHealth;

        /// the mode the bar should follow the target in
        [Tooltip("the mode the bar should follow the target in")]
        public MMFollowTarget.UpdateModes FollowTargetMode = MMFollowTarget.UpdateModes.LateUpdate;

        [Header("Death")]
        /// a gameobject (usually a particle system) to instantiate when the healthbar reaches zero
        [Tooltip("a gameobject (usually a particle system) to instantiate when the healthbar reaches zero")]
        public GameObject InstantiatedOnDeath;

        void OnEnable()
        {
            this.MMEventStartListening<HealthChangeEvent>();
        }

        void OnDisable()
        {
            this.MMEventStopListening<HealthChangeEvent>();
        }

        public void OnMMEvent(HealthChangeEvent eventType)
        {
            UpdateBar();
        }

        /// <summary>
        /// Updates the bar
        /// </summary>
        public virtual void UpdateBar()
        {
            if (TargetProgressBar != null)
            {
                TargetProgressBar.UpdateBar(TargetHealth.CurrentHealth, 0f, TargetHealth.MaximumHealth);
            }
        }
    }
}