using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KitTraden.Abeyance.Core.Cycles
{
    public abstract class CycleTimer : MonoBehaviour
    {
        [Tooltip("Whether to immediately begin the timer when the component Starts")]
        [SerializeField] protected bool playOnStart = false;
        protected CycleManager listener;

        protected virtual void Start()
        {
            InitializeTimer(playOnStart);
        }

        protected virtual void Update()
        {
            UpdateTimer();
        }

        /// <summary>
        /// Whether the timer is configured to start playing immediately
        /// after being initialized.
        /// </summary>
        public bool PlayOnStart()
        {
            return playOnStart;
        }

        /// <summary>
        /// Whether the timer is currently playing.
        /// </summary>
        public bool IsPlaying()
        {
            return !IsPaused();
        }

        /// <summary>
        /// Called when the CycleTimer's Start() lifecycle is called.
        /// </summary>
        /// <param name="playOnStart">
        /// If true, the timer should immediately start playing after being initialized.
        /// If false, the time should immediately pause instead.
        /// </param>
        protected abstract void InitializeTimer(bool playOnStart);

        /// <summary>
        /// Called once per frame to allow the timer to update its state.
        /// </summary>
        protected abstract void UpdateTimer();

        /// <summary>
        /// Whether the timer is currently paused.
        /// </summary>
        public abstract bool IsPaused();

        /// <summary>
        /// Informs the listener CycleManager that there is a cycle turnover.
        /// </summary>
        protected void NotifyListenerOfCycleTurnover()
        {
            listener.HandleCycleTurnover();
        }

        /// <summary>
        /// Tells the CycleTime which CycleManager to notify
        /// when cycles turn over.
        /// </summary>
        public void SetListener(CycleManager listener)
        {
            this.listener = listener;
        }

        /// <summary>
        /// Resets the CycleTimer as though it had been initialized
        /// for the first time.
        /// </summary>
        /// <param name="playImmediately">
        /// Starts running the timer immediately if true.
        /// Immediately pauses the timer if false.
        /// </param>
        public abstract void FullyResetTimer(bool playImmediately);

        /// <summary>
        /// Stops the timer while remembering how much time is left until the
        /// next turn over. Has no effect if the timer is already paused.
        /// </summary>
        public abstract void Pause();

        /// <summary>
        /// Resumes the timer with the same amount of time remaining as before
        /// it was paused. Has no effect if the timer is already playing.
        /// </summary>
        public abstract void Play();

        /// <summary>
        /// Restarts the current cycle from the beginning of its duration.
        /// Does not cause a turn over.
        /// </summary>
        /// <param name="pause">
        /// The timer will immediately pause after resetting the cycle if true.
        /// The timer will immediately play after resetting if false.
        /// </param>
        public abstract void ResetCycle(bool pause);

        /// <summary>
        /// Ends the current cycle and begins the next cycle from the beginning
        /// of its duration. Causes a turn over.
        /// </summary>
        /// <param name="pause">
        /// The timer will immediately pause after going to the next cycle if true.
        /// The timer will immediately play afterwards if false.
        /// </param>
        public abstract void ForceTurnoverNow(bool pause);

        /// <summary>
        /// Indicates how much time has passed in the current cycle.
        /// Does not count time spent paused.
        /// </summary>
        /// <returns>
        /// The number of seconds since the current cycle started.
        /// </returns>
        public abstract float TimePassedThisCycle();

        /// <summary>
        /// Indicates how much time is left until the next cycle begins.
        /// </summary>
        /// <returns>
        /// The number of seconds until the next cycle turnover.
        /// </returns>
        public abstract float TimeUntilNextCycle();

        /// <summary>
        /// Indicates how much time the current cycle lasts for from beginning to end.
        /// </summary>
        /// <returns>
        /// The number of seconds the current cycle lasts,
        /// from when it started to the next turnover.
        /// </returns>
        public abstract float DurationOfCurrentCycle();
    }
}