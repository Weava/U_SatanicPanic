using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Painter_Generation
{
    public abstract class LevelGenerator : MonoBehaviour
    {
        public virtual void Generate()
        {
            throw new NotImplementedException("Level generator needs a generation definition.");
        }
    }
}
