using Assets.Scripts.Generation.Painter.Cells.Base;
using Assets.Scripts.Generation.Painter.Rooms.Base;
using Assets.Scripts.Misc;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Generation.Extensions
{
    public static class Roomf
    {
        public static RoomProjection ProjectRoom(this Vector3 rootPosition, Direction direction, RoomSize roomSize, RoomOptions options)
        {
            var result = new RoomProjection();

            switch(roomSize)
            {
                case RoomSize.Room_1_1:
                    result = ProjectRoom_1_1(rootPosition, options);
                    break;
                case RoomSize.Room_1_2:
                    result = ProjectRoom_1_2(rootPosition, direction, options);
                    break;
                case RoomSize.Room_2_2:
                    result = ProjectRoom_2_2(rootPosition, direction, options);
                    break;
                case RoomSize.Room_2_3:
                    result = ProjectRoom_2_3(rootPosition, direction, options);
                    break;
                case RoomSize.Room_3_3:
                    result = ProjectRoom_3_3(rootPosition, direction, options);
                    break;
                case RoomSize.Room_4_4:
                    result = ProjectRoom_4_4(rootPosition, direction, options);
                    break;
                case RoomSize.Room_5_5:
                    result = ProjectRoom_5_5(rootPosition, direction, options);
                    break;
                default:
                    return null;
            }

            if (result != null && result.cells.Any() && !result.cells.PathCellsAreInSequence()) return null;

            return result;
        }

        public static Room ClaimForRoom(this RoomProjection projection, RoomSize roomSize, Direction direction, RoomOptions options)
        {
            var room = new Room();

            room.region = options.Region;
            room.subregion = options.Subregion;
            room.roomSize = roomSize;
            room.orientation = direction;

            foreach(var cell in projection.cells.ToList())
            {
                if (cell.claimed) return null;
                cell.claimed = true;
                cell.room = room;
                room.cells.Add(cell.position, cell);
                room.rootPosition = projection.rootCell.position;
            }

            RoomCollection.collection.Add(room);

            return room;
        }

        #region Projections
        // R ->
        public static RoomProjection ProjectRoom_1_1(this Vector3 rootPosition, RoomOptions options)
        {
            var result = new RoomProjection();

            result.rootCell = CellCollection.collection[rootPosition];

            if (VerifyCell(rootPosition, options, true)) result.cells.Add(CellCollection.collection[rootPosition]); else return null;

            return result;
        }

        // RX ->
        public static RoomProjection ProjectRoom_1_2(this Vector3 rootPosition, Direction direction, RoomOptions options)
        {
            var result = new RoomProjection();

            result.rootCell = CellCollection.collection[rootPosition];

            var pointsToTry = new RoomNode[] {
                new RoomNode(rootPosition, true),
                new RoomNode(rootPosition.Step(direction), true)
            };

            foreach (var point in pointsToTry)
            {
                if (VerifyCell(point.position, options, point.required)) result.cells.Add(CellCollection.collection[point.position]); else return null;
            }

            return result;
        }

        // RX ->
        // X-
        public static RoomProjection ProjectRoom_2_2(this Vector3 rootPosition, Direction direction, RoomOptions options)
        {
            var result = new RoomProjection();

            result.rootCell = CellCollection.collection[rootPosition];

            var pointsToTry = new RoomNode[] {
                new RoomNode(rootPosition, true),
                new RoomNode(rootPosition.Step(direction), true),
                new RoomNode(rootPosition.Step(direction.GetRightDirection()), true),
                new RoomNode(rootPosition.StepDiagonal(direction, direction.GetRightDirection()))
            };

            foreach (var point in pointsToTry)
            {
                if (VerifyCell(point.position, options, point.required)) result.cells.Add(CellCollection.collection[point.position]); else return null;
            }

            return result;
        }

        // X-
        // RX ->
        // X-
        public static RoomProjection ProjectRoom_2_3(this Vector3 rootPosition, Direction direction, RoomOptions options)
        {
            var result = new RoomProjection();

            result.rootCell = CellCollection.collection[rootPosition];

            var pointsToTry = new RoomNode[] {
                new RoomNode(rootPosition, true),
                new RoomNode(rootPosition.Step(direction), true),
                new RoomNode(rootPosition.Step(direction.GetRightDirection()), true),
                new RoomNode(rootPosition.Step(direction.GetLeftDirection()), true),
                new RoomNode(rootPosition.StepDiagonal(direction, direction.GetRightDirection())),
                new RoomNode(rootPosition.StepDiagonal(direction, direction.GetLeftDirection()))
            };

            foreach (var point in pointsToTry)
            {
                if (VerifyCell(point.position, options, point.required)) result.cells.Add(CellCollection.collection[point.position]); else return null;
            }

            return result;
        }

        // -X-
        // XRX ->
        // -X-
        public static RoomProjection ProjectRoom_3_3(this Vector3 rootPosition, Direction direction, RoomOptions options)
        {
            var result = new RoomProjection();

            result.rootCell = CellCollection.collection[rootPosition];

            var pointsToTry = new RoomNode[] {
                new RoomNode(rootPosition, true),
                new RoomNode(rootPosition.Step(direction), true),
                new RoomNode(rootPosition.Step(direction.GetRightDirection()), true),
                new RoomNode(rootPosition.Step(direction.GetLeftDirection()), true),
                new RoomNode(rootPosition.Step(direction.GetOppositeDirection()), true),
                new RoomNode(rootPosition.StepDiagonal(direction, direction.GetRightDirection())),
                new RoomNode(rootPosition.StepDiagonal(direction, direction.GetLeftDirection())),
                new RoomNode(rootPosition.StepDiagonal(direction.GetOppositeDirection(), direction.GetRightDirection())),
                new RoomNode(rootPosition.StepDiagonal(direction.GetOppositeDirection(), direction.GetLeftDirection()))
            };

            foreach (var point in pointsToTry)
            {
                if (VerifyCell(point.position, options, point.required)) result.cells.Add(CellCollection.collection[point.position]); else return null;
            }

            return result;
        }

        // -XX-
        // XRXX ->
        // XXXX
        // -XX-
        public static RoomProjection ProjectRoom_4_4(this Vector3 rootPosition, Direction direction, RoomOptions options)
        {
            var result = new RoomProjection();

            result.rootCell = CellCollection.collection[rootPosition];

            var pointsToTry = new RoomNode[] {
                new RoomNode(rootPosition.StepDiagonal(direction.GetOppositeDirection(), direction.GetLeftDirection())),
                new RoomNode(rootPosition.Step(direction.GetLeftDirection()), true),
                new RoomNode(rootPosition.StepDiagonal(direction, direction.GetLeftDirection()), true),
                new RoomNode(rootPosition.StepDiagonal(direction, direction.GetLeftDirection(), 2, 1)),

                new RoomNode(rootPosition.Step(direction.GetOppositeDirection()), true),
                new RoomNode(rootPosition, true),
                new RoomNode(rootPosition.Step(direction), true),
                new RoomNode(rootPosition.Step(direction, 2), true),

                new RoomNode(rootPosition.StepDiagonal(direction.GetOppositeDirection(), direction.GetRightDirection()), true),
                new RoomNode(rootPosition.Step(direction.GetRightDirection()), true),
                new RoomNode(rootPosition.StepDiagonal(direction, direction.GetRightDirection()), true),
                new RoomNode(rootPosition.StepDiagonal(direction, direction.GetRightDirection(), 2, 1), true),

                new RoomNode(rootPosition.StepDiagonal(direction.GetOppositeDirection(), direction.GetRightDirection(), 1, 2)),
                new RoomNode(rootPosition.Step(direction.GetRightDirection(), 2), true),
                new RoomNode(rootPosition.StepDiagonal(direction, direction.GetRightDirection(), 1, 2), true),
                new RoomNode(rootPosition.StepDiagonal(direction, direction.GetRightDirection(), 2, 2)),
            };

            foreach (var point in pointsToTry)
            {
                if (VerifyCell(point.position, options, point.required)) result.cells.Add(CellCollection.collection[point.position]); else return null;
            }

            return result;
        }

        // --X--
        // -XXX-
        // XXRXX ->
        // -XXX-
        // --X--
        public static RoomProjection ProjectRoom_5_5(this Vector3 rootPosition, Direction direction, RoomOptions options)
        {
            var result = new RoomProjection();

            result.rootCell = CellCollection.collection[rootPosition];

            var pointsToTry = new RoomNode[] {
                new RoomNode(rootPosition.StepDiagonal(direction.GetOppositeDirection(), direction.GetLeftDirection(), 2,2)),
                new RoomNode(rootPosition.StepDiagonal(direction.GetOppositeDirection(), direction.GetLeftDirection(), 1,2)),
                new RoomNode(rootPosition.Step(direction.GetLeftDirection(), 2), true),
                new RoomNode(rootPosition.StepDiagonal(direction, direction.GetLeftDirection(), 1,2)),
                new RoomNode(rootPosition.StepDiagonal(direction, direction.GetLeftDirection(), 2,2)),

                new RoomNode(rootPosition.StepDiagonal(direction.GetOppositeDirection(), direction.GetLeftDirection(), 1,2)),
                new RoomNode(rootPosition.StepDiagonal(direction.GetOppositeDirection(), direction.GetLeftDirection()), true),
                new RoomNode(rootPosition.Step(direction.GetLeftDirection()), true),
                new RoomNode(rootPosition.StepDiagonal(direction, direction.GetLeftDirection()), true),
                new RoomNode(rootPosition.StepDiagonal(direction, direction.GetLeftDirection(), 1,2)),

                new RoomNode(rootPosition.Step(direction.GetOppositeDirection(), 2), true),
                new RoomNode(rootPosition.Step(direction.GetOppositeDirection()), true),
                new RoomNode(rootPosition, true),
                new RoomNode(rootPosition.Step(direction), true),
                new RoomNode(rootPosition.Step(direction, 2), true),

                new RoomNode(rootPosition.StepDiagonal(direction.GetOppositeDirection(), direction.GetRightDirection(), 1,2)),
                new RoomNode(rootPosition.StepDiagonal(direction.GetOppositeDirection(), direction.GetRightDirection()), true),
                new RoomNode(rootPosition.Step(direction.GetRightDirection()), true),
                new RoomNode(rootPosition.StepDiagonal(direction, direction.GetRightDirection()), true),
                new RoomNode(rootPosition.StepDiagonal(direction, direction.GetRightDirection(), 1,2)),

                new RoomNode(rootPosition.StepDiagonal(direction.GetOppositeDirection(), direction.GetRightDirection(), 2,2)),
                new RoomNode(rootPosition.StepDiagonal(direction.GetOppositeDirection(), direction.GetRightDirection(), 1,2)),
                new RoomNode(rootPosition.Step(direction.GetRightDirection(), 2), true),
                new RoomNode(rootPosition.StepDiagonal(direction, direction.GetRightDirection(), 1,2)),
                new RoomNode(rootPosition.StepDiagonal(direction, direction.GetRightDirection(), 2,2)),
            };

            foreach (var point in pointsToTry)
            {
                if (VerifyCell(point.position, options, point.required)) result.cells.Add(CellCollection.collection[point.position]); else return null;
            }

            return result;
        }

        private static bool VerifyCell(this Vector3 position, RoomOptions options, bool required = true)
        {
            if(CellCollection.collection.Keys.Contains(position))
            {
                var cell = CellCollection.collection[position];
                if (cell.tags.Contains(Tags.REGION, options.Region)
                    || cell.cellType == CellType.Proxy_Cell)
                    return !CellCollection.collection[position].claimed;
                else
                    return false;
            } else if(required)
            {
                return false;
            }

            var proxyCell = new Cell(position, CellType.Proxy_Cell);
            CellCollection.collection[position] = proxyCell;

            return true;
        }

        #endregion
    }

    public class RoomOptions
    {
        public bool outside = false;

        public string Region;
        public string Subregion;

        public List<RoomSize> excludeRoomSize = new List<RoomSize>();
    }

    public class RoomProjection
    {
        public List<Cell> cells = new List<Cell>();
        public Cell rootCell;
    }

    public class RoomNode
    {
        public bool required;
        public Vector3 position;

        public RoomNode(Vector3 position, bool required = false)
        {
            this.required = required;
            this.position = position;
        }
    }
}
