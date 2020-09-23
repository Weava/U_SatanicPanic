using UnityEngine;

namespace Assets.Scripts.Misc
{
    public class Rotation : MonoBehaviour
    {
        public float rotationRate;

        private void Update()
        {
            var rate = Time.deltaTime * rotationRate;
            gameObject.transform.Rotate(Vector3.up, rate);
        }
    }
}