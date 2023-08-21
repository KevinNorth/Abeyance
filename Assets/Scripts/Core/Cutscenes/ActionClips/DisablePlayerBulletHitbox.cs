using UnityEngine;
using Slate;
using MoreMountains.TopDownEngine;

namespace KitTraden.Abeyance.Core.Cutscenes.ActionClips
{
    [Category("TopDown Engine")]
    public class DisablePlayerBulletHitbox : DirectorActionClip
    {
        [SerializeField] public LevelManager levelManager;

        //This is written within the clip in the editor
        public override string info
        {
            get
            {
                return "Disable Player Bullet Hitbox";
            }
        }

        //The length of the clip. Both get/set are optional. No override means the clip won't be scaleable.
        [SerializeField]
        [HideInInspector]
        private float _length = 1;

        public override float length
        {
            get { return _length; }
            set { _length = value; }
        }

        //Called once when the cutscene initialize. Return true if init was successful, or false if not
        protected override bool OnInitialize()
        {
            if (levelManager == null)
            {
                levelManager = GameObject.FindObjectOfType<LevelManager>();
            }

            return levelManager != null && levelManager.Players.Count >= 1;
        }

        //Called in forward sampling when the clip is entered
        protected override void OnEnter()
        {
            SetPlayerBulletTargetsActive(false);
        }

        //Called in forwards sampling when the clip exits
        protected override void OnExit()
        {
            SetPlayerBulletTargetsActive(true);
        }

        //Called in backwards sampling when the clip is entered.
        protected override void OnReverseEnter()
        {
            SetPlayerBulletTargetsActive(false);
        }

        protected override void OnReverse()
        {
            SetPlayerBulletTargetsActive(true);
        }

        private void SetPlayerBulletTargetsActive(bool enabled)
        {
            var player = levelManager.Players[0];

            foreach (Transform child in player.transform)
            {
                if (child.gameObject.layer == LayerMask.NameToLayer("PlayerBulletTarget"))
                {
                    var rigidBody = child.gameObject.GetComponent<Rigidbody2D>();

                    if (rigidBody != null)
                    {
                        rigidBody.simulated = enabled;
                    }
                }
            }
        }
    }
}