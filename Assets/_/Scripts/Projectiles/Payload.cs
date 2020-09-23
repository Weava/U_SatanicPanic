using UnityEngine;

namespace Assets.Scripts.Projectiles
{
    /// <summary>
    /// Product of a projectile that provides damage and modifier data
    /// </summary>
    public class Payload : DamageSource
    {
        [SerializeField]
        protected float impactForce = 0;

        protected Vector3 sourcePosition;

        protected override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// Determines the trajectory the source came from, useful for raycasts
        /// </summary>
        public void SetSourcePosition(Vector3 position)
        {
            sourcePosition = position;
        }

        protected override void OnTriggerEnter(Collider _collider)
        {
            if (_collider.transform.GetComponent<Rigidbody>() != null)
            {
                if (sourcePosition == null) sourcePosition = transform.position;
                _collider.transform.GetComponent<Rigidbody>().AddForceAtPosition(sourcePosition * -1,
                    transform.position,
                    ForceMode.Impulse
                    );
            }

            base.OnTriggerEnter(_collider);
        }

        public float GetImpactForce()
        {
            return impactForce;
        }
    }
}
