using System;
using UnityEngine;
using MoreMountains.Tools;

namespace KitTraden.Abeyance.Core.Events
{
    public abstract class GameOverListener : MonoBehaviour, MMEventListener<MMGameEvent>
    {
        public virtual void OnEnable()
        {
            this.MMEventStartListening<MMGameEvent>();
        }

        public virtual void OnDisable()
        {
            this.MMEventStopListening<MMGameEvent>();
        }

        public virtual void OnMMEvent(MMGameEvent gameEvent)
        {
            switch (gameEvent.EventName)
            {
                case "GameOver":
                    {
                        OnGameOver();
                        break;
                    }
                default:
                    {
                        // Let the event pass through
                        break;
                    }
            }
        }

        public abstract void OnGameOver();
    }
}