using System;
using UnityEngine;
using SonicBloom.Koreo;
using ND_VariaBULLET;
using System.Collections;

namespace KitTraden.Abeyance.VariaBULLET
{
    public class KoreographerShooter : MonoBehaviour
    {
        public SpreadPattern BulletPattern;
        public Koreographer Koreographer;
        public string TriggerEventID;

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
            StopAllCoroutines();
        }

        private void Initialize()
        {
            BulletPattern.TriggerAutoFire = false;

            if (Koreographer == null)
            {
                Koreographer = SonicBloom.Koreo.Koreographer.Instance;
            }
        }

        private void OnKoreographyEvent(KoreographyEvent _koreoEvent)
        {
            BulletPattern.TriggerAutoFire = true;
            StartCoroutine("StopFiring");
        }

        private IEnumerator StopFiring()
        {
            yield return new WaitForEndOfFrame();
            BulletPattern.TriggerAutoFire = false;
        }
    }
}