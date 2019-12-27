using Assets.Scripts.Painter_Generation.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Levels.Base
{
    public class Level : MonoBehaviour
    {
        #region Properties

        #region Seed
        public string levelSeed = "";
        protected int seed;
        #endregion

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            RenderLevelSeed();
        }

        #endregion

        #region Meta methods
        private void RenderLevelSeed()
        {
            if(levelSeed == "")
            {
                foreach(char c in levelSeed)
                {
                    seed += c;
                }
            } else
            {
                seed = (int)DateTime.Now.Ticks;
            }
        }
        #endregion
    }
}
