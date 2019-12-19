using Assets.Scripts.Painter_Generation.CellRegions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Painter_Generation.Generators
{
    public class SimpleLinearLevelGenerator : LevelGenerator
    {
        public InfiniteSizeRegion cellRegion;

        public GameObject marker;

        private void Start()
        {
            Generate();
        }

        public override void Generate()
        {
            var result = cellRegion.BuildRegion();
            if(result)
            {
                foreach(var cell in cellRegion.cells.collection)
                {
                    Instantiate(marker, cell.Key, new Quaternion());
                }
            }
        }
    }
}
