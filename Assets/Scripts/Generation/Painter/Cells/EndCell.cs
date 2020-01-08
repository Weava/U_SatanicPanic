using Assets.Scripts.Generation.Painter.Cells.Base;
using Assets.Scripts.Misc;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Generation.Painter.Cells
{
    public class EndCell : Cell
    {
        public EndCell(Vector3 position) 
            : base(position, new List<string>() {
                Tags.CELL_IMPORTANT, 
                Tags.CELL_END })
        {
            cellType = CellType.End_Cell;
        }

        public EndCell(Vector3 position, List<string> tags)
            : base(position, new List<string>() {
                Tags.CELL_IMPORTANT,
                Tags.CELL_END }, tags)
        {
            cellType = CellType.End_Cell;
        }
    }
}
