using System;
using UnityEngine;
using DarkTonic.MasterAudio;
using Sirenix.OdinInspector;

namespace KitTraden.Abeyance.Core.Music
{
    public enum TransitionTypes
    {
        GO_TO_BEGINNING,
        GO_TO_SAME_TIMESTAMP,
        GO_TO_SPECIFIC_TIMESTAMP,
        USE_MASTER_AUDIO_TRANSITION_GROUP
    }

    [CreateAssetMenu(fileName = "SongTransition", menuName = "Song Transition")]
    public class SongTransition : ScriptableObject
    {
        [PlaylistAttribute]
        public string NewPlaylistName;
        public string NewSongName;
        [SoundGroupAttribute]
        public string TransitionMaskSound;
        [Range(0f, 1f)] public float TransitionMaskVolume = 1f;
        [Tooltip("Set to 0 for an immediate cut to the next track")]
        [Range(0f, 10f)] public float CrossfadeTime = 0f;
        public TransitionTypes TransitionType;
        [Tooltip("The sample timestamp to jump to if Transition Type is GO_TO_SPECIFIC_TIMESTAMP")]
        [MinValue(0)] public int SampleToJumpTo = 0;
    }
}