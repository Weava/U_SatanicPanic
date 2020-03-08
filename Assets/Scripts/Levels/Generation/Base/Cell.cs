using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation.Base
{
    public class Cell
    {
        public CellType type = CellType.Cell;

        public int sequence = 0;

        public string region = "";

        public Vector3 position = new Vector3();

        public Cell parent;

        public List<Cell> children = new List<Cell>();

        public bool claimedByRoom { get { return room != null; } }

        public Room room;

        public Cell() { }

        public Cell(CellType type, Vector3 position)
        {
            this.type = type;
            this.position = position;
        }
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
            if(cellsToAdd.Any(x => cells.ContainsKey(x.position)))
            { return false; }

            foreach(var cell in cellsToAdd)
            {
                cells.Add(cell.position, cell);
            }

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

        public static List<Cell> GetByRegion(string region)
        {
            return cells.Select(s => s.Value).Where(x => x.region == region).ToList();
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
