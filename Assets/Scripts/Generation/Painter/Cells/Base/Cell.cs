using Assets.Scripts.Generation.Extensions;
using Assets.Scripts.Generation.Painter.Rooms.Base;
using Assets.Scripts.Misc;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Generation.Painter.Cells.Base
{
    public class Cell
    {
        #region Properties

        public CellType cellType;

        public bool proxyTemp = false;

        public bool claimed = false;

        public bool important = false;

        public int pathSequence;

        public string Region;

        public string Subregion;

        public Vector3 position = new Vector3();

        public Room room;

        public List<CellConnection> connections;

        public TagCollection tags;

        #endregion

        #region CTOR

        public Cell(CellType type = CellType.Cell)
        {
            tags = new TagCollection();
            connections = new List<CellConnection>();
            cellType = type;
            if (type == CellType.Proxy_Cell) proxyTemp = true;
            UpdatePathCell();
        }

        public Cell(Vector3 initPosition, CellType type = CellType.Cell)
        {
            tags = new TagCollection();
            connections = new List<CellConnection>();
            position = initPosition;
            cellType = type;
            if (type == CellType.Proxy_Cell) proxyTemp = true;
            UpdatePathCell();
        }

        public Cell(Vector3 initPosition, List<string> initTags, CellType type = CellType.Cell)
        {
            tags = new TagCollection();
            connections = new List<CellConnection>();
            position = initPosition;
            tags.Add(initTags.ToArray());
            cellType = type;
            if (type == CellType.Proxy_Cell) proxyTemp = true;
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

    public class CellConnection
    {
        public Cell connectedCell;
        public DoorType doorType;
    }

    public enum CellType
    {
        Cell,
        End_Cell,
        Spawn_Cell,
        Path_Cell,
        Proxy_Cell,
        Elevation_Cell,
        Teleport_Cell
    }

    public enum DoorType
    {
        Doorway,
        Open
    }
}
