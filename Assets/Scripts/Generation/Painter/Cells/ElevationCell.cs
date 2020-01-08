using Assets.Scripts.Generation.Painter.Cells.Base;
using Assets.Scripts.Misc;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Generation.Painter.Cells
{
    public class ElevationCell : Cell
    {
        public ElevationCell(Vector3 position) : base(position, new List<string>(){
            Tags.CELL_IMPORTANT,
            Tags.CELL_ELEVATION
        })
        {
            cellType = CellType.Elevation_Cell;
        }

        public ElevationCell(Vector3 position, List<string> tags) : base(position, new List<string>(){
            Tags.CELL_IMPORTANT,
            Tags.CELL_ELEVATION
        }, tags)
        {
            cellType = CellType.Elevation_Cell;
        }
    }
}
