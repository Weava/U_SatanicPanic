using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public static class Directionf
    {
        #region Shorthand
        public static Direction Opposite(this Direction direction)
        {
            return GetOppositeDirection(direction);
        }

        public static List<Direction> Neighbors(this Direction direction)
        {
            return GetNeighborDirections(direction);
        }

        public static Direction Left(this Direction direction)
        {
            return GetLeftDirection(direction);
        }

        public static Direction Right(this Direction direction)
        {
            return GetRightDirection(direction);
        }

        public static List<Direction> Directions(bool includeUpAndDown = false)
        {
            return GetDirectionList(includeUpAndDown);
        }

        public static Direction Random(List<Direction> directionPool)
        {
            return RandomDirection(directionPool);
        }

        public static Vector3 ToVector(this Direction direction)
        {
            return DirectionToVector(direction);
        }
        #endregion

        [Obsolete]
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
        [Obsolete]
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
        [Obsolete]
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
        [Obsolete]
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
        [Obsolete]
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
        [Obsolete]
        public static Direction RandomDirection(List<Direction> directions)
        {
            return directions[UnityEngine.Random.Range(0, directions.Count-2 /*Don't include Up and Down*/)];
        }
        [Obsolete]
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
        [Obsolete]
        public static Vector3 DirectionToVector(this Direction direction)
        {
            switch(direction)
            {
                case Direction.Down:
                    return new Vector3(0, -0.5f, 0);
                case Direction.Up:
                    return new Vector3(0, 0.5f, 0);
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
