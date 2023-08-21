using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace KitTraden.Abeyance.Core.Cycles
{
    public class CycleManagerDebugger : MonoBehaviour
    {
        [SerializeField] CycleManager cycleManager;
        [SerializeField] bool logTimestamps = false;
        [ReadOnly]
        [ShowInInspector]
        public int cycleCount
        {
            get
            {
                if (cycleManager == null)
                {
                    return -1;
                }

                return cycleManager.GetCurrentCycle();
            }
        }

        [ReadOnly]
        [ShowInInspector]
        public bool playing
        {
            get
            {
                if (cycleManager == null)
                {
                    return false;
                }

                return cycleManager.IsPlaying();
            }
        }

        [ReadOnly]
        [ShowInInspector]
        public bool paused
        {
            get
            {
                if (cycleManager == null)
                {
                    return false;
                }

                return cycleManager.IsPaused();
            }
        }

        void OnEnable()
        {
            cycleManager.AddCycleTurnoverListener(OnCycleTurnover);
        }

        void OnDisable()
        {
            cycleManager.RemoveCycleTurnoverListener(OnCycleTurnover);
        }

        void Update()
        {
            if (logTimestamps)
            {
                LogTimestampNow();
            }
        }

        void OnCycleTurnover(int nextCycle)
        {
            Debug.Log($"Cycle Turnover! Next cycle: {nextCycle}");
        }

        [Button]
        void LogTimestampNow()
        {
            Debug.Log($"Cycle Duration: {cycleManager.DurationOfCurrentCycle()} | Time Elapsed: {cycleManager.TimePassedThisCycle()} | Time Remaining: {cycleManager.TimeUntilNextCycle()}");
        }

        [Button]
        void Play()
        {
            cycleManager.Play();
        }

        [Button]
        void Pause()
        {
            cycleManager.Pause();
        }

        [Button]
        void ResetCycle()
        {
            cycleManager.ResetCycle(false);
        }

        [Button]
        void ForceTurnoverNow()
        {
            cycleManager.ForceTurnoverNow(false);
        }

        [Button]
        void FullyReset()
        {
            cycleManager.FullyReset(true, true);
        }
    }
}