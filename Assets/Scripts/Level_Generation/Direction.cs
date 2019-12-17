using System.Collections.Generic;

namespace Assets.Scripts
{
    public static class Directionf
    {
        public static Direction GetOppositeDirection(Direction direction)
        {
            switch(direction)
            {
                case Direction.North:
                    return Direction.South;
                case Direction.East:
                    return Direction.West;
                case Direction.South:
                    return Direction.North;
                case Direction.West:
                    return Direction.East;
                default:
                    return direction;
            }
        }

        public static List<Direction> GetNeighborDirections(Direction direction)
        {
            switch(direction)
            {
                case Direction.North:
                case Direction.South:
                    return new List<Direction>() { Direction.East, Direction.West };
                case Direction.West:
                case Direction.East:
                    return new List<Direction>() { Direction.North, Direction.South };
                default:
                    return new List<Direction>();
            }
        }

        public static Direction GetLeftDirection(Direction direction)
        {
            switch(direction)
            {
                case Direction.North:
                    return Direction.West;
                case Direction.East:
                    return Direction.North;
                case Direction.South:
                    return Direction.East;
                default:
                    return Direction.South;
            }
        }

        public static Direction GetRightDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return Direction.East;
                case Direction.East:
                    return Direction.South;
                case Direction.South:
                    return Direction.West;
                default:
                    return Direction.North;
            }
        }

        public static List<Direction> GetDirectionList()
        {
            return new List<Direction>()
            {
                Direction.North,
                Direction.West,
                Direction.South,
                Direction.East
            };
        }
    }

    public enum Direction
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3
    }
}
