using Assets.Scripts.Generation.Painter.Cells.Base;
using Assets.Scripts.Misc;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Generation.Painter.Cells
{
    public class SpawnCell : Cell
    {
        public SpawnCell(Vector3 position) 
            : base(position, new List<string>() {
                Tags.CELL_IMPORTANT,
                Tags.CELL_START,
                Tags.CELL_SPAWN })
        {
            cellType = CellType.Spawn_Cell;
        }
    }
}
