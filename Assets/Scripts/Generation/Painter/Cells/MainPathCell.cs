using Assets.Scripts.Generation.Painter.Cells.Base;
using Assets.Scripts.Misc;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Generation.Painter.Cells
{
    public class MainPathCell : Cell
    {
        public int pathSequence;

        protected static int sequence = 0;

        public MainPathCell(Vector3 position) 
            : base(position, new List<string> {
                Tags.CELL_IMPORTANT,
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
