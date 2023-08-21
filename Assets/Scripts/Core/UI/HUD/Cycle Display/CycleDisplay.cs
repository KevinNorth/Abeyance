using System;
using System.Collections;
using System.Globalization;
using KitTraden.Abeyance.Core.Cycles;
using Michsky.UI.MTP;
using TMPro;
using UnityEngine;
using MoreMountains.TopDownEngine;
using KitTraden.Abeyance.Core.Events;

namespace KitTraden
{
    public class CycleDisplay : GameOverListener
    {
        private static CultureInfo CURRENT_CULTURE_INFO = CultureInfo.CurrentUICulture;

        public TextMeshProUGUI CycleCounterText;
        public TextMeshProUGUI RemainingTimeText;
        /// See https://learn.microsoft.com/en-us/dotnet/api/system.string.format?view=net-7.0
        /// for documenation on acceptable format strings.
        public string CycleCounterTextFormat = "Cycle {0}";
        /// See https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-timespan-format-strings
        /// and https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-timespan-format-strings
        /// for documenation on acceptable format strings.
        public string RemainingTimeTextFormat = "{0:'<mspace=0.8em>'m'</mspace>'\\:'<mspace=0.8em>'ss'</mspace>'\\.'<mspace=0.8em>'ff'</mspace>'}";
        public string PausedRemainingTimeText = "<mspace=0.8em>-</mspace>:<mspace=0.8em>--</mspace>.<mspace=0.8em>--</mspace>";
        public StyleManager TurnoverAnnouncementUpper;
        public StyleManager TurnoverAnnouncementLower;
        [Range(0f, 10f)] public float TurnoverAnnouncementLeadTime = 0.5f;

        private CycleManager _cycleManager;
        private Camera _mainCamera;
        private LevelManager _levelManager;
        private bool _hasTurnoverAnnouncementStarted;
        private bool _hasGameOvered;
        private int _cycleCountAtGameOver;
        private float _remainingTimeAtGameOver;

        void Awake()
        {
            _cycleManager = GameObject.FindObjectOfType<CycleManager>();
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            _levelManager = GameObject.FindObjectOfType<LevelManager>();

            CycleCounterText.SetText(FormattedCycleCount(1));
            RemainingTimeText.SetText(FormattedRemainingTime(_cycleManager.DurationOfCurrentCycle()));

            _hasTurnoverAnnouncementStarted = false;
            _hasGameOvered = false;
        }

        void Update()
        {
            int currentCycle;
            float timeUntilNextCycle;

            if (_hasGameOvered)
            {
                currentCycle = _cycleCountAtGameOver;
                timeUntilNextCycle = _remainingTimeAtGameOver;
            }
            else
            {
                currentCycle = _cycleManager.GetCurrentCycle();
                timeUntilNextCycle = _cycleManager.TimeUntilNextCycle();
            }

            CycleCounterText.SetText(FormattedCycleCount(currentCycle));

            if (_cycleManager.IsPaused())
            {
                RemainingTimeText.SetText(PausedRemainingTimeText);
            }
            else
            {
                RemainingTimeText.SetText(FormattedRemainingTime(timeUntilNextCycle));
            }

            if (!_hasTurnoverAnnouncementStarted && timeUntilNextCycle <= TurnoverAnnouncementLeadTime)
            {
                PlayTurnoverAnnouncement();
                _hasTurnoverAnnouncementStarted = true;
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            _cycleManager.AddCycleTurnoverListener(OnCycleTurnover);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            _cycleManager.RemoveCycleTurnoverListener(OnCycleTurnover);
        }

        void OnCycleTurnover(int _nextCycle)
        {
            _hasTurnoverAnnouncementStarted = false;
        }

        public override void OnGameOver()
        {
            _hasGameOvered = true;
            _cycleCountAtGameOver = _cycleManager.GetCurrentCycle();
            _remainingTimeAtGameOver = _cycleManager.TimeUntilNextCycle();
        }

        public string FormattedCycleCount(int cycleCount)
        {
            return String.Format(CURRENT_CULTURE_INFO, CycleCounterTextFormat, cycleCount);
        }

        public string FormattedRemainingTime(float remainingTime)
        {
            var timeSpan = TimeSpan.FromSeconds(remainingTime);
            return String.Format(CURRENT_CULTURE_INFO, RemainingTimeTextFormat, timeSpan);
        }

        private void PlayTurnoverAnnouncement()
        {
            // As a safety measure, if for some reason there isn't a player object
            // in the scene, default to showing the upper turnover announcement.
            if (_levelManager.Players.Count == 0)
            {
                TurnoverAnnouncementUpper.gameObject.SetActive(true);
                return;
            }

            var player = _levelManager.Players[0];
            var playerCameraHeight = _mainCamera.WorldToScreenPoint(player.transform.position).y;
            var screenHeight = _mainCamera.pixelHeight;

            if (playerCameraHeight > screenHeight / 2 && playerCameraHeight < screenHeight)
            {
                TurnoverAnnouncementLower.gameObject.SetActive(true);
                return;
            }

            TurnoverAnnouncementUpper.gameObject.SetActive(true);
        }
    }
}