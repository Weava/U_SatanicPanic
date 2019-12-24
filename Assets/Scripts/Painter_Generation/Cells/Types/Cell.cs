using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Painter_Generation.Cells
{
    public class Cell
    {
        #region Properties
        public CellType cellType;

        public bool claimed = false;

        public Vector3 position = new Vector3();

        public List<string> tags = new List<string>();
        #endregion

        public Cell(Vector3 initPosition)
        {
            position = initPosition;
            cellType = CellType.Cell;
        }

        public Cell(Vector3 initPosition, List<string> initTags)
        {
            position = initPosition;
            tags.AddRange(initTags);
            cellType = CellType.Cell;
        }
    }

    public enum CellType
    {
        Cell,
        End_Cell,
        Spawn_Cell,
        Main_Path_Cell,
        Dead_Cell
    }
}
