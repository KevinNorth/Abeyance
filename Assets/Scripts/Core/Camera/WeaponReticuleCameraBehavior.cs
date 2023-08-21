using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;
using WeaponStates = MoreMountains.TopDownEngine.Weapon.WeaponStates;
using System.Linq;

namespace KitTraden.Abeyance.Core.Camera
{
    public class WeaponReticuleCameraBehavior : MonoBehaviour, MMEventListener<MMStateChangeEvent<WeaponStates>>
    {
        protected static WeaponStates[] USING_WEAPON_STATES = new WeaponStates[] {
            WeaponStates.WeaponDelayBeforeUse,
            WeaponStates.WeaponDelayBetweenUses,
            WeaponStates.WeaponStart,
            WeaponStates.WeaponUse
        };

        [Range(0f, 1f)] public float MaxTargetOffset = 0.5f;
        public AnimationCurve TargetOffsetCurve = AnimationCurve.Linear(0, 0, 1, 1);

        private Weapon weapon;
        private WeaponAim2D weaponAim;
        private bool isWeaponBeingUsed = false;

        void Start()
        {
            Init();
        }

        void Update()
        {
            if (isWeaponBeingUsed)
            {
                var curvePosition = weapon.Owner.LinkedInputManager.SecondaryMovement.magnitude;
                var curveValue = TargetOffsetCurve.Evaluate(curvePosition);
                weaponAim.CameraTargetOffset = curveValue * MaxTargetOffset;
            }
            else
            {
                weaponAim.CameraTargetOffset = 0;
            }
        }

        void OnEnable()
        {
            Init();
            this.MMEventStartListening<MMStateChangeEvent<WeaponStates>>();
        }

        void OnDisable()
        {
            this.MMEventStopListening<MMStateChangeEvent<WeaponStates>>();
        }

        private void Init()
        {
            isWeaponBeingUsed = false;
            weapon = gameObject.GetComponent<Weapon>();
            weaponAim = gameObject.GetComponent<WeaponAim2D>();
        }

        public void OnMMEvent(MMStateChangeEvent<WeaponStates> weaponEvent)
        {
            if (weaponEvent.Target != weapon.gameObject)
            {
                return;
            }

            isWeaponBeingUsed = USING_WEAPON_STATES.Contains(weaponEvent.NewState);
        }
    }
}