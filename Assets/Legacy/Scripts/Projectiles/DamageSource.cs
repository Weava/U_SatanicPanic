using UnityEngine;

namespace Assets.Scripts.Projectiles
{
    public class DamageSource : MonoBehaviour
    {
        [SerializeField]
        protected float impactDamage;

        protected Collider collider;

        // Use this for initialization
        protected virtual void Start()
        {
            collider = GetComponent<Collider>();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
        }

        protected virtual void OnTriggerEnter(Collider _collider)
        {
        }

        public Collider GetCollider()
        {
            return collider;
        }

        public float GetImpactDamage()
        {
            return -impactDamage;
        }
    }
}