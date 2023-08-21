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
    /// Add this class to a Camera with a Limitless Glitch 7 post processing and it'll be able to "shake" its values by getting events
    /// </summary>
    [AddComponentMenu("More Mountains/Feedbacks/Shakers/PostProcessing/Limitless Glitch/MMLimitlessGlitch7Shaker")]
#if MM_POSTPROCESSING
	[RequireComponent(typeof(PostProcessVolume))]
#endif
    public class MMLimitlessGlitch7Shaker : MMShaker
    {
        [MMInspectorGroup("Limitless Glitch 7 Intensity", true, 46)]
        /// whether or not to add to the initial value
        [Tooltip("whether or not to add to the initial value")]
        public bool RelativeIntensity = false;
        /// the curve used to animate the intensity value on
        [Tooltip("the curve used to animate the fade value on")]
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
        protected LimitlessGlitch7 _limitlessGlitch7;
        protected float _initialFade;
        protected float _originalShakeDuration;
        protected AnimationCurve _originalShakeFade;
        protected float _originalRemapFadeZero;
        protected float _originalRemapFadeOne;
        protected bool _originalFadeIntensity;

        /// <summary>
        /// On init we initialize our values
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();

            _volume = this.gameObject.GetComponent<PostProcessVolume>();
            _volume.profile.TryGetSettings(out _limitlessGlitch7);
        }

        /// <summary>
        /// Shakes values over time
        /// </summary>
        protected override void Shake()
        {
            float newValue = ShakeFloat(ShakeFade, RemapFadeZero, RemapFadeOne, RelativeIntensity, _initialFade);
            _limitlessGlitch7.Fade.Override(newValue);
        }

        /// <summary>
        /// Collects initial values on the target
        /// </summary>
        protected override void GrabInitialValues()
        {
            _initialFade = _limitlessGlitch7.Fade;
        }

        /// <summary>
        /// When we get the appropriate event, we trigger a shake
        /// </summary>
        /// <param name="fade"></param>
        /// <param name="duration"></param>
        /// <param name="amplitude"></param>
        /// <param name="relativeIntensity"></param>
        /// <param name="feedbacksFade"></param>
        /// <param name="channel"></param>
        public virtual void OnMMLimitlessGlitch7ShakeEvent(AnimationCurve fade, float duration, float remapMin, float remapMax, bool relativeIntensity = false,
            float feedbacksFade = 1.0f, MMChannelData channelData = null, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true,
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
                _originalShakeFade = ShakeFade;
                _originalRemapFadeZero = RemapFadeZero;
                _originalRemapFadeOne = RemapFadeOne;
                _originalFadeIntensity = RelativeIntensity;
            }

            TimescaleMode = timescaleMode;
            ShakeDuration = duration;
            ShakeFade = fade;
            RemapFadeZero = remapMin * feedbacksFade;
            RemapFadeOne = remapMax * feedbacksFade;
            RelativeIntensity = relativeIntensity;
            ForwardDirection = forwardDirection;

            Play();
        }

        /// <summary>
        /// Resets the target's values
        /// </summary>
        protected override void ResetTargetValues()
        {
            base.ResetTargetValues();
            _limitlessGlitch7.Fade.Override(_initialFade);
        }

        /// <summary>
        /// Resets the shaker's values
        /// </summary>
        protected override void ResetShakerValues()
        {
            base.ResetShakerValues();
            ShakeDuration = _originalShakeDuration;
            ShakeFade = _originalShakeFade;
            RemapFadeZero = _originalRemapFadeZero;
            RemapFadeOne = _originalRemapFadeOne;
            RelativeIntensity = _originalFadeIntensity;
        }

        /// <summary>
        /// Starts listening for events
        /// </summary>
        public override void StartListening()
        {
            base.StartListening();
            MMLimitlessGlitch7ShakeEvent.Register(OnMMLimitlessGlitch7ShakeEvent);
        }

        /// <summary>
        /// Stops listening for events
        /// </summary>
        public override void StopListening()
        {
            base.StopListening();
            MMLimitlessGlitch7ShakeEvent.Unregister(OnMMLimitlessGlitch7ShakeEvent);
        }
    }

    /// <summary>
    /// An event used to trigger vignette shakes
    /// </summary>
    public struct MMLimitlessGlitch7ShakeEvent
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