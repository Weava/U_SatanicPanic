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

        public int globalSequence;

        public static int globalSequenceCounter = 0;

        public string Region;

        public string Subregion;

        public Vector3 position = new Vector3();

        public List<CellConnection> connections;

        public TagCollection tags;

        #region Context

        public Direction orientation;

        public bool isCornerCell;

        public Room room;

        public List<Cell> knownConnections = new List<Cell>();

        #endregion

        #endregion

        #region CTOR

        public Cell(CellType type = CellType.Cell)
        {
            tags = new TagCollection();
            connections = new List<CellConnection>();
            cellType = type;
            if (type == CellType.Proxy_Cell) proxyTemp = true;
            if (type == CellType.Path_Cell) globalSequence = globalSequenceCounter++;
            UpdatePathCell();
        }

        public Cell(Vector3 initPosition, CellType type = CellType.Cell)
        {
            tags = new TagCollection();
            connections = new List<CellConnection>();
            position = initPosition;
            cellType = type;
            if (type == CellType.Proxy_Cell) proxyTemp = true;
            if (type == CellType.Path_Cell) globalSequence = globalSequenceCounter++;
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
            if (type == CellType.Path_Cell) globalSequence = globalSequenceCounter++;
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
        public Direction normal; //Points towards the connection
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
        Doorway = 0,
        Open = 1
    }
}
