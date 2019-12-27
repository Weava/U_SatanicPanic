using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Painter_Generation.Cells.Types
{
    public class DeadCell : Cell
    {
        public DeadCell(Vector3 position):base(position)
        {
            cellType = CellType.Dead_Cell;
        }
    }
}
