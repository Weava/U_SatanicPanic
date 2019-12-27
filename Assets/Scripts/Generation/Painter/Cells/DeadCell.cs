using Assets.Scripts.Generation.Painter.Cells.Base;
using UnityEngine;

namespace Assets.Scripts.Generation.Painter.Cells
{
    public class DeadCell : Cell
    {
        public DeadCell(Vector3 position):base(position)
        {
            cellType = CellType.Dead_Cell;
        }
    }
}
