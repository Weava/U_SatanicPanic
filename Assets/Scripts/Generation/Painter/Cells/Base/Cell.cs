using Assets.Scripts.Generation.Extensions;
using Assets.Scripts.Misc;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Generation.Painter.Cells.Base
{
    public class Cell
    {
        #region Properties

        public CellType cellType;

        public int pathSequence;

        public bool claimed = false;

        public Vector3 position = new Vector3();

        public TagCollection tags;

        #endregion

        #region CTOR

        public Cell(CellType type = CellType.Cell)
        {
            tags = new TagCollection();
            cellType = type;
            UpdatePathCell();
        }

        public Cell(Vector3 initPosition, CellType type = CellType.Cell)
        {
            tags = new TagCollection();
            position = initPosition;
            cellType = type;
            UpdatePathCell();
        }

        public Cell(Vector3 initPosition, List<string> initTags, CellType type = CellType.Cell)
        {
            tags = new TagCollection();
            position = initPosition;
            tags.Add(initTags.ToArray());
            cellType = type;
            UpdatePathCell();
        }

        #endregion

        #region Meta

        private void UpdatePathCell()
        {
            if (cellType == CellType.Path_Cell)
            {
                this.AddToPathSequence();
            }
        }

        #endregion
    }

    public enum CellType
    {
        Cell,
        End_Cell,
        Spawn_Cell,
        Path_Cell,
        Dead_Cell,
        Elevation_Cell,
        Teleport_Cell
    }
}
