using Assets.Scripts.Generation.Painter.Cells.Base;
using Assets.Scripts.Misc;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Generation.Painter.Cells
{
    public class ElevationCell : Cell
    {
        public Direction elevationDirection; //Up or Down

        public ElevationCell(Vector3 position, Direction direction) : base(position, new List<string>(){
            Tags.CELL_IMPORTANT,
            Tags.CELL_ELEVATION
        })
        {
            cellType = CellType.Elevation_Cell;
            elevationDirection = direction;
        }

        public ElevationCell(Vector3 position, List<string> tags, Direction direction) : base(position, new List<string>(){
            Tags.CELL_IMPORTANT,
            Tags.CELL_ELEVATION
        }, tags)
        {
            cellType = CellType.Elevation_Cell;
            elevationDirection = direction;
        }
    }
}
