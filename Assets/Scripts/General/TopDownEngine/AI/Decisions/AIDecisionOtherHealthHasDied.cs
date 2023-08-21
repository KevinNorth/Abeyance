using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;

namespace KitTraden.Abeyance.TopDownEngine.Characters.AI.Decisions
{
    [AddComponentMenu("TopDown Engine/Character/AI/Actions/AIDecisionOtherHealthHasDied")]
    public class AIDecisionOtherHealthHasDied : AIDecision
    {
        [SerializeField] MoreMountains.TopDownEngine.Health HealthToMonitor;
        protected bool isDead = false;

        public override void Initialization()
        {
            base.Initialization();
            isDead = HealthToMonitor == null || HealthToMonitor.CurrentHealth <= 0;
        }

        public override bool Decide()
        {
            return isDead;
        }

        public void OnEnable()
        {
            HealthToMonitor.OnDeath += OnDeath;
        }

        public void OnDisable()
        {
            HealthToMonitor.OnDeath -= OnDeath;
        }

        public void OnDeath()
        {
            isDead = true;
        }
    }
}