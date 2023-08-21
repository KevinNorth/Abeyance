using System;
using System.Linq;
using UnityEngine;
using DarkTonic.MasterAudio;
using Slate;
using KitTraden.Abeyance.Core.Music;

namespace KitTraden.Abeyance.Core.Cutscenes.ActionClips
{
    [Category("Music")]
    public class JumpToMusicClip : DirectorActionClip
    {
        public SongTransition SongTransition;
        [Range(0f, 10f)] public float CrossfadeTime = 0f;
        [SoundGroupAttribute]
        public string JumpBackMaskSound = null;
        [Range(0f, 1f)] public float JumpBackMaskVolume = 1f;

        private SongTransition _transitionBack;

        //This is written within the clip in the editor
        public override string info
        {
            get
            {
                if (SongTransition == null)
                {
                    return "Set a SongTransition on this clip to change music during the cutscene";
                }

                return $"{SongTransition.NewSongName} in playlist {SongTransition.NewPlaylistName}";
            }
        }

        //The length of the clip. Both get/set are optional. No override means the clip won't be scaleable.
        [SerializeField]
        [HideInInspector]
        private float _length = 1;

        public override float length
        {
            get { return _length; }
            set { _length = value; }
        }

        //Called once when the cutscene initialize. Return true if init was successful, or false if not
        protected override bool OnInitialize()
        {
            var playlist = MasterAudio.GrabPlaylist(SongTransition.NewPlaylistName);
            return playlist.MusicSettings.Any((MusicSetting song) => song.songName == SongTransition.NewSongName);
        }

        //Called in forward sampling when the clip is entered
        protected override void OnEnter()
        {
            TransitionToNewTrack();
        }

        //Called per frame while the clip is updating. Time is the local time within the clip.
        //So a time of 0, means the start of the clip.
        protected override void OnUpdate(float time) { }

        //Called in forwards sampling when the clip exits
        protected override void OnExit()
        {
            TransitionBack();
        }

        //Called in backwards sampling when the clip is entered.
        protected override void OnReverseEnter()
        {
            TransitionToNewTrack();
        }

        //Called in backwards sampling when the clip exits.
        protected override void OnReverse()
        {
            TransitionBack();
        }

        private void TransitionToNewTrack()
        {
            var musicManager = GameObject.FindObjectOfType<MusicManager>();

            _transitionBack = ScriptableObject.CreateInstance<SongTransition>();
            _transitionBack.NewPlaylistName = musicManager.GetCurrentPlaylistName();
            _transitionBack.NewSongName = musicManager.GetCurrentSongName();
            _transitionBack.CrossfadeTime = CrossfadeTime;
            _transitionBack.TransitionMaskSound = JumpBackMaskSound;
            _transitionBack.TransitionMaskVolume = JumpBackMaskVolume;
            _transitionBack.TransitionType = TransitionTypes.GO_TO_SPECIFIC_TIMESTAMP;
            _transitionBack.SampleToJumpTo = musicManager.GetCurrentCycleEndSample();

            musicManager.TransitionToNewTrack(SongTransition);
        }

        private void TransitionBack()
        {
            var musicManager = GameObject.FindObjectOfType<MusicManager>();
            musicManager.TransitionToNewTrack(_transitionBack);
        }
    }
}