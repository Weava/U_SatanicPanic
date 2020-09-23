using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Misc
{
    public class Rotation : MonoBehaviour
    {
        public float rotationRate;

        void Update()
        {
            var rate = Time.deltaTime * rotationRate;
            gameObject.transform.Rotate(Vector3.up, rate);
        }
    }
}
