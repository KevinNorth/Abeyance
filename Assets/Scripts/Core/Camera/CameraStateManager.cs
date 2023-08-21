using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;
using WeaponStates = MoreMountains.TopDownEngine.Weapon.WeaponStates;

namespace KitTraden.Abeyance.Core.Camera
{
    public class CameraStateManager : MonoBehaviour, MMEventListener<MMGameEvent>, MMEventListener<MMStateChangeEvent<CharacterStates.MovementStates>>, MMEventListener<MMStateChangeEvent<WeaponStates>>
    {
        protected static WeaponStates[] USING_WEAPON_STATES = new WeaponStates[] {
            WeaponStates.WeaponDelayBeforeUse,
            WeaponStates.WeaponDelayBetweenUses,
            WeaponStates.WeaponStart,
            WeaponStates.WeaponUse
        };

        [SerializeField] Animator animator;
        private LevelManager levelManager;
        private Character player = null;

        void Start()
        {
            levelManager = GameObject.FindObjectOfType<LevelManager>();
        }

        // Start is called before the first frame update
        void Update()
        {
            // Can't add the player during Start() because the LevelManager needs to
            // be guaraneed to finish intializing itself first.
            if (player == null)
            {
                InitializePlayer();
            }
        }

        public virtual void OnEnable()
        {
            this.MMEventStartListening<MMGameEvent>();
            this.MMEventStartListening<MMStateChangeEvent<CharacterStates.MovementStates>>();
            this.MMEventStartListening<MMStateChangeEvent<WeaponStates>>();
        }

        public virtual void OnDisable()
        {
            this.MMEventStopListening<MMGameEvent>();
            this.MMEventStopListening<MMStateChangeEvent<CharacterStates.MovementStates>>();
            this.MMEventStopListening<MMStateChangeEvent<WeaponStates>>();
        }

        private void InitializePlayer()
        {
            if (levelManager.Players.Count > 0)
            {
                player = levelManager.Players[0];
            }
        }

        public void OnMMEvent(MMGameEvent gameEvent)
        {
            var parameter = animator.parameters.FirstOrDefault(
                (parameter) =>
                    parameter.type == AnimatorControllerParameterType.Trigger
                        && parameter.name == gameEvent.EventName
            );

            if (parameter != null)
            {
                animator.SetTrigger(parameter.name);
            }
        }

        public void OnMMEvent(MMStateChangeEvent<CharacterStates.MovementStates> movementEvent)
        {
            if (player == null || movementEvent.Target != player.gameObject)
            {
                return;
            }

            animator.SetBool("Dashing", movementEvent.NewState == CharacterStates.MovementStates.Dashing);
            animator.SetBool("FallingDownHole", movementEvent.NewState == CharacterStates.MovementStates.FallingDownHole);
            animator.SetBool("Idling", movementEvent.NewState == CharacterStates.MovementStates.Idle);
            animator.SetBool("SlowMoving", movementEvent.NewState == CharacterStates.MovementStates.MovingSlowly);
            animator.SetBool("Running", movementEvent.NewState == CharacterStates.MovementStates.Running);
            animator.SetBool("Walking", movementEvent.NewState == CharacterStates.MovementStates.Walking);
        }

        public void OnMMEvent(MMStateChangeEvent<WeaponStates> weaponEvent)
        {
            var handleWeaponAbility = player.FindAbility<CharacterHandleWeapon>();

            if (player == null || handleWeaponAbility == null)
            {
                return;
            }

            var weapon = handleWeaponAbility.CurrentWeapon;

            if (weapon == null)
            {
                return;
            }

            if (weaponEvent.Target != weapon.gameObject)
            {
                return;
            }

            animator.SetBool("UsingWeapon", USING_WEAPON_STATES.Contains(weaponEvent.NewState));
        }
    }
}