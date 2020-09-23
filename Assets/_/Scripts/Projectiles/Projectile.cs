using UnityEngine;

namespace Assets.Scripts.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        protected bool isRaycast;

        [SerializeField]
        protected float range;

        [SerializeField]
        protected Payload payload;

        private Ray rayDirection;

        //Audio references - Loop for ballistic projectiles

        //Trail object

        // Update is called once per frame
        protected void Update()
        {
            if (isRaycast)
            {
                UpdateRaycast();
            }
        }

        protected void DeployPayload(Vector3 position, Quaternion rotation)
        {
            Instantiate(payload, position, rotation);
        }

        /// <summary>
        /// Don't shoot yourself
        /// </summary>
        public void SetParentActorCollidor(Collider collider)
        {

        }

        public void SetTarget(Vector3 targetPoint)
        {
            transform.LookAt(targetPoint);
        }

        private void UpdateRaycast()
        {       
            RaycastHit hit;
             var hasHit = Physics.Raycast(transform.position, transform.forward, out hit, range);

            if (hasHit)
            {
                DeployPayload(hit.point, hit.transform.rotation);
            }

            Destroy(gameObject);
        }
    }
}
