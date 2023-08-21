using System;
using UnityEngine;
using Cinemachine;
using Sirenix.OdinInspector;
using PixelPerfectCamera = UnityEngine.U2D.PixelPerfectCamera;
using UnityCamera = UnityEngine.Camera;

namespace KitTraden.Abeyance.Core.Camera
{
    /// <summary>
    /// Allows Cameras with PixelPerfectCamera components to zoom smoothly
    /// by disabling the PixelPerfectCameras when zooming.
    /// This allows some visual artifacts to appear in exchange for perfectly
    /// smooth zooming.
    /// </summary>
    /// <seealso cref="KitTraden.Abeyance.Core.Camera.PixelPerfectZoom"/>
    public class PixelImperfectZoom : MonoBehaviour
    {
        public UnityCamera UnityCamera;
        public CinemachineVirtualCamera CinemachineCamera;
        public CinemachinePixelPerfect CinemachinePixelPerfect;
        public PixelPerfectZoom CompetingZoom;
        [Tooltip("Change OrthographicSize in scripts to change zoom level. The initial value is set on Start, so you can't set it in the inspector.")]
        [ReadOnly][Min(0)] public float OrthographicSize = 1;

        [ReadOnly]
        [ShowInInspector]
        private float initialCameraSize;
        [ReadOnly]
        [ShowInInspector]
        private PixelPerfectCamera unityPixelPerfect;
        private GameObject unityPixelPerfectParent;

        void Start()
        {
            CheckCameras();

            if (CinemachineCamera == null)
            {
                initialCameraSize = UnityCamera.orthographicSize;
            }
            else
            {
                initialCameraSize = CinemachineCamera.m_Lens.OrthographicSize;
                CinemachinePixelPerfect.enabled = true;
            }

            OrthographicSize = initialCameraSize;
            unityPixelPerfect.enabled = true;
        }

        void Update()
        {
            CheckCameras();

            if (CompetingZoom != null && CompetingZoom.GetZoom() != 1)
            {
                return;
            }

            bool enablePixelPerfects = OrthographicSize == initialCameraSize;

            if (CinemachineCamera == null)
            {
                UnityCamera.orthographicSize = OrthographicSize;
            }
            else
            {
                CinemachineCamera.m_Lens.OrthographicSize = OrthographicSize;
                CinemachinePixelPerfect.enabled = enablePixelPerfects;
            }

            Debug.Log("UnityPixelPerfect:");
            Debug.Log(unityPixelPerfect);

            unityPixelPerfect.enabled = enablePixelPerfects;
        }

        public void SetZoom(float zoom)
        {
            if (zoom <= 0f)
            {
                throw new ArgumentException("Camera zoom must be positive");
            }

            OrthographicSize = initialCameraSize / zoom;
        }

        public float GetZoom()
        {
            return initialCameraSize / OrthographicSize;
        }

        private void CheckCameras()
        {
            if (CinemachineCamera == null && UnityCamera == null)
            {
                throw new Exception("PixelPerfectZoom needs a Unity Camera or a CinemachineVirtualCamera");
            }

            if (CinemachineCamera != null && UnityCamera != null)
            {
                throw new Exception("PixelPerfectZoom needs exactly one of either a Unity Camera or a CinemachineVirtualCamera");
            }

            if (CinemachineCamera != null && CinemachinePixelPerfect == null)
            {
                throw new Exception("When using PixelImperfectZoom on a CinemachineVirtualCamera, you must set the CinemachinePixelPerfect");
            }

            if (UnityCamera != null && CinemachinePixelPerfect != null)
            {
                throw new Exception("When using PixelImperfectZoom on a Unity Camera, you must leave the CinemachinePixelPerfect as null");
            }

            if (unityPixelPerfectParent == null)
            {
                unityPixelPerfectParent = GameObject.FindWithTag("MainCamera");
            }

            if (unityPixelPerfect == null)
            {
                unityPixelPerfect = unityPixelPerfectParent.GetComponent<PixelPerfectCamera>();
            }
        }

        [Min(0)] public float DebugZoomLevel = 1;
        [Button]
        public void SetZoomLevelAtRuntime()
        {
            SetZoom(DebugZoomLevel);
        }
    }
}