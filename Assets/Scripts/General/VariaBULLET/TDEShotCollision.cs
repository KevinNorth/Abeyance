using System;
using System.Collections;
using UnityEngine;
using MoreMountains.TopDownEngine;
using ND_VariaBULLET;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;

namespace KitTraden.Abeyance.VariaBULLET
{
    public class TDEShotCollision : MonoBehaviour, IShotCollidable
    {
        [Tooltip("The Health that should take damage as a result of the collision")]
        public Health Health;

        [Tooltip("The feedbacks to play when hit by either a laser or a bullet")]
        public MMFeedbacks hitFeedbacks;

        [Tooltip("The feedbacks to play when specifically hit by a bullet")]
        public MMFeedbacks bulletFeedbacks;

        [Tooltip("The feedbacks to play when specifically hit by a laser")]
        public MMFeedbacks laserFeedbacks;

        [Tooltip("Sets which collision layers can produce explosions when colliding with this object.")]
        public string[] CollisionList;

        private IEnumerable SelectableLayers = new ValueDropdownList<int>()
        {
            { "PlayerBulletTarget", 24 },
            { "EnemyBulletTarget", 25 },
            { "Obstacles", 8 },
            { "BulletOnlyObstacle", 26 }
        };
        [Tooltip("Sets which collision layer this component's GameObject should be on while the Health is alive")]
        [ValueDropdown("SelectableLayers")]
        public int Layer = 25;

        [Tooltip("Whether to recursively apply the layer to all children")]
        public bool ApplyLayerRecursively = true;

        [Tooltip("Sets the name of the explosion prefab that's instantiated upon laser collision. [Note: prefab must also be pre-loaded in GlobalShotManager.ExplosionPrefabs].")]
        public string LaserExplosion;

        [Tooltip("Sets the name of the explosion prefab that's instantiated upon bullet collision. [Note: prefab must also be pre-loaded in GlobalShotManager.ExplosionPrefabs].")]
        public string BulletExplosion;

        [Tooltip("Sets whether or not explosion moves with this object or remains at point of impact.")]
        public bool ParentExplosion = true;

        [Range(0f, 5f)]
        [Tooltip("Sets the duration (in seconds) for the damage flicker and invincibility effect upon collision.")]
        public float InvulnDuration = 0f;

        private bool isAlive = false;

        void OnEnable()
        {
            Health.OnDeath += OnDeath;
            Health.OnRevive += OnRevive;
        }

        void OnDisable()
        {
            Health.OnDeath -= OnDeath;
            Health.OnRevive -= OnRevive;
        }

        void Update()
        {
            if (isAlive && gameObject.layer != Layer)
            {
                gameObject.layer = Layer;

                if (ApplyLayerRecursively)
                {
                    gameObject.transform.ChangeLayersRecursively(Layer);
                }
            }
        }

        private void OnDeath()
        {
            isAlive = false;
        }

        private void OnRevive()
        {
            isAlive = true;
        }

        public IEnumerator OnLaserCollision(CollisionArgs sender)
        {
            if (CollisionFilter.collisionAccepted(sender.gameObject.layer, CollisionList) && !CalcObject.IsOutBounds(sender.point))
            {
                var direction = CalculateDamageDirection(sender.point);
                Health.Damage(sender.damage, sender.gameObject, InvulnDuration, InvulnDuration, direction);

                hitFeedbacks?.PlayFeedbacks();
                laserFeedbacks?.PlayFeedbacks();

                CollisionFilter.setExplosion(LaserExplosion, ParentExplosion, this.transform, new Vector2(sender.point.x, sender.point.y), 0, this);
                yield return null;
            }
        }

        public IEnumerator OnCollisionEnter2D(Collision2D collision)
        {
            var iDamager = collision.gameObject.GetComponent<IDamager>();

            if (iDamager != null && CollisionFilter.collisionAccepted(collision.gameObject.layer, CollisionList) && !CalcObject.IsOutBounds(collision.contacts[0].point))
            {
                var direction = (collision.transform.position - transform.position).normalized;
                Health.Damage(iDamager.DMG, collision.gameObject, InvulnDuration, InvulnDuration, direction);

                hitFeedbacks?.PlayFeedbacks();
                bulletFeedbacks?.PlayFeedbacks();

                CollisionFilter.setExplosion(BulletExplosion, ParentExplosion, this.transform, collision.contacts[0].point, 0, this);
                yield return null;
            }
        }

        private Vector3 CalculateDamageDirection(Vector2 pointOfDamage)
        {
            Vector2 position2d = new Vector2(transform.position.x, transform.position.y);
            Vector2 direction = (pointOfDamage - position2d).normalized;
            return new Vector3(direction.x, direction.y, 0);
        }
    }
}