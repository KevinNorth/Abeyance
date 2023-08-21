using System;
using System.Linq;
using UnityEngine;
using DarkTonic.MasterAudio;
using Slate;
using KitTraden.Abeyance.Core.Music;

namespace KitTraden.Abeyance.Core.Cutscenes.ActionClips
{
    [Category("Music")]
    public class JumpToMusicClipAndStay : DirectorActionClip
    {
        public SongTransition SongTransition;

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

        protected override void OnReverseEnter()
        {
            TransitionToNewTrack();
        }

        private void TransitionToNewTrack()
        {
            var musicManager = GameObject.FindObjectOfType<MusicManager>();
            musicManager.TransitionToNewTrack(SongTransition);
        }
    }
}