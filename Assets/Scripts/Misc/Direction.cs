using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public static class Directionf
    {
        public static Direction GetOppositeDirection(this Direction direction)
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
                case Direction.Up:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.Up;
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

        public static Direction GetLeftDirection(this Direction direction)
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

        public static Direction GetRightDirection(this Direction direction)
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

        public static List<Direction> GetDirectionList(bool includeUpAndDown = false)
        {
            if(includeUpAndDown)
            {
                return new List<Direction>()
                {
                    Direction.North,
                    Direction.West,
                    Direction.South,
                    Direction.East,
                    Direction.Up,
                    Direction.Down
                };
            }

            return new List<Direction>()
            {
                Direction.North,
                Direction.West,
                Direction.South,
                Direction.East
            };
        }

        public static Direction RandomDirection(List<Direction> directions)
        {
            return directions[Random.Range(0, directions.Count-2 /*Don't include Up and Down*/)];
        }

        public static int RotationAngle(this Direction direction)
        {
            switch(direction)
            {
                case Direction.East:
                    return 90;
                case Direction.South:
                    return 180;
                case Direction.West:
                    return 270;
                case Direction.North:
                default:
                    return 0;
            }
        }

        public static Vector3 DirectionToVector(this Direction direction)
        {
            switch(direction)
            {
                case Direction.Down:
                    return new Vector3(0, -1, 0);
                case Direction.Up:
                    return new Vector3(0, 1, 0);
                case Direction.West:
                    return new Vector3(-1, 0, 0);
                case Direction.East:
                    return new Vector3(1, 0, 0);
                case Direction.South:
                    return new Vector3(0, 0, -1);
                case Direction.North:
                default:
                    return new Vector3(0, 0, 1);
            }
        }
    }

    public enum Direction
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3,
        Up = 4,
        Down = 5
    }
}
