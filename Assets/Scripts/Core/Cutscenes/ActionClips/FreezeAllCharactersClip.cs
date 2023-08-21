using System.Collections.Generic;
using UnityEngine;
using Slate;
using MoreMountains.TopDownEngine;
using MMCharacter = MoreMountains.TopDownEngine.Character;

namespace KitTraden.Abeyance.Core.Cutscenes.ActionClips
{
    [Category("TopDown Engine")]
    public class FreezeAllCharactersClip : DirectorActionClip
    {
        public bool MakeInvulnerableToDamage = true;

        private Dictionary<MMCharacter, bool> originalWasFrozen;
        private Dictionary<Health, bool> originalWasInvulnerable;

        //This is written within the clip in the editor
        public override string info
        {
            get
            {
                return "Freeze All Characters";
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
            ResetDictionaries();
            return true;
        }

        //Called in forward sampling when the clip is entered
        protected override void OnEnter()
        {
            var characters = GameObject.FindObjectsOfType<MMCharacter>();

            foreach (var character in characters)
            {
                FreezeCharacter(character);
            }
        }

        //Called per frame while the clip is updating. Time is the local time within the clip.
        //So a time of 0, means the start of the clip.
        protected override void OnUpdate(float time) { }

        //Called in forwards sampling when the clip exits
        protected override void OnExit()
        {
            var characters = GameObject.FindObjectsOfType<MMCharacter>();

            foreach (var character in characters)
            {
                UnfreezeCharacter(character);
            }

            ResetDictionaries();
        }

        //Called in backwards sampling when the clip is entered.
        protected override void OnReverseEnter()
        {
            var characters = GameObject.FindObjectsOfType<MMCharacter>();

            foreach (var character in characters)
            {
                FreezeCharacter(character);
            }
        }

        //Called in backwards sampling when the clip exits.
        protected override void OnReverse()
        {
            var characters = GameObject.FindObjectsOfType<MMCharacter>();

            foreach (var character in characters)
            {
                UnfreezeCharacter(character);
            }

            ResetDictionaries();
        }

        private void ResetDictionaries()
        {
            originalWasFrozen = new Dictionary<MMCharacter, bool>();
            originalWasInvulnerable = new Dictionary<Health, bool>();
        }

        private void FreezeCharacter(MMCharacter character)
        {
            if (character == null)
            {
                return;
            }

            originalWasFrozen.Add(character, IsFrozen(character));
            character.Freeze();

            if (MakeInvulnerableToDamage)
            {
                var health = GetHealth(character);

                if (health)
                {
                    originalWasInvulnerable.Add(health, IsInvulnerable(health));
                    health.Invulnerable = true;
                }
            }
        }

        private void UnfreezeCharacter(MMCharacter character)
        {
            if (character == null)
            {
                return;
            }

            if (!originalWasFrozen[character])
            {
                character.UnFreeze();
            }

            if (MakeInvulnerableToDamage)
            {
                var health = GetHealth(character);

                if (health)
                {
                    health.Invulnerable = originalWasInvulnerable[health];
                }
            }
        }

        private Health GetHealth(MMCharacter character)
        {
            var health = character.gameObject.GetComponent<Health>();
            if (health == null)
            {
                health = actor.gameObject.GetComponentInChildren<Health>();
            }

            return health;
        }

        private bool IsFrozen(MMCharacter character)
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