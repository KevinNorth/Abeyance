using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KitTraden.Abeyance.Core.Cycles
{
    public class ClockCycleTimer : CycleTimer
    {
        [SerializeField][Range(1f, 180f)] float secondsPerCycle = 30f;

        private bool isPaused;
        private float lastCycleTurnoverTimestamp;
        private float lastPauseTimestamp;
        private float timeSpentPausedThisCycle;

        /// <summary>
        /// Called when the CycleTimer's Start() lifecycle is called.
        /// </summary>
        /// <param name="playOnStart">
        /// If true, the timer should immediately start playing after being initialized.
        /// If false, the time should immediately pause instead.
        /// </param>
        protected override void InitializeTimer(bool playOnStart)
        {
            FullyResetTimer(playOnStart);
        }

        /// <summary>
        /// Called once per frame to allow the timer to update its state.
        /// </summary>
        protected override void UpdateTimer()
        {
            if (!isPaused)
            {
                float timePassedThisCycle = TimePassedThisCycle();
                if (timePassedThisCycle >= secondsPerCycle)
                {
                    NotifyListenerOfCycleTurnover();
                    ResetCycle(false);
                }
            }
        }

        /// <summary>
        /// Whether the timer is currently paused.
        /// </summary>
        public override bool IsPaused()
        {
            return isPaused;
        }

        /// <summary>
        /// Resets the CycleTimer as though it had been initialized
        /// for the first time.
        /// </summary>
        /// <param name="playImmediately">
        /// Starts running the timer immediately if true.
        /// Immediately pauses the timer if false.
        /// </param>
        public override void FullyResetTimer(bool playImmediately)
        {
            isPaused = !playImmediately;
            lastCycleTurnoverTimestamp = Time.time;
            lastPauseTimestamp = Time.time;
            timeSpentPausedThisCycle = 0f;
        }

        /// <summary>
        /// Stops the timer while remembering how much time is left until the
        /// next turn over. Has no effect if the timer is already paused.
        /// </summary>
        public override void Pause()
        {
            if (!isPaused)
            {
                isPaused = true;
                lastPauseTimestamp = Time.time;
            }
        }

        /// <summary>
        /// Resumes the timer with the same amount of time remaining as before
        /// it was paused. Has no effect if the timer is already playing.
        /// </summary>
        public override void Play()
        {
            if (isPaused)
            {
                isPaused = false;
                timeSpentPausedThisCycle += (Time.time - lastPauseTimestamp);
            }
        }

        /// <summary>
        /// Restarts the current cycle from the beginning of its duration.
        /// Does not cause a turn over.
        /// </summary>
        /// <param name="pause">
        /// The timer will immediately pause after resetting the cycle if true.
        /// The timer will immediately play after resetting if false.
        /// </param>
        public override void ResetCycle(bool pause)
        {
            isPaused = pause;
            lastCycleTurnoverTimestamp = Time.time;
            lastPauseTimestamp = Time.time;
            timeSpentPausedThisCycle = 0f;
        }

        /// <summary>
        /// Ends the current cycle and begins the next cycle from the beginning
        /// of its duration. Causes a turn over.
        /// </summary>
        /// <param name="pause">
        /// The timer will immediately pause after going to the next cycle if true.
        /// The timer will immediately play afterwards if false.
        /// </param>
        public override void ForceTurnoverNow(bool pause)
        {
            ResetCycle(pause);
        }

        /// <summary>
        /// Indicates how much time has passed in the current cycle.
        /// Does not count time spent paused.
        /// </summary>
        /// <returns>
        /// The number of seconds since the current cycle started.
        /// </returns>
        public override float TimePassedThisCycle()
        {
            float timePassedThisCycle = (Time.time - lastCycleTurnoverTimestamp) - timeSpentPausedThisCycle;

            if (isPaused)
            {
                timePassedThisCycle -= (Time.time - lastPauseTimestamp);
            }

            return timePassedThisCycle;
        }

        /// <summary>
        /// Indicates how much time is left until the next cycle begins.
        /// </summary>
        /// <returns>
        /// The number of seconds until the next cycle turnover.
        /// </returns>
        public override float TimeUntilNextCycle()
        {
            return secondsPerCycle - TimePassedThisCycle();
        }

        /// <summary>
        /// Indicates how much time the current cycle lasts for from beginning to end.
        /// </summary>
        /// <returns>
        /// The number of seconds the current cycle lasts,
        /// from when it started to the next turnover.
        /// </returns>
        public override float DurationOfCurrentCycle()
        {
            return secondsPerCycle;
        }
    }
}