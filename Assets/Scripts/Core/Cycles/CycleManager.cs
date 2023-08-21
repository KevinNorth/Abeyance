using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MoreMountains.Feedbacks;
using KitTraden.Abeyance.Core.Events;

namespace KitTraden.Abeyance.Core.Cycles
{
    public class CycleManager : GameOverListener
    {
        [SerializeField][SerializeReference] CycleTimer timer;
        [Tooltip("The int argument is the number of the cycle that just started")]
        [SerializeField] UnityEvent<int> onCycleTurnover;
        [SerializeField] MMFeedbacks cycleTurnoverFeedbacks;

        private int currentCycle;

        void Start()
        {
            currentCycle = 1;
            timer.SetListener(this);
        }

        public void HandleCycleTurnover()
        {
            currentCycle += 1;
            onCycleTurnover.Invoke(currentCycle);
            cycleTurnoverFeedbacks?.PlayFeedbacks();
        }

        public void AddCycleTurnoverListener(UnityAction<int> listener)
        {
            onCycleTurnover.AddListener(listener);
        }

        public void RemoveCycleTurnoverListener(UnityAction<int> listener)
        {
            onCycleTurnover.RemoveListener(listener);
        }

        public int GetCurrentCycle()
        {
            return currentCycle;
        }

        public float TimePassedThisCycle()
        {
            return timer.TimePassedThisCycle();
        }

        public float TimeUntilNextCycle()
        {
            return timer.TimeUntilNextCycle();
        }

        public float DurationOfCurrentCycle()
        {
            return timer.DurationOfCurrentCycle();
        }

        public void Play()
        {
            timer.Play();
        }

        public void Pause()
        {
            timer.Pause();
        }

        public bool IsPlaying()
        {
            return timer.IsPlaying();
        }

        public bool IsPaused()
        {
            return timer.IsPaused();
        }

        public void ResetCycle(bool pause)
        {
            timer.ResetCycle(pause);
        }

        public void ForceTurnoverNow(bool pause)
        {
            currentCycle += 1;
            timer.ForceTurnoverNow(pause);
            onCycleTurnover.Invoke(currentCycle);
        }

        public void FullyReset(bool playImmediately, bool invokeTurnover)
        {
            currentCycle = 1;
            timer.FullyResetTimer(playImmediately);

            if (invokeTurnover)
            {
                onCycleTurnover.Invoke(currentCycle);
            }
        }

        public override void OnGameOver()
        {
            Pause();
        }
    }
}