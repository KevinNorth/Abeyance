using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using DarkTonic.MasterAudio;
using SonicBloom.Koreo;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using KitTraden.Abeyance.Core.Cycles;

namespace KitTraden.Abeyance.Core.Music
{
    public class MusicManager : MonoBehaviour
    {
        public Koreographer Koreographer;
        public Koreography[] Koreographies;
        public PlaylistController StartingPlaylist;
        public MMTimeManager TimeManager;
        public CycleManager CycleManager;
        public string StartingSongName;
        public bool BeginPlayingMusicOnSceneStart = true;
        [Range(0f, 60f)] public float FadeOutTimeOnGameOver = 10f;
        public bool EnsureCycleTurnoverWhenLoopingWithinSameCycle = true;

        private protected PlaylistController __currentPlaylistController;
        private PlaylistController _currentPlaylistController
        {
            get { return __currentPlaylistController; }
            set
            {
                __currentPlaylistController.SongChanged -= OnSongChanged;
                __currentPlaylistController.SongEnded -= OnSongEnded;
                __currentPlaylistController = value;
                __currentPlaylistController.SongChanged += OnSongChanged;
                __currentPlaylistController.SongEnded += OnSongEnded;
            }
        }
        private string _currentSongName = null;
        private SongTransition _transitionAtEndOfSong = null;
        private int _currentCycleStartSample = 0;
        private int _currentCycleEndSample = 0;

        void Awake()
        {
            __currentPlaylistController = StartingPlaylist;

            if (__currentPlaylistController != null)
            {
                __currentPlaylistController.SongChanged += OnSongChanged;
                __currentPlaylistController.SongEnded += OnSongEnded;
            }
        }

        void Start()
        {
            if (BeginPlayingMusicOnSceneStart)
            {
                _currentSongName = StartingSongName;
                _currentPlaylistController.TriggerPlaylistClip(StartingSongName);
                LoadKoreographyForCurrentSong();
            }
        }

        void Update()
        {
            var currentAudioSource = GetCurrentAudioSource();
            var fadingAudioSource = _currentPlaylistController?.FadingSource;
            var timeScale = TimeManager.CurrentTimeScale;

            if (currentAudioSource != null && currentAudioSource.pitch != timeScale)
            {
                currentAudioSource.pitch = timeScale;
                if (fadingAudioSource != null)
                {
                    fadingAudioSource.pitch = timeScale;
                }

                MasterAudio.ChangePlaylistPitch(GetCurrentPlaylistName(), timeScale);
            }
        }

        void OnEnable()
        {
            Koreographer.RegisterForEventsWithTime("Cycles", OnKoreographerCycle);
            Koreographer.RegisterForEvents("SongLoop", OnKoreographerSongLoop);
        }

        void OnDisable()
        {
            Koreographer.UnregisterForEvents("Cycles", OnKoreographerCycle);
            Koreographer.UnregisterForEvents("SongLoop", OnKoreographerSongLoop);
        }

        public string GetCurrentPlaylistName()
        {
            return _currentPlaylistController?.PlaylistName;
        }

        public string GetCurrentSongName()
        {
            return _currentSongName;
        }

        public AudioClip GetCurrentAudioClip()
        {
            return _currentPlaylistController?.CurrentPlaylistClip;
        }

        public AudioSource GetCurrentAudioSource()
        {
            return _currentPlaylistController?.ActiveAudioSource;
        }

        public int GetCurrentCycleStartSample()
        {
            return _currentCycleStartSample;
        }

        public int GetCurrentCycleEndSample()
        {
            return _currentCycleEndSample;
        }

        public void OnSongChanged(string newSongName, MusicSetting musicSetting)
        {
            _currentSongName = newSongName;
        }

        public void OnSongEnded(String _songName)
        {
            if (_transitionAtEndOfSong != null)
            {
                TransitionToNewTrack(_transitionAtEndOfSong);
                _transitionAtEndOfSong = null;
            }
        }

        public void OnKoreographerCycle(KoreographyEvent koreoEvent, int _sampleTime, int _sampleDelta, DeltaSlice deltaSlice)
        {
            _currentCycleStartSample = koreoEvent.StartSample;
            _currentCycleEndSample = koreoEvent.EndSample;
        }

