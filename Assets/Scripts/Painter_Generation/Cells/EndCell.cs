using Assets.Scripts.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Painter_Generation.Cells
{
    public class EndCell : Cell
    {
        public EndCell(Vector3 position) 
            : base(position, new List<string>() {
                Tags.CELL_IMPORTANT, 
                Tags.CELL_END })
        {

        }
    }
}
