using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Misc
{
    /// <summary>
    /// Gameobject that spawns when an actor dies
    /// </summary>
    public class Gib : MonoBehaviour
    {
        [SerializeField]
        protected float selfExplosiveForce;

        protected List<GameObject> debris = new List<GameObject>();

        private void Start()
        {

        }
    }
}
