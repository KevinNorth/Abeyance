using System;
using UnityEngine;
using UnityEngine.Events;
using Slate;

namespace KitTraden.Abeyance.Core.Cutscenes
{
    public class CutsceneTrigger2D : MonoBehaviour
    {
        public Cutscene Cutscene;
        public bool DestroyAfterCutscene = true;
        public UnityEvent OnCutsceneStart;
        public UnityEvent OnCutsceneEnd;
        private int playerLayerMask;

        void Start()
        {
            playerLayerMask = LayerMask.GetMask("Player");
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            var otherLayerMask = 1 << other.gameObject.layer;
            if ((otherLayerMask & playerLayerMask) != 0)
            {
                OnCutsceneStart.Invoke();
                Cutscene.Play(CutsceneEndCallback);
            }
        }

        private void CutsceneEndCallback()
        {
            OnCutsceneEnd.Invoke();

            if (DestroyAfterCutscene)
            {
                Destroy(gameObject);
            }
        }
    }
}