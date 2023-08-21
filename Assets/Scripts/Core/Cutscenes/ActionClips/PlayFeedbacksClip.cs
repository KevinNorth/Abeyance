using System;
using System.Linq;
using MoreMountains.Feedbacks;
using Slate;
using UnityEngine;

namespace KitTraden.Abeyance.Core.Cutscenes.ActionClips
{
    [Category("TopDown Engine")]
    public class PlayFeedbacksClip : DirectorActionClip
    {
        public MMFeedbacks Feedbacks;

        public override string info
        {
            get
            {
                if (Feedbacks == null)
                {
                    return "MMFeedbacks";
                }

                var str = "";
                foreach (var feedback in Feedbacks.Feedbacks)
                {
                    str += $"{feedback.Label} ";
                }

                return str.TrimEnd();
            }
        }

        protected override bool OnInitialize()
        {
            return true;
        }

        protected override void OnEnter()
        {
            Feedbacks.PlayFeedbacks();
        }

        protected override void OnReverseEnter()
        {
            Feedbacks.PlayFeedbacksInReverse();
        }
    }
}