using Assets.Scripts.Generation.Painter.Cells.Base;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Generation.Extensions
{
    public static class Cellf
    {
        private static readonly int CELL_SCALE = 8;

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
    }
}
