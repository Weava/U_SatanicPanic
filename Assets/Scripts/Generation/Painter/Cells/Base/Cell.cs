using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Generation.Painter.Cells.Base
{
    public class Cell
    {
        #region Properties
        public CellType cellType;

        public bool claimed = false;

        public Vector3 position = new Vector3();

        public List<string> tags = new List<string>();
        #endregion

        public Cell()
        {
        }

        public Cell(Vector3 initPosition)
        {
            position = initPosition;
            cellType = CellType.Cell;
        }

        public Cell(Vector3 initPosition, List<string> tags)
        {
            position = initPosition;
            tags.AddRange(tags);
            cellType = CellType.Cell;
        }

        public Cell(Vector3 initPosition, List<string> initTags, List<string> additionalTags)
        {
            position = initPosition;
            tags.AddRange(initTags);
            tags.AddRange(additionalTags);
            cellType = CellType.Cell;
        }
    }

    public enum CellType
    {
        Cell,
        End_Cell,
        Spawn_Cell,
        Path_Cell,
        Dead_Cell,
        Elevation_Cell
    }
}
