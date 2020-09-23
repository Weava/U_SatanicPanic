using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation
{
    public class BaseLevelGenerator : MonoBehaviour
    {
        public string seed;

        public

        void Awake()
        {
            if (string.IsNullOrEmpty(seed))
            { seed = DateTime.Now.Ticks.ToString(); }
        }
    }
}