        public void OnKoreographerSongLoop(KoreographyEvent koreoEvent)
        {
            int loopPoint = koreoEvent.GetIntValue();

            if (!koreoEvent.HasIntPayload() || loopPoint < 0 || loopPoint > GetCurrentAudioSource().timeSamples)
            {
                throw new Exception("Expected SongLoop KoreographyEvent to have an int payload indicating the sample to loop back to");
            }

            if (EnsureCycleTurnoverWhenLoopingWithinSameCycle
                && loopPoint >= _currentCycleStartSample
                && loopPoint <= _currentCycleEndSample
                && GetCurrentAudioSource().timeSamples <= _currentCycleEndSample)
            {
                // If jumping back in the song won't trigger a new cycle on its own,
                // we'll do so manually.
                CycleManager.HandleCycleTurnover();
            }

            JumpToTimestampInCurrentSong(loopPoint);
        }

        public void PauseMusic()
        {
            if (_currentPlaylistController.PlaylistState == PlaylistController.PlaylistStates.Playing
                || _currentPlaylistController.PlaylistState == PlaylistController.PlaylistStates.Crossfading)
            {
                _currentPlaylistController.PausePlaylist();
            }
            else if (_currentPlaylistController.PlaylistState == PlaylistController.PlaylistStates.NotInScene)
            {
                throw new System.Exception("Tried to pause a playlist that isn't in the current scene");
            }
            else
            {
                Debug.LogWarning("Tried to pause a playlist that is already paused or stopped. This has no effect.");
            }
        }

        public void PlayMusic()
        {
            if (_currentPlaylistController.PlaylistState == PlaylistController.PlaylistStates.Paused
                || _currentPlaylistController.PlaylistState == PlaylistController.PlaylistStates.Stopped)
            {
                if (_currentSongName == null)
                {
                    _currentSongName = StartingSongName;
                    LoadKoreographyForCurrentSong();
                }

                _currentPlaylistController.TriggerPlaylistClip(_currentSongName);
            }
            else if (_currentPlaylistController.PlaylistState == PlaylistController.PlaylistStates.NotInScene)
            {
                throw new System.Exception("Tried to play a playlist that isn't in the current scene");
            }
            else
            {
                Debug.LogWarning("Tried to play a playlist that is already playing. This has no effect.");
            }
        }

        public void JumpToTimestampInCurrentSong(int sampleTimestamp, string jumpMaskSound = null, float jumpMaskVolume = 1f)
        {
            var currentSong = GetCurrentAudioClip();

            if (sampleTimestamp < 0 || sampleTimestamp > currentSong.samples)
            {
                throw new System.Exception($"Timestamp of {sampleTimestamp} samples is not in the length of current song ${_currentSongName}, which is ${currentSong.samples} samples long");
            }

            var currentAudioSource = GetCurrentAudioSource();
            currentAudioSource.timeSamples = sampleTimestamp;

            if (jumpMaskSound != null)
            {
                MasterAudio.PlaySoundAndForget(jumpMaskSound, jumpMaskVolume);
            }
        }

        public void JumpToTimestampInCurrentSong(float secondsIntoSong, string jumpMaskSound = null, float jumpMaskVolume = 1f)
        {
            var currentSong = GetCurrentAudioClip();

            if (secondsIntoSong < 0 || secondsIntoSong > currentSong.length)
            {
                throw new System.Exception($"Timestamp of {secondsIntoSong} seconds is not in the length of current song {_currentSongName}, which is {currentSong.length} seconds long");
            }

            float sampleRate = ((float)currentSong.samples) / currentSong.length;
            int samplesIntoSong = Mathf.Clamp(Mathf.RoundToInt(secondsIntoSong * sampleRate), 0, currentSong.samples);
            JumpToTimestampInCurrentSong(secondsIntoSong, jumpMaskSound, jumpMaskVolume);
        }

