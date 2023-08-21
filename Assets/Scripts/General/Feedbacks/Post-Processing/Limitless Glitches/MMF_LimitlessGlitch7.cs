﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using MoreMountains.FeedbacksForThirdParty;

namespace KitTraden.Abeyance.TopDownEngine.Feedbacks
{
    /// <summary>
    /// This feedback allows you to control the Limitless Glitch 7 post-processing effect's
    /// intensity over time. It requires you have in your scene an object with a PostProcessVolume
    /// with Limitless Glitch 7 active, and a MMLimitlessGlitch7Shaker component.
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackPath("PostProcess/Limitless Glitch/7")]
    [FeedbackHelp("This feedback allows you to control the Limitless Glitch 7 post-processing effect's " +
                  "intensity over time. It requires you have in your scene an object with a PostProcessVolume " +
                  "with Limitless Glitch 7 active, and a MMLimitlessGlitch7Shaker component")]
    public class MMF_LimitlessGlitch7 : MMF_Feedback
    {
        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;
        /// sets the inspector color for this feedback
#if UNITY_EDITOR
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.PostProcessColor; } }
        public override string RequiredTargetText => RequiredChannelText;
#endif

        /// the duration of this feedback is the duration of the shake
        public override float FeedbackDuration { get { return ApplyTimeMultiplier(Duration); } set { Duration = value; } }
        public override bool HasChannel => true;
        public override bool HasRandomness => true;

        [MMFInspectorGroup("Limitless Glitch 7", true, 44)]
        /// the duration of the shake, in seconds
        [Tooltip("the duration of the shake, in seconds")]
        public float Duration = 0.2f;
        /// whether or not to reset shaker values after shake
        [Tooltip("whether or not to reset shaker values after shake")]
        public bool ResetShakerValuesAfterShake = true;
        /// whether or not to reset the target's values after shake
        [Tooltip("whether or not to reset the target's values after shake")]
        public bool ResetTargetValuesAfterShake = true;
        /// the value to remap the curve's 0 to
        [Tooltip("the value to remap the curve's 0 to")]
        [Range(0f, 1f)]
        public float RemapFadeZero = 0f;
        /// the value to remap the curve's 1 to
        [Tooltip("the value to remap the curve's 1 to")]
        [Range(0f, 1f)]
        public float RemapFadeOne = 1f;

        [MMFInspectorGroup("Fade", true, 45)]
        /// the curve to animate the intensity on
        [Tooltip("the curve to animate the fade on")]
        public AnimationCurve Fade = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        /// the multiplier to apply to the intensity curve
        [Tooltip("the multiplier to apply to the fade curve")]
        [Range(0f, 1f)]
        public float Amplitude = 1.0f;
        /// whether or not to add to the initial intensity
        [Tooltip("whether or not to add to the initial fade")]
        public bool RelativeFade = false;

        /// <summary>
        /// Triggers a Limitless Glitch 01 shake
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized)
            {
                return;
            }
            float intensityMultiplier = ComputeIntensity(feedbacksIntensity, position);
            MMLimitlessGlitch7ShakeEvent.Trigger(Fade, FeedbackDuration, RemapFadeZero, RemapFadeOne, RelativeFade, intensityMultiplier,
                ChannelData, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake, NormalPlayDirection, ComputedTimescaleMode);
        }

        /// <summary>
        /// On stop we stop our transition
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            if (!Active || !FeedbackTypeAuthorized)
            {
                return;
            }
            base.CustomStopFeedback(position, feedbacksIntensity);
            MMLimitlessGlitch7ShakeEvent.Trigger(Fade, FeedbackDuration, RemapFadeZero, RemapFadeOne, RelativeFade, stop: true);
        }

        /// <summary>
        /// On restore, we put our object back at its initial position
        /// </summary>
        protected override void CustomRestoreInitialValues()
        {
            if (!Active || !FeedbackTypeAuthorized)
            {
                return;
            }
            MMLimitlessGlitch7ShakeEvent.Trigger(Fade, FeedbackDuration, RemapFadeZero, RemapFadeOne, RelativeFade, restore: true);
        }
    }
}