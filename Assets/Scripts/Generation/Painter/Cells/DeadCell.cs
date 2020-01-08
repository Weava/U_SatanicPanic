using Assets.Scripts.Generation.Painter.Cells.Base;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Generation.Painter.Cells
{
    public class DeadCell : Cell
    {
        public DeadCell(Vector3 position):base(position)
        {
            cellType = CellType.Dead_Cell;
        }

        public DeadCell(Vector3 position, List<string> tags) : base(position, tags)
        {
            cellType = CellType.Dead_Cell;
        }
    }
}
