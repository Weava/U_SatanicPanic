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

            switch (roomSize)
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

            foreach (var cell in projection.cells.ToList())
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
        // R -> 01 ->
        public static RoomProjection ProjectRoom_1_1(this Vector3 rootPosition, RoomOptions options)
        {
            var result = new RoomProjection();

            result.rootCell = CellCollection.collection[rootPosition];

            var proxy = new Cell();
            if (VerifyCell(rootPosition, options, ref proxy, true))
            {
                result.cells.Add(CellCollection.collection[rootPosition]);
            }
            else
            {
                CellCollection.Remove(proxy);
                return null;
            }

            return result;
        }

        // RX -> [01]02 ->
        public static RoomProjection ProjectRoom_1_2(this Vector3 rootPosition, Direction direction, RoomOptions options)
        {
            var result = new RoomProjection();

            result.rootCell = CellCollection.collection[rootPosition];

            var pointsToTry = new RoomNode[] {
                new RoomNode(rootPosition, true),
                new RoomNode(rootPosition.Step(direction), true)
            };

            var proxyCellTemps = new List<Cell>();

            foreach (var point in pointsToTry)
            {
                var proxy = new Cell();
                if (VerifyCell(point.position, options, ref proxy, point.required))
                {
                    proxyCellTemps.Add(proxy);
                    result.cells.Add(CellCollection.collection[point.position]);
                }
                else
                {
                    foreach (var cell in proxyCellTemps)
                    {
                        CellCollection.Remove(cell);
                    }
                    return null;
                }
            }

            return result;
        }

        // RX -> [01]02
        // X-     03 04 ->
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

            var proxyCellTemps = new List<Cell>();

            foreach (var point in pointsToTry)
            {
                var proxy = new Cell();
                if (VerifyCell(point.position, options, ref proxy, point.required))
                {
                    proxyCellTemps.Add(proxy);
                    result.cells.Add(CellCollection.collection[point.position]);
                }
                else
                {
                    foreach (var cell in proxyCellTemps)
                    {
                        CellCollection.Remove(cell);
                    }
                    return null;
                }
            }

            return result;
        }

        // X-     01 02
        // RX -> [03]04 ->
        // X-     05 06
        public static RoomProjection ProjectRoom_2_3(this Vector3 rootPosition, Direction direction, RoomOptions options)
        {
            var result = new RoomProjection();

            result.rootCell = CellCollection.collection[rootPosition];

            var pointsToTry = new RoomNode[] {
                new RoomNode(rootPosition.Step(direction.GetLeftDirection()), true),
                new RoomNode(rootPosition.StepDiagonal(direction, direction.GetLeftDirection())),
                new RoomNode(rootPosition, true),
                new RoomNode(rootPosition.Step(direction), true),
                new RoomNode(rootPosition.Step(direction.GetRightDirection()), true),
                new RoomNode(rootPosition.StepDiagonal(direction, direction.GetRightDirection()))
            };

            var proxyCellTemps = new List<Cell>();

            foreach (var point in pointsToTry)
            {
                var proxy = new Cell();
                if (VerifyCell(point.position, options, ref proxy, point.required)) {
                    proxyCellTemps.Add(proxy);
                    result.cells.Add(CellCollection.collection[point.position]);
                } else {
                    foreach (var cell in proxyCellTemps)
                    {
                        CellCollection.Remove(cell);
                    }
                    return null;
                }
            }

            return result;
        }

        // -X-     01 02 03
        // XRX ->  04[05]06 ->
        // -X-     07 08 09
        public static RoomProjection ProjectRoom_3_3(this Vector3 rootPosition, Direction direction, RoomOptions options)
        {
            var result = new RoomProjection();

            result.rootCell = CellCollection.collection[rootPosition];

            var pointsToTry = new RoomNode[] {
                new RoomNode(rootPosition.StepDiagonal(direction.GetOppositeDirection(), direction.GetLeftDirection())),
                new RoomNode(rootPosition.Step(direction.GetLeftDirection()), true),
                new RoomNode(rootPosition.StepDiagonal(direction, direction.GetLeftDirection())),

                new RoomNode(rootPosition.Step(direction.GetOppositeDirection()), true),
                new RoomNode(rootPosition, true),
                new RoomNode(rootPosition.Step(direction), true),

                new RoomNode(rootPosition.StepDiagonal(direction.GetOppositeDirection(), direction.GetRightDirection())),   
                new RoomNode(rootPosition.Step(direction.GetRightDirection()), true), 
                new RoomNode(rootPosition.StepDiagonal(direction, direction.GetRightDirection())),
            };

            var proxyCellTemps = new List<Cell>();

            foreach (var point in pointsToTry)
            {
                var proxy = new Cell();
                if (VerifyCell(point.position, options, ref proxy, point.required))
                {
                    proxyCellTemps.Add(proxy);
                    result.cells.Add(CellCollection.collection[point.position]);
                }
                else
                {
                    foreach (var cell in proxyCellTemps)
                    {
                        CellCollection.Remove(cell);
                    }
                    return null;
                }
            }

            return result;
        }

        // -XX-    01 02 03 04
        // XRXX -> 05[06]07 08 ->
        // XXXX    09 10 11 12
        // -XX-    13 14 15 16
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

            var proxyCellTemps = new List<Cell>();

            foreach (var point in pointsToTry)
            {
                var proxy = new Cell();
                if (VerifyCell(point.position, options, ref proxy, point.required))
                {
                    proxyCellTemps.Add(proxy);
                    result.cells.Add(CellCollection.collection[point.position]);
                }
                else
                {
                    foreach (var cell in proxyCellTemps)
                    {
                        CellCollection.Remove(cell);
                    }
                    return null;
                }
            }

            return result;
        }

        // --X--    01 02 03 04 05
        // -XXX-    06 07 08 09 10
        // XXRXX -> 11 12[13]14 15 ->
        // -XXX-    16 17 18 19 20 
        // --X--    21 22 23 24 25
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

            var proxyCellTemps = new List<Cell>();

            foreach (var point in pointsToTry)
            {
                var proxy = new Cell();
                if (VerifyCell(point.position, options, ref proxy, point.required))
                {
                    proxyCellTemps.Add(proxy);
                    result.cells.Add(CellCollection.collection[point.position]);
                }
                else
                {
                    foreach (var cell in proxyCellTemps)
                    {
                        CellCollection.Remove(cell);
                    }
                    return null;
                }
            }

            return result;
        }

        private static bool VerifyCell(this Vector3 position, RoomOptions options, ref Cell proxyCell, bool required = false)
        {
            if (CellCollection.collection.Keys.Contains(position))
            {
                var cell = CellCollection.collection[position];
                if (cell.Region == options.Region
                    || cell.cellType == CellType.Proxy_Cell)
                    return !CellCollection.collection[position].claimed;
                else
                    return false;
            } else if (required)
            {
                return false;
            }

            proxyCell = new Cell(position, CellType.Proxy_Cell);
            proxyCell.Region = Tags.CELL_UNCLAIMED;
            CellCollection.collection[position] = proxyCell;

            return true;
        }

        #endregion

        #region

        public static List<Room> NeighborRooms(this Room room)
        {
            var result = new List<Room>();

            foreach(var cell in room.cells.Select(s => s.Value))
            {
                var neighborCells = cell.NeighborCells().Where(x => x.room != room);
                if(neighborCells.Any())
                {
                    foreach(var neighbor in neighborCells)
                    {
                        if(!result.Any(x => x == neighbor.room))
                        {
                            result.Add(neighbor.room);
                        }
                    }
                }
            }

            return result;
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
