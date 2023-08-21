using System;
using UnityEngine;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityCamera = UnityEngine.Camera;
using PixelPerfectCamera = UnityEngine.U2D.PixelPerfectCamera;

namespace KitTraden.Abeyance.Core.Camera
{
    /// <summary>
    /// Allows Cameras with PixelPerfectCamera components to zoom smoothly
    /// by changing the pixels per unit values of the PixelPerfectCameras.
    /// This allows zooming with no visual artifacts, but zooming can't be
    /// smooth. This is because PixelPerfectCameras use whole integers for
    /// their pixels per unit values (they need to in order to function as
    /// intended), so each possible PPU is a discreet possible zoom value.
    /// </summary>
    /// <seealso cref="KitTraden.Abeyance.Core.Camera.CinmachinePixelImperfectZoom"/>
    public class PixelPerfectZoom : MonoBehaviour
    {
        public CinemachineVirtualCamera CinemachineCamera;
        public UnityCamera UnityCamera;
        public PixelImperfectZoom CompetingZoom;

        [ReadOnly]
        [ShowInInspector]
        private float initialCameraSize;
        [ReadOnly]
        [ShowInInspector]
        private int initialPixelsPerUnit;
        [ReadOnly]
        [ShowInInspector]
        private int pixelsPerUnit;
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
            }

            initialPixelsPerUnit = unityPixelPerfect.assetsPPU;

            pixelsPerUnit = initialPixelsPerUnit;
        }

        void Update()
        {
            CheckCameras();

            if (CompetingZoom != null && CompetingZoom.GetZoom() != 1)
            {
                return;
            }

            unityPixelPerfect.assetsPPU = GetPixelsPerUnit();

            if (CinemachineCamera == null)
            {
                UnityCamera.orthographicSize = GetOrthographicSize();
            }
            else
            {
                CinemachineCamera.m_Lens.OrthographicSize = GetOrthographicSize();
            }
        }

        /// <returns>The exact zoom ratio applied. Since PixelPerfectCameras
        /// strictly use whole integers for their Pixel Per Unit values, the
        /// exact zoom passed in might not be possible to use; if so, this
        /// function will round to the nearest viable ratio, use that, and
        /// return it.</returns>
        public float SetZoom(float zoom)
        {
            if (zoom <= 0f)
            {
                throw new ArgumentException("Camera zoom must be positive");
            }

            pixelsPerUnit = Mathf.Max(Mathf.RoundToInt(initialPixelsPerUnit * zoom), 1);
            return GetZoom();
        }

        public float GetZoom()
        {
            return (float)pixelsPerUnit / (float)initialPixelsPerUnit;
        }

        public int GetPixelsPerUnit()
        {
            return pixelsPerUnit;
        }

        public void SetPixelsPerUnit(int newPixelsPerUnit)
        {
            if (newPixelsPerUnit <= 0)
            {
                throw new ArgumentException("Camera zoom must be positive");
            }

            pixelsPerUnit = newPixelsPerUnit;
        }

        /// <returns>The exact orthographic size applied. Since PixelPerfectCameras
        /// strictly use whole integers for their Pixel Per Unit values, the
        /// exact ortho size passed in might not be possible to use; if so, this
        /// function will round to the nearest viable size, use that, and
        /// return it.</returns>
        public float SetOrthographicSize(float orthographicSize)
        {
            if (orthographicSize <= 0)
            {
                throw new ArgumentException("Camera zoom must be positive");
            }

            float zoom = initialCameraSize / orthographicSize;
            SetZoom(zoom);

            return GetOrthographicSize();
        }

        public float GetOrthographicSize()
        {
            return initialCameraSize / GetZoom();
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