using UnityEngine;
using Slate;
using Slate.ActionClips;
using MoreMountains.TopDownEngine;

namespace KitTraden.Abeyance.Core.Cutscenes.ActionClips
{
    [Category("TopDown Engine")]
    public class FreezeCharacterClip : ActorActionClip
    {
        public bool MakeInvulnerableToDamage = true;

        private bool originalWasFrozen = false;
        private bool originalWasInvulnerable = false;

        //This is written within the clip in the editor
        public override string info
        {
            get
            {
                return "Freeze Character";
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
            return GetCharacter() != null;
        }

        //Called in forward sampling when the clip is entered
        protected override void OnEnter()
        {
            FreezeCharacter();
        }

        //Called per frame while the clip is updating. Time is the local time within the clip.
        //So a time of 0, means the start of the clip.
        protected override void OnUpdate(float time) { }

        //Called in forwards sampling when the clip exits
        protected override void OnExit()
        {
            UnfreezeCharacter();
        }

        //Called in backwards sampling when the clip is entered.
        protected override void OnReverseEnter()
        {
            FreezeCharacter();
        }

        //Called in backwards sampling when the clip exits.
        protected override void OnReverse()
        {
            UnfreezeCharacter();
        }

        private void FreezeCharacter()
        {
            var character = GetCharacter();
            originalWasFrozen = IsFrozen(character);
            character?.Freeze();

            if (MakeInvulnerableToDamage)
            {
                var health = GetHealth();

                if (health)
                {
                    originalWasInvulnerable = IsInvulnerable(health);
                    health.Invulnerable = true;
                }
            }
        }

        private void UnfreezeCharacter()
        {
            if (!originalWasFrozen)
            {
                GetCharacter()?.UnFreeze();
            }

            if (MakeInvulnerableToDamage)
            {
                var health = GetHealth();

                if (health)
                {
                    health.Invulnerable = originalWasInvulnerable;
                }
            }
        }

        private MoreMountains.TopDownEngine.Character GetCharacter()
        {
            var character = actor.gameObject.GetComponent<MoreMountains.TopDownEngine.Character>();
            if (character == null)
            {
                character = actor.gameObject.GetComponentInChildren<MoreMountains.TopDownEngine.Character>();
            }

            return character;
        }

        private Health GetHealth()
        {
            var health = actor.gameObject.GetComponent<Health>();
            if (health == null)
            {
                health = actor.gameObject.GetComponentInChildren<Health>();
            }

            return health;
        }

        private bool IsFrozen(MoreMountains.TopDownEngine.Character character)
        {
            if (character == null)
            {
                return false;
            }

            return character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Frozen;
        }

        private bool IsInvulnerable(Health health)
        {
            return health?.Invulnerable ?? false;
        }
    }
}