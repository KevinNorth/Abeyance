using UnityEngine;

namespace KitTraden.Abeyance.General
{
    public class DestroySelfWithTimer : MonoBehaviour
    {
        public enum OnTimerEndBehavior
        {
            Destroy,
            Disable
        }

        public float SecondsBeforeDestroyingSelf = 1.0f;
        public OnTimerEndBehavior onTimerEndBehavior = OnTimerEndBehavior.Destroy;
        public bool UseUnscaledTime = false;
        public bool ResetOnEnable = false;

        private float startTimestamp;

        void Start()
        {
            startTimestamp = GetCurrentTime();
        }

        void OnEnable()
        {
            if (ResetOnEnable)
            {
                startTimestamp = GetCurrentTime();
            }
        }

        void Update()
        {
            float currentTime = GetCurrentTime();

            if (currentTime - startTimestamp > SecondsBeforeDestroyingSelf)
            {
                switch (onTimerEndBehavior)
                {
                    case OnTimerEndBehavior.Destroy:
                        {
                            Destroy(gameObject);
                            break;
                        }
                    case OnTimerEndBehavior.Disable:
                        {
                            gameObject.SetActive(false);
                            break;
                        }
                    default:
                        {
                            throw new System.Exception($"Don't know how to handle OnTimerEndBehavior of {onTimerEndBehavior}");
                        }
                }
            }
        }

        private float GetCurrentTime()
        {
            if (UseUnscaledTime)
            {
                return Time.unscaledTime;
            }
            else
            {
                return Time.time;
            }
        }
    }
}