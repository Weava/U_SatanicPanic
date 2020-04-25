using Assets.Scripts.Levels.Generation.Base.Mono;
using Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation.Base
{
    public class Cell
    {
        #region Meta
        public CellType type = CellType.Cell;

        public bool important { get { return (type == CellType.Pathway || type == CellType.Elevation || type == CellType.Spawn); } } 

        public int sequence = 0;

        public Vector3 position = new Vector3();

        public Cell parent;

        public List<Cell> children = new List<Cell>();

        public List<Node_Door> doors { get { return Level.doors.Where(x => x.Contains(this)).ToList(); } }

        public bool claimedByRoom { get {
                if(roomId != "")
                { return true; }
                return false;
            } }

        public bool elevationOverride_Upper = false;
        public bool elevationOverride_Lower = false;

        public string regionId = "";
        public string roomId = "";

        //public Room room;

        public Cell() { }

        public Cell(CellType type, Vector3 position)
        {
            this.type = type;
            this.position = position;
        }
        #endregion

        #region Parsing

        public bool hasDoor { get { return Level.doors.Any(x => x.cell_1 == this || x.cell_2 == this); } }

        #endregion
    }

    public static class CellCollection
    {
        public static Dictionary<Vector3, Cell> cells = new Dictionary<Vector3, Cell>();

        #region Add

        public static bool Add(this Cell cell, Vector3 position)
        {
            cell.position = position;
            return cell.Add();
        }

        public static bool Add(this Cell cell)
        {
            if(!cells.ContainsKey(cell.position))
            {
                cells.Add(cell.position, cell);
                return true;
            }

            return false;
        }

        public static bool Add(this List<Cell> cellsToAdd)
        {
            cellsToAdd.Where(x => cells.ContainsKey(x.position)).ToList().ForEach(x => cellsToAdd.Remove(x));

            foreach(var cell in cellsToAdd)
            {
                if(!HasCellAt(cell.position)) cells.Add(cell.position, cell);
            }

            return true;
        }

        #endregion

        #region Update

        public static bool Update(this Cell cell)
        {
            cells[cell.position] = cell;
            return true;
        }

        #endregion

        #region Remove

        public static bool Remove(Cell cell)
        {
            return Remove(cell.position);
        }

        public static bool Remove(Vector3 positon)
        {
            if(HasCellAt(positon))
            {
                var cell = cells[positon];
                cell.children = new List<Cell>();
                cell.parent = null;
                cells.Select(s => s.Value).Where(x => x.children.Contains(cell)).ToList().ForEach(x => x.children.Remove(cell));
                cells.Remove(cell.position);
                cell = null;
                return true;
            }

            return false;
        }

        #endregion

        #region Get

        public static bool HasCellAt(Vector3 position)
        {
            return cells.ContainsKey(position);
        }

        public static List<Cell> GetByRoom(string roomId)
        {
            return cells.Where(x => x.Value.roomId == roomId).Select(s => s.Value).ToList();
        }

        public static List<Cell> GetByRegion(string regionId)
        {
            return cells.Select(s => s.Value).Where(x => x.regionId == regionId).ToList();
        }

        public static Room GetRoom(this Cell cell)
        {
            if(cell.roomId != "")
            {
                return RoomCollection.rooms[cell.roomId];
            }
            return null;
        }

        public static Region GetRegion(this Cell cell)
        {
            if (cell.regionId != "")
            {
                return RegionCollection.regions[cell.regionId];
            }
            return null;
        }

        #endregion
    }

    public enum CellType
    {
        Cell,
        Pathway,
        Elevation,
        Spawn
    }
}
