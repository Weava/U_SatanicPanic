using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Misc
{
    public class Slider : MonoBehaviour
    {
        public GameObject point_1;
        public GameObject point_2;

        public float movementRate;

        private GameObject nextPoint;

        void Start()
        {
            nextPoint = point_2;
        }

        void Update()
        {
            var selfPosition = transform.position;
            var pointPosition = nextPoint.transform.position;

            if (Vector3.Distance(selfPosition, pointPosition) <= 0.01f)
            {
                if (nextPoint == point_2)
                    nextPoint = point_1;
                else
                    nextPoint = point_2;
            }

            var newPosition = new Vector3(
                Mathf.Lerp(selfPosition.x, pointPosition.x, movementRate * Time.deltaTime),
                Mathf.Lerp(selfPosition.y, pointPosition.y, movementRate * Time.deltaTime),
                Mathf.Lerp(selfPosition.z, pointPosition.z, movementRate * Time.deltaTime)
                );

            transform.position = newPosition;
        }
    }
}
