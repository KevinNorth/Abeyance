using KitTraden.Abeyance.TopDownEngine;
using Slate;
using UnityEngine;

namespace KitTraden.Abeyance.Core.Cutscenes
{
    public class BossPhaseChangeCutscenePlayer : MonoBehaviour
    {
        public MultiLayerHealthBar BossHealthBar;
        public Cutscene[] PhaseTransitionCutscenes;
        public Cutscene BossDeathCutscene;
        public bool DisableOnBossDeath = true;

        private int phasesTriggered = 0;

        void OnEnable()
        {
            if (BossHealthBar != null)
            {
                BossHealthBar.TargetProgressBar.OnBarMovementChangeLayerDecreasing.AddListener(OnPhaseChange);
                BossHealthBar.TargetHealth.OnDeath += OnDeath;
            }
        }

        void OnDisable()
        {
            if (BossHealthBar != null)
            {
                BossHealthBar.TargetProgressBar.OnBarMovementChangeLayerDecreasing.RemoveListener(OnPhaseChange);
                BossHealthBar.TargetHealth.OnDeath -= OnDeath;
            }
        }

        private void OnPhaseChange()
        {
            if (PhaseTransitionCutscenes.Length == 0)
            {
                return;
            }

            if (BossHealthBar.TargetHealth.CurrentHealth > 0)
            {
                int cutsceneToPlay = Mathf.Clamp(phasesTriggered, 0, PhaseTransitionCutscenes.Length - 1);
                PhaseTransitionCutscenes[cutsceneToPlay]?.Play();
                phasesTriggered += 1;
            }
        }

        private void OnDeath()
        {
            BossDeathCutscene?.Play();
            enabled = !DisableOnBossDeath;
        }
    }
}