using UnityEngine;
using SonicBloom.Koreo;
using MoreMountains.Feedbacks;
using KitTraden.Abeyance.Core.Cycles;

namespace KitTraden.Abeyance.General.Feedbacks
{
    public class KoreographerFeedbackTrigger : MonoBehaviour
    {
        public MMFeedbacks Feedbacks;
        public Koreographer Koreographer;
        public string TriggerEventID;
        public CycleManager CycleManager;
        public bool SkipWhenCycleManagerIsPaused = true;

        private void Start()
        {
            Initialize();
        }

        private void OnEnable()
        {
            Initialize();
            Koreographer.RegisterForEvents(TriggerEventID, OnKoreographyEvent);
        }

        private void OnDisable()
        {
            Koreographer.UnregisterForEvents(TriggerEventID, OnKoreographyEvent);
        }

        private void Initialize()
        {
            if (Koreographer == null)
            {
                Koreographer = SonicBloom.Koreo.Koreographer.Instance;
            }

            if (CycleManager == null)
            {
                CycleManager = GameObject.FindObjectOfType<CycleManager>();
            }
        }

        private void OnKoreographyEvent(KoreographyEvent _koreoEvent)
        {
            if (SkipWhenCycleManagerIsPaused && CycleManager.IsPaused())
            {
                return;
            }

            Feedbacks.PlayFeedbacks();
        }
    }
}