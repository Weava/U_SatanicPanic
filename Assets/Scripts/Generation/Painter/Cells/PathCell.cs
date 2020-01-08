using Assets.Scripts.Generation.Painter.Cells.Base;
using Assets.Scripts.Misc;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Generation.Painter.Cells
{
    public class PathCell : Cell
    {
        public int pathSequence;

        protected static int sequence = 0;

        public PathCell(Vector3 position) 
            : base(position, new List<string> {
                Tags.CELL_IMPORTANT,
                Tags.CELL_PATH })
        {
            pathSequence = sequence++;
            cellType = CellType.Path_Cell;
        }

        public PathCell(Vector3 position, List<string> tags)
            : base(position, new List<string> {
                Tags.CELL_IMPORTANT,
                Tags.CELL_PATH },
                  tags)
        {
            pathSequence = sequence++;
            cellType = CellType.Path_Cell;
        }

        public static void ResetSequence()
        {
            sequence = 0;
        }
    }
}