        public void TransitionToNewTrack(SongTransition songTransition)
        {
            if (songTransition.NewPlaylistName == GetCurrentPlaylistName()
                && songTransition.NewSongName == _currentSongName)
            {
                if (songTransition.TransitionType == TransitionTypes.GO_TO_BEGINNING)
                {
                    JumpToTimestampInCurrentSong(
                        0,
                        songTransition.TransitionMaskSound,
                        songTransition.TransitionMaskVolume
                    );
                }

                return;
            }

            var currentTimestamp = GetCurrentAudioSource().timeSamples;
            MasterAudio.Instance.MasterCrossFadeTime = songTransition.CrossfadeTime;

            if (songTransition.NewPlaylistName == GetCurrentPlaylistName())
            {
                GoToNewTrackInSamePlaylist(songTransition);
            }
            else
            {
                GoToNewTrackInDifferentPlaylist(songTransition);
            }

            switch (songTransition.TransitionType)
            {
                case TransitionTypes.GO_TO_BEGINNING:
                    {
                        JumpToTimestampInCurrentSong(
                            0,
                            songTransition.TransitionMaskSound,
                            songTransition.TransitionMaskVolume
                        );
                        break;
                    }
                case TransitionTypes.GO_TO_SAME_TIMESTAMP:
                    {
                        JumpToTimestampInCurrentSong(
                            Mathf.Clamp(currentTimestamp, 0, GetCurrentAudioClip().samples),
                            songTransition.TransitionMaskSound,
                            songTransition.TransitionMaskVolume
                        );
                        break;
                    }
                case TransitionTypes.GO_TO_SPECIFIC_TIMESTAMP:
                    {
                        JumpToTimestampInCurrentSong(
                            Mathf.Clamp(songTransition.SampleToJumpTo, 0, GetCurrentAudioClip().samples),
                            songTransition.TransitionMaskSound,
                            songTransition.TransitionMaskVolume
                        );
                        break;
                    }
                case TransitionTypes.USE_MASTER_AUDIO_TRANSITION_GROUP:
                    {
                        // Rely on Master Audio to correctly sync up the music.
                        // Just play the transition mask sound.
                        MasterAudio.PlaySoundAndForget(songTransition.TransitionMaskSound, songTransition.TransitionMaskVolume);
                        break;
                    }
                default:
                    {
                        throw new System.ArgumentException($"Don't know how to handle a transition type of ${songTransition.TransitionType}");
                    }
            }

            LoadKoreographyForCurrentSong();
        }

        public void PlaySongOnceAndThenReturnAtNextCycle(SongTransition songTransition, float crossfadeTime = 0f, string jumpBackMaskSound = null, float jumpBackMaskVolume = 1f)
        {
            _transitionAtEndOfSong = ScriptableObject.CreateInstance<SongTransition>();
            _transitionAtEndOfSong.NewSongName = GetCurrentSongName();
            _transitionAtEndOfSong.NewPlaylistName = GetCurrentPlaylistName();
            _transitionAtEndOfSong.TransitionType = TransitionTypes.GO_TO_SAME_TIMESTAMP;
            _transitionAtEndOfSong.CrossfadeTime = crossfadeTime;
            _transitionAtEndOfSong.TransitionMaskSound = jumpBackMaskSound;
            _transitionAtEndOfSong.TransitionMaskVolume = jumpBackMaskVolume;
            _transitionAtEndOfSong.SampleToJumpTo = _currentCycleEndSample;

            TransitionToNewTrack(songTransition);
        }

        private void GoToNewTrackInSamePlaylist(SongTransition songTransition)
        {
            _currentPlaylistController.TriggerPlaylistClip(songTransition.NewSongName);
            _currentSongName = songTransition.NewSongName;
        }

        private void GoToNewTrackInDifferentPlaylist(SongTransition songTransition)
        {
            MasterAudio.ChangePlaylistByName(songTransition.NewPlaylistName, false);

            _currentPlaylistController.TriggerPlaylistClip(songTransition.NewSongName);
            _currentSongName = songTransition.NewSongName;
        }

        private void LoadKoreographyForCurrentSong()
        {
            var currentAudioClip = GetCurrentAudioClip();
            var koreographyToLoad = Koreographies.First(
                (Koreography koreography) => koreography.SourceClip == currentAudioClip || koreography.SourceClipName == _currentSongName
            );

            Koreographer.LoadKoreography(koreographyToLoad);

            List<Koreography> loadedKoreographies = new List<Koreography>();
            Koreographer.GetAllLoadedKoreography(loadedKoreographies);

            foreach (var koreography in loadedKoreographies)
            {
                if (koreography != koreographyToLoad)
                {
                    Koreographer.UnloadKoreography(koreography);
                }
            }
        }
    }
}