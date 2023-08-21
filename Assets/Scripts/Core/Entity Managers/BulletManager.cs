using System;
using UnityEngine;
using ND_VariaBULLET;
using KitTraden.Abeyance.Core.Cycles;

namespace KitTraden.Abeyance.Core.EntityManagers
{
    public class BulletManager : MonoBehaviour
    {
        void OnEnable()
        {
            var cycleManager = GameObject.FindObjectOfType<CycleManager>();

            if (cycleManager != null)
            {
                cycleManager.AddCycleTurnoverListener(ClearAllShots);
            }
        }

        void OnDisable()
        {
            var cycleManager = GameObject.FindObjectOfType<CycleManager>();

            if (cycleManager != null)
            {
                cycleManager.RemoveCycleTurnoverListener(ClearAllShots);
            }
        }

        public void ClearAllShots(int _currentCycle)
        {
            var shots = GameObject.FindObjectsOfType<ShotBase>();

            foreach (var shot in shots)
            {
                // There isn't a great way to sensibly reset lasers that
                // are supposed to persist across cycles, so we'll leave
                // them be.
                if (shot is ShotLaserBase)
                {
                    continue;
                }

                if (shot.FiringScript is IPooler)
                {
                    shot.RePool(shot.FiringScript as IPooler);
                }
                else
                {
                    Destroy(shot);
                    GlobalShotManager.Instance.ActiveBullets -= 1;
                }
            }
        }
    }
}