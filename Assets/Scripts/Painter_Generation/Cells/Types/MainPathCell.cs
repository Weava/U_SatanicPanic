using Assets.Scripts.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Painter_Generation.Cells
{
    public class MainPathCell : Cell
    {
        public int pathSequence;

        protected static int sequence = 0;

        public MainPathCell(Vector3 position) 
            : base(position, new List<string> {
                Tags.CELL_MAINPATH })
        {
            pathSequence = sequence++;
            cellType = CellType.Main_Path_Cell;
        }

        public static void ResetSequence()
        {
            sequence = 0;
        }
    }
}
