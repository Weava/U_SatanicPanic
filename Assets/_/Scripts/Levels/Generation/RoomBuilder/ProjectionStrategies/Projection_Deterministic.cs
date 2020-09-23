using Assets.Scripts.Levels.Generation.Base;
using Assets.Scripts.Levels.Generation.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Levels.Generation.RoomBuilder.ProjectionStrategies
{
    public static class Projection_Deterministic
    {
        public static List<Cell> Project(Cell root, ref List<Cell> cellsLeftToClaim)
        {
            var result = new List<Cell>();

            foreach (var direction in Directionf.Directions()) //Largest result is returned
            {
                var attempt = ProjectCascade(root, cellsLeftToClaim, direction);
                if (attempt.Count > result.Count) result = attempt; //Get the biggest result
            }

            //Fetched cells, now claim them
            foreach (var cell in result)
            {
                cellsLeftToClaim.Remove(cell);
            }

            return result;
        }

        #region Projection Methods

        private static List<Cell> ProjectCascade(Cell root, List<Cell> cellsLeftToClaim, Direction direction)
        {
            var result = new List<Cell> { root };

            if (!Project_1_2(root, cellsLeftToClaim, direction, ref result)) return result;

            Direction secondaryDirection;

            if (!Project_L(root, cellsLeftToClaim, direction, ref result, out secondaryDirection)) return result;

            RoomProjectionBranchType type;

            if (!Project_T_Or_Box(root, cellsLeftToClaim, direction, secondaryDirection, ref result, out type)) return result;

            if (type == RoomProjectionBranchType.Cross)
            {
                if (!Project_Cross(root, cellsLeftToClaim, direction, secondaryDirection, ref result)) return result;
            }
            else if (type == RoomProjectionBranchType.Box)
            {
                if (!Project_Rect(root, cellsLeftToClaim, direction, secondaryDirection, ref result)) return result;

                Direction tertiaryDirection
                    ;
                if (!Project_MediumBox(root, cellsLeftToClaim, direction, secondaryDirection, ref result, out tertiaryDirection)) return result;

                if (!Project_MediumRect(root, cellsLeftToClaim, direction, secondaryDirection, tertiaryDirection, ref result)) return result;

                if (!Project_Large(root, cellsLeftToClaim, direction, secondaryDirection, tertiaryDirection, ref result)) return result;
            }

            return result;
        }

        private static bool Project_1_2(Cell root, List<Cell> cellsLeftToClaim, Direction direction, ref List<Cell> result)
        {
            var cell = cellsLeftToClaim.FirstOrDefault(x => root.Step(direction) == x.position);
            if (cell != null)
            {
                result.Add(cell);
                return true;
            }

            return false;
        }

        private static bool Project_L(Cell root, List<Cell> cellsLeftToClaim, Direction direction, ref List<Cell> result, out Direction secondaryDirection)
        {
            //Right
            var attempt = cellsLeftToClaim.FirstOrDefault(x =>
                root.Step(direction)
                .Step(direction.Right()) == x.position);

            if (attempt != null)
            {
                result.Add(attempt);
                secondaryDirection = direction.Right();
                return true;
            }

            //Left
            attempt = cellsLeftToClaim.FirstOrDefault(x =>
                root.Step(direction)
                    .Step(direction.Left()) == x.position);

            if (attempt != null)
            {
                result.Add(attempt);
                secondaryDirection = direction.Left();
                return true;
            }

            secondaryDirection = Direction.Up; //Indicates nothing, it failed

            return false;
        }

        private static bool Project_T_Or_Box(Cell root, List<Cell> cellsLeftToClaim, Direction direction, Direction secondaryDirection,
            ref List<Cell> result, out RoomProjectionBranchType type)
        {
            //Box attempt
            var cell = cellsLeftToClaim.FirstOrDefault(x => root.Step(secondaryDirection) == x.position);

            if (cell != null)
            {
                result.Add(cell);
                type = RoomProjectionBranchType.Box;
                return true;
            }

            //T Attempt
            cell = cellsLeftToClaim.FirstOrDefault(x => root.Step(direction, 2) == x.position);
            if (cell != null)
            {
                result.Add(cell);
                type = RoomProjectionBranchType.Cross;
                return true;
            }

            type = RoomProjectionBranchType.Box; //Doesn't mean anything, it failed

            return false;
        }

        private static bool Project_Cross(Cell root, List<Cell> cellsLeftToClaim, Direction direction,
            Direction secondaryDirection,
            ref List<Cell> result)
        {
            var cell = cellsLeftToClaim.FirstOrDefault(x =>
                root.Step(direction).Step(secondaryDirection.Opposite()) == x.position);

            if (cell != null)
            {
                result.Add(cell);
                return true;
            }

            return false;
        }

        private static bool Project_Rect(Cell root, List<Cell> cellsLeftToClaim, Direction direction, Direction secondaryDirection,
            ref List<Cell> result)
        {
            var cell_1 = cellsLeftToClaim.FirstOrDefault(x => root.Step(direction.Opposite()) == x.position);
            var cell_2 = cellsLeftToClaim.FirstOrDefault(x => root.Step(direction.Opposite()).Step(secondaryDirection) == x.position);

            if (cell_1 != null && cell_2 != null)
            {
                result.Add(cell_1);
                result.Add(cell_2);
                return true;
            }

            return false;
        }

        private static bool Project_MediumBox(Cell root, List<Cell> cellsLeftToClaim, Direction direction,
            Direction secondaryDirection,
            ref List<Cell> result, out Direction tertiaryDirection)
        {
            var cell_1 = cellsLeftToClaim.FirstOrDefault(x => root.Step(direction.Opposite()).Step(secondaryDirection.Opposite()) == x.position);
            var cell_2 = cellsLeftToClaim.FirstOrDefault(x => root.Step(secondaryDirection.Opposite()) == x.position);
            var cell_3 = cellsLeftToClaim.FirstOrDefault(x => root.Step(direction).Step(secondaryDirection.Opposite()) == x.position);

            if (cell_1 != null && cell_2 != null && cell_3 != null)
            {
                result.Add(cell_1);
                result.Add(cell_2);
                result.Add(cell_3);
                tertiaryDirection = secondaryDirection.Opposite();
                return true;
            }

            cell_1 = cellsLeftToClaim.FirstOrDefault(x => root.Step(direction.Opposite()).Step(secondaryDirection, 2) == x.position);
            cell_2 = cellsLeftToClaim.FirstOrDefault(x => root.Step(secondaryDirection, 2) == x.position);
            cell_3 = cellsLeftToClaim.FirstOrDefault(x => root.Step(direction).Step(secondaryDirection, 2) == x.position);

            if (cell_1 != null && cell_2 != null && cell_3 != null)
            {
                result.Add(cell_1);
                result.Add(cell_2);
                result.Add(cell_3);
                tertiaryDirection = secondaryDirection;
                return true;
            }

            tertiaryDirection = Direction.Up;

            return false;
        }

        private static bool Project_MediumRect(Cell root, List<Cell> cellsLeftToClaim, Direction direction,
            Direction secondaryDirection, Direction tertiaryDirection, ref List<Cell> result)
        {
            if (tertiaryDirection == secondaryDirection)
            {
                var cell_1 = cellsLeftToClaim.FirstOrDefault(x =>
                    root.Step(direction.Opposite()).Step(secondaryDirection, 3) == x.position);
                var cell_2 = cellsLeftToClaim.FirstOrDefault(x =>
                    root.Step(secondaryDirection, 3) == x.position);
                var cell_3 = cellsLeftToClaim.FirstOrDefault(x =>
                    root.Step(direction).Step(secondaryDirection, 3) == x.position);

                if (cell_1 != null && cell_2 != null && cell_3 != null)
                {
                    result.Add(cell_1);
                    result.Add(cell_2);
                    result.Add(cell_3);
                    return true;
                }
            }
            else
            {
                var cell_1 = cellsLeftToClaim.FirstOrDefault(x =>
                    root.Step(direction.Opposite()).Step(secondaryDirection.Opposite(), 2) == x.position);
                var cell_2 = cellsLeftToClaim.FirstOrDefault(x =>
                    root.Step(secondaryDirection.Opposite(), 2) == x.position);
                var cell_3 = cellsLeftToClaim.FirstOrDefault(x =>
                    root.Step(direction).Step(secondaryDirection.Opposite(), 2) == x.position);

                if (cell_1 != null && cell_2 != null && cell_3 != null)
                {
                    result.Add(cell_1);
                    result.Add(cell_2);
                    result.Add(cell_3);
                    return true;
                }
            }

            return false;
        }

        private static bool Project_Large(Cell root, List<Cell> cellsLeftToClaim, Direction direction,
            Direction secondaryDirection, Direction tertiaryDirection, ref List<Cell> result)
        {
            if (tertiaryDirection == secondaryDirection)
            {
                var cell_1 = cellsLeftToClaim.FirstOrDefault(x => root.Step(direction, 2) == x.position);
                var cell_2 = cellsLeftToClaim.FirstOrDefault(x => root.Step(direction, 2).Step(secondaryDirection) == x.position);
                var cell_3 = cellsLeftToClaim.FirstOrDefault(x => root.Step(direction, 2).Step(secondaryDirection, 2) == x.position);
                var cell_4 = cellsLeftToClaim.FirstOrDefault(x => root.Step(direction, 2).Step(secondaryDirection, 3) == x.position);

                if (cell_1 != null && cell_2 != null && cell_3 != null && cell_4 != null)
                {
                    result.Add(cell_1);
                    result.Add(cell_2);
                    result.Add(cell_3);
                    result.Add(cell_4);
                    return true;
                }

                cell_1 = cellsLeftToClaim.FirstOrDefault(x => root.Step(direction.Opposite(), 2) == x.position);
                cell_2 = cellsLeftToClaim.FirstOrDefault(x => root.Step(direction.Opposite(), 2).Step(secondaryDirection) == x.position);
                cell_3 = cellsLeftToClaim.FirstOrDefault(x => root.Step(direction.Opposite(), 2).Step(secondaryDirection, 2) == x.position);
                cell_4 = cellsLeftToClaim.FirstOrDefault(x => root.Step(direction.Opposite(), 2).Step(secondaryDirection, 3) == x.position);

                if (cell_1 != null && cell_2 != null && cell_3 != null && cell_4 != null)
                {
                    result.Add(cell_1);
                    result.Add(cell_2);
                    result.Add(cell_3);
                    result.Add(cell_4);
                    return true;
                }
            }
            else
            {
                var cell_1 = cellsLeftToClaim.FirstOrDefault(x => root.Step(direction, 2) == x.position);
                var cell_2 = cellsLeftToClaim.FirstOrDefault(x => root.Step(direction, 2).Step(secondaryDirection) == x.position);
                var cell_3 = cellsLeftToClaim.FirstOrDefault(x => root.Step(direction, 2).Step(secondaryDirection.Opposite()) == x.position);
                var cell_4 = cellsLeftToClaim.FirstOrDefault(x => root.Step(direction, 2).Step(secondaryDirection.Opposite(), 2) == x.position);

                if (cell_1 != null && cell_2 != null && cell_3 != null && cell_4 != null)
                {
                    result.Add(cell_1);
                    result.Add(cell_2);
                    result.Add(cell_3);
                    result.Add(cell_4);
                    return true;
                }

                cell_1 = cellsLeftToClaim.FirstOrDefault(x => root.Step(direction.Opposite(), 2) == x.position);
                cell_2 = cellsLeftToClaim.FirstOrDefault(x => root.Step(direction.Opposite(), 2).Step(secondaryDirection) == x.position);
                cell_3 = cellsLeftToClaim.FirstOrDefault(x => root.Step(direction.Opposite(), 2).Step(secondaryDirection.Opposite()) == x.position);
                cell_4 = cellsLeftToClaim.FirstOrDefault(x => root.Step(direction.Opposite(), 2).Step(secondaryDirection.Opposite(), 2) == x.position);

                if (cell_1 != null && cell_2 != null && cell_3 != null && cell_4 != null)
                {
                    result.Add(cell_1);
                    result.Add(cell_2);
                    result.Add(cell_3);
                    result.Add(cell_4);
                    return true;
                }
            }

            return false;
        }

        #endregion Projection Methods
    }

    public enum RoomProjectionBranchType
    {
        Cross,
        Box
    }
}