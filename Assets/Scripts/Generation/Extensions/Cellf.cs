using Assets.Scripts.Generation.Painter.Cells.Base;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Generation.Extensions
{
    public static class Cellf
    {
        private static int sequence = 0;

        private static readonly int CELL_SCALE = 8;

        #region Cell Generation

        public static Vector3 Step(this Vector3 position, Direction direction, int multiple = 1)
        {
            switch (direction)
            {
                case Direction.East:
                    return position + new Vector3(CELL_SCALE * multiple, 0, 0);
                case Direction.South:
                    return position + new Vector3(0, 0, -CELL_SCALE * multiple);
                case Direction.West:
                    return position + new Vector3(-CELL_SCALE * multiple, 0, 0);
                case Direction.North:
                default:
                    return position + new Vector3(0, 0, CELL_SCALE * multiple);
            }
        }

        public static Vector3 Step(this Cell cell, Direction direction, int multiple = 1)
        {
            return Step(cell.position, direction, multiple);
        }

        public static Vector3 StepDiagonal(this Vector3 position, Direction direction_1, Direction direction_2, int multiple_1 = 1, int multiple_2 = 1)
        {
            var result = position;
            switch (direction_1)
            {
                case Direction.East:
                    result += new Vector3(CELL_SCALE * multiple_1, 0, 0);
                    break;
                case Direction.South:
                    result += new Vector3(0, 0, -CELL_SCALE * multiple_1);
                    break;
                case Direction.West:
                    result += new Vector3(-CELL_SCALE * multiple_1, 0, 0);
                    break;
                case Direction.North:
                default:
                    result += new Vector3(0, 0, CELL_SCALE * multiple_1);
                    break;
            }

            switch (direction_2)
            {
                case Direction.East:
                    return result += new Vector3(CELL_SCALE * multiple_2, 0, 0);
                case Direction.South:
                    return result += new Vector3(0, 0, -CELL_SCALE * multiple_2);
                case Direction.West:
                    return result += new Vector3(-CELL_SCALE * multiple_2, 0, 0);
                case Direction.North:
                default:
                    return result += new Vector3(0, 0, CELL_SCALE * multiple_2);
            }
        }

        public static Vector3 StepDiagonal(this Cell cell, Direction direction_1, Direction direction_2, int multiple_1 = 1,  int multiple_2 = 1)
        {
            return StepDiagonal(cell.position, direction_1, direction_2, multiple_1, multiple_2);
        }

        public static List<Direction> AvailableDirections(this Cell cell, List<Direction> excludeDirections = null)
        {
            var directions = Directionf.GetDirectionList();
            if(excludeDirections != null)
            {
                foreach(var direction in excludeDirections)
                {
                    if (directions.Contains(direction)) directions.Remove(direction);
                }
            }
            var result = new List<Direction>();

            foreach(var direction in directions)
            {
                if( ! CellCollection.HasCellAt(cell.Step(direction)))
                    result.Add(direction);
            }

            return result;
        }

        public static void AddToPathSequence(this Cell cell)
        {
            cell.pathSequence = sequence++;
        }

        public static void ResetSequence()
        {
            sequence = 0;
        }

        #endregion

        #region Cell Context

        public static bool PathCellsAreInSequence(this List<Cell> cells)
        {
            var pathCells = cells.Where(x => x.cellType == CellType.Path_Cell).OrderBy(o => o.pathSequence);
            if (!pathCells.Any()) return true;
            var previousCell = pathCells.First();
            foreach(var cell in pathCells)
            {
                if(cell != previousCell)
                {
                    if (cell.pathSequence != previousCell.pathSequence + 1)
                        return false;
                    else
                        previousCell = cell;
                }
                else
                {
                    continue;
                }
            }

            return true;
        }

        public static List<Cell> NeighborCells(this Cell cell)
        {
            var result = new List<Cell>();

            foreach(var direction in Directionf.GetDirectionList(true))
            {
                if(CellCollection.HasCellAt(cell.Step(direction)))
                {
                    result.Add(CellCollection.collection[cell.Step(direction)]);
                }
            }

            return result;
        }

        #endregion
    }
}
