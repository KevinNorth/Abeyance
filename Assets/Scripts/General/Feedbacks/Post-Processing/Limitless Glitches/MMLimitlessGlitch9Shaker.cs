using UnityEngine;
#if MM_POSTPROCESSING
using UnityEngine.Rendering.PostProcessing;
#endif
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.FeedbacksForThirdParty;
using UnityEngine.Rendering.PostProcessing;

namespace KitTraden.Abeyance.TopDownEngine.Feedbacks
{
    /// <summary>
    /// Add this class to a Camera with a Limitless Glitch 9 post processing and it'll be able to "shake" its values by getting events
    /// </summary>
    [AddComponentMenu("More Mountains/Feedbacks/Shakers/PostProcessing/Limitless Glitch/MMLimitlessGlitch9Shaker")]
#if MM_POSTPROCESSING
	[RequireComponent(typeof(PostProcessVolume))]
#endif
    public class MMLimitlessGlitch9Shaker : MMShaker
    {
        [MMInspectorGroup("Limitless Glitch 9 Intensity", true, 46)]
        /// whether or not to add to the initial value
        [Tooltip("whether or not to add to the initial value")]
        public bool RelativeFade = false;
        /// the curve used to animate the intensity value on
        [Tooltip("the curve used to animate the intensity value on")]
        public AnimationCurve ShakeFade = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        /// the value to remap the curve's 0 to
        [Tooltip("the value to remap the curve's 0 to")]
        [Range(0f, 1f)]
        public float RemapFadeZero = 0f;
        /// the value to remap the curve's 1 to
        [Tooltip("the value to remap the curve's 1 to")]
        [Range(0f, 1f)]
        public float RemapFadeOne = 1f;

        protected PostProcessVolume _volume;
        protected LimitlessGlitch9 _limitlessGlitch9;
        protected float _initialFade;
        protected float _originalShakeDuration;
        protected AnimationCurve _originalShakeIntensity;
        protected float _originalRemapIntensityZero;
        protected float _originalRemapIntensityOne;
        protected bool _originalRelativeIntensity;

        /// <summary>
        /// On init we initialize our values
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();

            _volume = this.gameObject.GetComponent<PostProcessVolume>();
            _volume.profile.TryGetSettings(out _limitlessGlitch9);
        }

        /// <summary>
        /// Shakes values over time
        /// </summary>
        protected override void Shake()
        {
            float newValue = ShakeFloat(ShakeFade, RemapFadeZero, RemapFadeOne, RelativeFade, _initialFade);
            _limitlessGlitch9.fade.Override(newValue);
        }

        /// <summary>
        /// Collects initial values on the target
        /// </summary>
        protected override void GrabInitialValues()
        {
            _initialFade = _limitlessGlitch9.fade;
        }

        /// <summary>
        /// When we get the appropriate event, we trigger a shake
        /// </summary>
        /// <param name="fade"></param>
        /// <param name="duration"></param>
        /// <param name="amplitude"></param>
        /// <param name="relativeFade"></param>
        /// <param name="feedbacksIntensity"></param>
        /// <param name="channel"></param>
        public virtual void OnMMLimitlessGlitch9ShakeEvent(AnimationCurve fade, float duration, float remapMin, float remapMax, bool relativeFade = false,
            float feedbacksIntensity = 1.0f, MMChannelData channelData = null, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true,
            bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false, bool restore = false)
        {
            if (!CheckEventAllowed(channelData) || (!Interruptible && Shaking))
            {
                return;
            }

            if (stop)
            {
                Stop();
                return;
            }

            if (restore)
            {
                ResetTargetValues();
                return;
            }

            _resetShakerValuesAfterShake = resetShakerValuesAfterShake;
            _resetTargetValuesAfterShake = resetTargetValuesAfterShake;

            if (resetShakerValuesAfterShake)
            {
                _originalShakeDuration = ShakeDuration;
                _originalShakeIntensity = ShakeFade;
                _originalRemapIntensityZero = RemapFadeZero;
                _originalRemapIntensityOne = RemapFadeOne;
                _originalRelativeIntensity = RelativeFade;
            }

            TimescaleMode = timescaleMode;
            ShakeDuration = duration;
            ShakeFade = fade;
            RemapFadeZero = remapMin * feedbacksIntensity;
            RemapFadeOne = remapMax * feedbacksIntensity;
            RelativeFade = relativeFade;
            ForwardDirection = forwardDirection;

            Play();
        }

        /// <summary>
        /// Resets the target's values
        /// </summary>
        protected override void ResetTargetValues()
        {
            base.ResetTargetValues();
            _limitlessGlitch9.fade.Override(_initialFade);
        }

        /// <summary>
        /// Resets the shaker's values
        /// </summary>
        protected override void ResetShakerValues()
        {
            base.ResetShakerValues();
            ShakeDuration = _originalShakeDuration;
            ShakeFade = _originalShakeIntensity;
            RemapFadeZero = _originalRemapIntensityZero;
            RemapFadeOne = _originalRemapIntensityOne;
            RelativeFade = _originalRelativeIntensity;
        }

        /// <summary>
        /// Starts listening for events
        /// </summary>
        public override void StartListening()
        {
            base.StartListening();
            MMLimitlessGlitch9ShakeEvent.Register(OnMMLimitlessGlitch9ShakeEvent);
        }

        /// <summary>
        /// Stops listening for events
        /// </summary>
        public override void StopListening()
        {
            base.StopListening();
            MMLimitlessGlitch9ShakeEvent.Unregister(OnMMLimitlessGlitch9ShakeEvent);
        }
    }

    /// <summary>
    /// An event used to trigger vignette shakes
    /// </summary>
    public struct MMLimitlessGlitch9ShakeEvent
    {
        static private event Delegate OnEvent;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)] private static void RuntimeInitialization() { OnEvent = null; }
        static public void Register(Delegate callback) { OnEvent += callback; }
        static public void Unregister(Delegate callback) { OnEvent -= callback; }

        public delegate void Delegate(AnimationCurve intensity, float duration, float remapMin, float remapMax, bool relativeIntensity = false,
            float feedbacksIntensity = 1.0f, MMChannelData channelData = null, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true,
            bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false, bool restore = false);

        static public void Trigger(AnimationCurve intensity, float duration, float remapMin, float remapMax, bool relativeIntensity = false,
            float feedbacksIntensity = 1.0f, MMChannelData channelData = null, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true,
            bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false, bool restore = false)
        {
            OnEvent?.Invoke(intensity, duration, remapMin, remapMax, relativeIntensity, feedbacksIntensity, channelData, resetShakerValuesAfterShake, resetTargetValuesAfterShake, forwardDirection, timescaleMode, stop, restore);
        }
    }
}