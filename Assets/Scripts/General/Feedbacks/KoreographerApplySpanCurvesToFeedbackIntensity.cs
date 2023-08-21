using UnityEngine;
using SonicBloom.Koreo;
using MoreMountains.Feedbacks;

namespace KitTraden.Abeyance.General.Feedbacks
{
    public class KoreographerApplySpanCurvesToFeedbackIntensity : MonoBehaviour
    {
        public MMFeedbacks Feedbacks;
        [Range(0f, 20f)] public float curveExponent = 1f;
        public Koreographer Koreographer;
        public string TriggerEventID;

        private void Start()
        {
            Initialize();
        }

        private void OnEnable()
        {
            Initialize();
            Koreographer.RegisterForEventsWithTime(TriggerEventID, OnKoreographyEvent);
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
        }

        private void OnKoreographyEvent(KoreographyEvent koreoEvent, int sampleTime, int _sampleDelta, DeltaSlice _deltaSlice)
        {
            var curve = koreoEvent.GetCurveValue();
            if (curve == null)
            {
                throw new System.Exception("Feedback intensity can only be controlled with curve events");
            }

            int timeIntoEvent = sampleTime - koreoEvent.StartSample;
            int eventLength = koreoEvent.EndSample - koreoEvent.StartSample;
            float fractionThroughEvent = ((float)timeIntoEvent) / ((float)eventLength);
            float currentValue = curve.Evaluate(fractionThroughEvent);

            Feedbacks.FeedbacksIntensity = Mathf.Pow(currentValue, curveExponent);
        }
    }
}