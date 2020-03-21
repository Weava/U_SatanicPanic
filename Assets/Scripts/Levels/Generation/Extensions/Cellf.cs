using Assets.Scripts.Levels.Generation.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation.Extensions
{
    public static class Cellf
    {
        public const int CELL_STEP_OFFSET = 8;

        #region Directional Positioning

        public static Vector3 Step(this Cell cell, Direction direction, int offset = 1)
        {
            return Step(cell.position, direction, offset);
        }

        public static Vector3 Step(this Vector3 position, Direction direction, int offset = 1)
        {
            switch (direction)
            {
                case Direction.East:
                    return position + new Vector3(CELL_STEP_OFFSET * offset, 0, 0);
                case Direction.South:
                    return position + new Vector3(0, 0, -CELL_STEP_OFFSET * offset);
                case Direction.West:
                    return position + new Vector3(-CELL_STEP_OFFSET * offset, 0, 0);
                case Direction.Up:
                    return position + new Vector3(0, CELL_STEP_OFFSET * offset, 0);
                case Direction.Down:
                    return position + new Vector3(0, -CELL_STEP_OFFSET * offset, 0);
                case Direction.North:
                default:
                    return position + new Vector3(0, 0, CELL_STEP_OFFSET * offset);
            }
        }

        public static Vector3 PositionBetween(this Cell cell, Cell otherCell)
        {
            return PositionBetween(cell.position, otherCell.position);
        }

        public static Vector3 PositionBetween(this Vector3 position, Vector3 otherPosition)
        {
            return new Vector3(
                (position.x + otherPosition.x) / 2,
                (position.y + otherPosition.y) / 2,
                (position.z + otherPosition.z) / 2);
        }

        public static Vector3 PositionBetween(List<Cell> cells)
        {
            var result = new Vector3();

            result.x = cells.Sum(s => s.position.x) / cells.Count();
            result.y = cells.Sum(s => s.position.y) / cells.Count();
            result.z = cells.Sum(s => s.position.z) / cells.Count();

            return result;
        }

        public static bool HasSameRoom(this Cell cell, Cell target)
        {
            return cell.room == target.room;
        }

        #endregion

        #region Searching

        public static Cell FindClosestPathway(this Cell cell)
        {
            if (cell.type != CellType.Cell) return cell;
            if (cell.parent == null) { return null; }

            return FindClosestPathway(cell.parent);
        }

        public static List<Cell> NeighborCellsInRegion(this Cell cell)
        {
            var result = new List<Cell>();

            if (cell.region == "") return result;

            foreach(var direction in Directionf.Directions())
            {
                if(CellCollection.HasCellAt(cell.Step(direction))
                    && CellCollection.cells[cell.Step(direction)].region == cell.region)
                {
                    result.Add(CellCollection.cells[cell.Step(direction)]);
                }
            }

            return result;
        }

        public static List<Cell> NeighborCellsOutOfRoom(this Cell cell)
        {
            var result = new List<Cell>();

            foreach (var direction in Directionf.Directions())
            {
                if (CellCollection.HasCellAt(cell.Step(direction))
                    && CellCollection.cells[cell.Step(direction)].room != cell.room)
                {
                    result.Add(CellCollection.cells[cell.Step(direction)]);
                }
            }

            return result;
        }

        public static List<Cell> NeighborCellsInRoom(this Cell cell)
        {
            var result = new List<Cell>();

            foreach (var direction in Directionf.Directions())
            {
                if (CellCollection.HasCellAt(cell.Step(direction))
                    && CellCollection.cells[cell.Step(direction)].room == cell.room)
                {
                    result.Add(CellCollection.cells[cell.Step(direction)]);
                }
            }

            return result;
        }

        #endregion
    }
}
