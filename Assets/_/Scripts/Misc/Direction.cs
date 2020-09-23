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

        public static float ToAngle(this Direction direction)
        {
            switch (direction)
            {
                case Direction.East:
                    return 90f;
                case Direction.South:
                    return 180f;
                case Direction.West:
                    return 270f;
                default:
                    return 0f;
            }
        }

        public static Direction GetNormalTowards(Vector3 root, Vector3 towards)
        {
            var normal = (towards - root).normalized;
            if (normal.x > 0)
            {
                if (normal.z > 0)
                {
                    if (normal.z - normal.x > 0) return Direction.North;
                    return Direction.East;
                }
                else
                {
                    if (normal.z + normal.x > 0) return Direction.East;
                    return Direction.South;
                }
            } else if (normal.x < 0)
            {
                if (normal.z > 0)
                {
                    if (normal.z + normal.x > 0) return Direction.North;
                    return Direction.West;
                }
                else
                {
                    if (normal.z - normal.x > 0) return Direction.West;
                    return Direction.South;
                }
            }

            return Direction.Up;
        }

        public static Direction ToDirection(this int value)
        {
            if (value == 1) return Direction.East;
            if (value == 2) return Direction.South;
            if (value == 3) return Direction.West;
            return Direction.North;
        }

        public static int ToByteValue(this Direction direction)
        {
            switch (direction)
            {
                case Direction.East:
                    return 1;
                case Direction.South:
                    return 2;
                case Direction.West:
                    return 3;
                    default: return 0;
            }
        }

        public static List<Direction> GetDirectionsFromByte(this int value)
        {
            var result = new List<Direction>();

            if ((value & 0b_0001) == 0b_0001) result.Add(Direction.North);
            if ((value & 0b_0010) == 0b_0010) result.Add(Direction.East);
            if ((value & 0b_0100) == 0b_0100) result.Add(Direction.South);
            if ((value & 0b_1000) == 0b_1000) result.Add(Direction.West);

            return result;
        }

        #endregion

        /// <summary>
        /// Assumes that 0,0,0 is the pivot of this projection and North is the default normal
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Vector3 ProjectOffsetToNormal(this Vector3 offset, Direction normal)
        {
            switch (normal)
            {
                case Direction.West:
                    return new Vector3(-offset.z, offset.y, offset.x);
                case Direction.East:
                    return new Vector3(offset.z, offset.y, -offset.x);
                case Direction.South:
                    return new Vector3(-offset.x, offset.y, -offset.z);
                default:
                    return offset;
            }
        }

        public static Vector4 ProjectOffsetToNormal(this Vector4 offset, Direction normal)
        {
            switch (normal)
            {
                case Direction.West:
                    return new Vector4(-offset.z, offset.y, offset.x, ((int)offset.w).ToDirection().Left().ToByteValue());
                case Direction.East:
                    return new Vector4(offset.z, offset.y, -offset.x, ((int)offset.w).ToDirection().Right().ToByteValue());
                case Direction.South:
                    return new Vector4(-offset.x, offset.y, -offset.z, ((int)offset.w).ToDirection().Opposite().ToByteValue());
                default:
                    return offset;
            }
        }

        #region Obsolete

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

        #endregion
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
