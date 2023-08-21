using UnityEngine;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using KitTraden.Abeyance.Core.Camera;

namespace KitTraden.Abeyance.TopDownEngine.Feedbacks
{
    /// <summary>
    /// Add this class to a Cinemachine Virtual Camera with a Pixel Perfect Zoom and it will be able to "shake" its zoom by getting events
    /// </summary>
    [AddComponentMenu("More Mountains/Feedbacks/Shakers/Camera/MMPixelPerfectZoomShaker")]
    public class MMPixelPerfectZoomShaker : MMShaker
    {
        [MMInspectorGroup("Zoom Level", true, 46)]
        /// whether or not to add to the initial value
        [Tooltip("whether or not to add to the initial value")]
        public bool RelativeZoom = false;
        /// the curve used to animate the intensity value on
        [Tooltip("the curve used to animate the intensity value on")]
        public AnimationCurve ShakeIntensity = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        /// the value to remap the curve's 0 to
        [Tooltip("the value to remap the curve's 0 to")]
        [Min(0f)]
        public float RemapZoomZero = 0.5f;
        /// the value to remap the curve's 1 to
        [Tooltip("the value to remap the curve's 1 to")]
        [Min(0f)]
        public float RemapZoomOne = 1f;

        protected PixelPerfectZoom _pixelPerfectZoom;
        protected float _initialZoom;
        protected float _originalZoomDuration;
        protected AnimationCurve _originalShakeZoom;
        protected float _originalRemapZoomZero;
        protected float _originalRemapZoomOne;
        protected bool _originalRelativeZoom;

        /// <summary>
        /// On init we initialize our values
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            _pixelPerfectZoom = gameObject.GetComponent<PixelPerfectZoom>();
        }

        /// <summary>
        /// Shakes values over time
        /// </summary>
        protected override void Shake()
        {
            float newValue = ShakeFloat(ShakeIntensity, RemapZoomZero, RemapZoomOne, RelativeZoom, _initialZoom);
            _pixelPerfectZoom.SetZoom(newValue);
        }

        /// <summary>
        /// Collects initial values on the target
        /// </summary>
        protected override void GrabInitialValues()
        {
            _initialZoom = _pixelPerfectZoom.GetZoom();
        }

        /// <summary>
        /// When we get the appropriate event, we trigger a shake
        /// </summary>
        /// <param name="zoom"></param>
        /// <param name="duration"></param>
        /// <param name="amplitude"></param>
        /// <param name="relativeZoom"></param>
        /// <param name="feedbacksIntensity"></param>
        /// <param name="channel"></param>
        public virtual void OnMMPixelPerfectZoomShakeEvent(AnimationCurve zoom, float duration, float remapMin, float remapMax, bool relativeZoom = false,
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
                _originalZoomDuration = ShakeDuration;
                _originalShakeZoom = ShakeIntensity;
                _originalRemapZoomZero = RemapZoomZero;
                _originalRemapZoomOne = RemapZoomOne;
                _originalRelativeZoom = RelativeZoom;
            }

            TimescaleMode = timescaleMode;
            ShakeDuration = duration;
            ShakeIntensity = zoom;
            RemapZoomZero = remapMin * feedbacksIntensity;
            RemapZoomOne = remapMax * feedbacksIntensity;
            RelativeZoom = relativeZoom;
            ForwardDirection = forwardDirection;

            Play();
        }

        /// <summary>
        /// Resets the target's values
        /// </summary>
        protected override void ResetTargetValues()
        {
            base.ResetTargetValues();
            _pixelPerfectZoom.SetZoom(_initialZoom);
        }

        /// <summary>
        /// Resets the shaker's values
        /// </summary>
        protected override void ResetShakerValues()
        {
            base.ResetShakerValues();
            ShakeDuration = _originalZoomDuration;
            ShakeIntensity = _originalShakeZoom;
            RemapZoomZero = _originalRemapZoomZero;
            RemapZoomOne = _originalRemapZoomOne;
            RelativeZoom = _originalRelativeZoom;
        }

        /// <summary>
        /// Starts listening for events
        /// </summary>
        public override void StartListening()
        {
            base.StartListening();
            MMPixelPerfectZoomShakeEvent.Register(OnMMPixelPerfectZoomShakeEvent);
        }

        /// <summary>
        /// Stops listening for events
        /// </summary>
        public override void StopListening()
        {
            base.StopListening();
            MMPixelPerfectZoomShakeEvent.Unregister(OnMMPixelPerfectZoomShakeEvent);
        }
    }

    /// <summary>
    /// An event used to trigger vignette shakes
    /// </summary>
    public struct MMPixelPerfectZoomShakeEvent
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