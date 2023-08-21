using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace KitTraden.Abeyance.Core.Music
{
    public class MusicManagerDebugger : MonoBehaviour
    {
        [SerializeField] MusicManager musicManager;

        [ReadOnly]
        [ShowInInspector]
        public string playlistName
        {
            get
            {
                return musicManager?.GetCurrentPlaylistName();
            }
        }

        [ReadOnly]
        [ShowInInspector]
        public string songName
        {
            get
            {
                return musicManager?.GetCurrentSongName();
            }
        }

        [ReadOnly]
        [ShowInInspector]
        public int currentStartSample
        {
            get
            {
                if (musicManager == null)
                {
                    return -1;
                }

                return musicManager.GetCurrentCycleStartSample();
            }
        }

        [ReadOnly]
        [ShowInInspector]
        public int currentEndSample
        {
            get
            {
                if (musicManager == null)
                {
                    return -1;
                }

                return musicManager.GetCurrentCycleEndSample();
            }
        }

        [Button]
        void PlayMusic()
        {
            musicManager.PlayMusic();
        }

        [Button]
        void PauseMusic()
        {
            musicManager.PauseMusic();
        }
    }
}