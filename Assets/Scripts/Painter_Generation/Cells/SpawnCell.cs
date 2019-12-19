using Assets.Scripts.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Painter_Generation.Cells
{
    public class SpawnCell : Cell
    {
        public SpawnCell(Vector3 position) 
            : base(position, new List<string>() {
                Tags.CELL_IMPORTANT,
                Tags.CELL_START,
                Tags.CELL_SPAWN })
        {

        }
    }
}
