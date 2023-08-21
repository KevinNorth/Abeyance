using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using KitTraden.Abeyance.Core.Cycles;
using SonicBloom.Koreo.Players.MasterAudio;
using DarkTonic.MasterAudio;
using KitTraden.Abeyance.Core.Music;
using Sirenix.OdinInspector;

public class KoreographerCycleTimer : CycleTimer
{
    public static float SamplesToSeconds(int samples, int sampleRate = 48000)
    {
        return (float)samples / (float)sampleRate;
    }

    public static float SamplesToSeconds(int samples, AudioClip audioClip)
    {
        var sampleRate = Mathf.RoundToInt((float)audioClip.samples / audioClip.length);
        return SamplesToSeconds(samples, sampleRate);
    }

    public Koreographer koreographer;
    public MusicManager musicManager;
    [SoundGroupAttribute] public string jumpInMusicMaskSound;
    [Range(0f, 1f)] public float jumpInMusicMaskVolume = 1f;

    [ReadOnly]
    [ShowInInspector]
    protected int currentStartSample;
    [ReadOnly]
    [ShowInInspector]
    protected int currentEndSample;
    [ReadOnly]
    [ShowInInspector]
    protected bool hasFirstCycleStarted;
    [ReadOnly]
    [ShowInInspector]
    protected bool isTimerPaused;
    [ReadOnly]
    [ShowInInspector]
    protected bool skipNextCycleTurnover;

    // Start is called before the first frame update
    protected override void Start()
    {
        if (!koreographer)
        {
            koreographer = GameObject.FindObjectOfType<Koreographer>();
        }

        currentStartSample = 0;
        currentEndSample = 0;
        hasFirstCycleStarted = false;
        isTimerPaused = !playOnStart;
        skipNextCycleTurnover = false;
    }

    protected void OnEnable()
    {
        koreographer.RegisterForEventsWithTime("Cycles", OnKoreographerCycleEvent);
    }

    protected void OnDisable()
    {
        koreographer.UnregisterForAllEvents(this);
    }

    void OnKoreographerCycleEvent(KoreographyEvent koreoEvent, int _sampleTime, int _sampleDelta, DeltaSlice deltaSlice)
    {
        if (hasFirstCycleStarted && koreoEvent.StartSample == currentStartSample)
        {
            return;
        }

        if (skipNextCycleTurnover)
        {
            skipNextCycleTurnover = false;
            currentStartSample = koreoEvent.StartSample;
            currentEndSample = koreoEvent.EndSample;
            return;
        }

        if (hasFirstCycleStarted && !isTimerPaused)
        {
            NotifyListenerOfCycleTurnover();
        }

        hasFirstCycleStarted = true;

        currentStartSample = koreoEvent.StartSample;
        currentEndSample = koreoEvent.EndSample;
    }

    public override float DurationOfCurrentCycle()
    {
        if (!musicManager.GetCurrentAudioClip())
        {
            return 0;
        }

        return SamplesToSeconds(currentEndSample - currentStartSample, musicManager.GetCurrentAudioClip());
    }

    public override void ForceTurnoverNow(bool pause)
    {
        musicManager.JumpToTimestampInCurrentSong(currentEndSample, jumpInMusicMaskSound, jumpInMusicMaskVolume);

        // The CycleManager already increments the cycle count when ForceTurnoverNow() is called
        skipNextCycleTurnover = true;
    }

    public override void FullyResetTimer(bool playImmediately)
    {
        musicManager.JumpToTimestampInCurrentSong(0, jumpInMusicMaskSound, jumpInMusicMaskVolume);
        currentStartSample = 0;
        isTimerPaused = !playImmediately;
        hasFirstCycleStarted = false;
    }

    public override bool IsPaused()
    {
        return isTimerPaused;
    }

    public override void Pause()
    {
        isTimerPaused = true;
    }

    public override void Play()
    {
        isTimerPaused = false;
    }

    public override void ResetCycle(bool pause)
    {
        isTimerPaused = pause;
        skipNextCycleTurnover = true;

        musicManager.JumpToTimestampInCurrentSong(
            currentStartSample,
            jumpInMusicMaskSound,
            jumpInMusicMaskVolume
        );
    }

    public override float TimePassedThisCycle()
    {
        var audioClip = musicManager.GetCurrentAudioClip();
        var audioSource = musicManager.GetCurrentAudioSource();

        if (audioClip == null || audioSource == null)
        {
            return 0;
        }

        int samplesPassed = Mathf.Clamp(audioSource.timeSamples - currentStartSample, 0, currentEndSample - currentStartSample);

        return SamplesToSeconds(samplesPassed, audioClip);
    }

    public override float TimeUntilNextCycle()
    {
        var audioClip = musicManager.GetCurrentAudioClip();
        var audioSource = musicManager.GetCurrentAudioSource();

        if (audioClip == null || audioSource == null)
        {
            return 0;
        }

        int samplesRemaining = Mathf.Clamp(currentEndSample - audioSource.timeSamples, 0, currentEndSample - currentStartSample);

        return SamplesToSeconds(samplesRemaining, audioClip);
    }

    protected override void InitializeTimer(bool playOnStart)
    {
        isTimerPaused = !playOnStart;
    }

    protected override void UpdateTimer()
    {
        // Do nothing
        // The timer is controlled by Koreographer events
    }
}
