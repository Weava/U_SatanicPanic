using Assets.Scripts.Generation.Extensions;
using Assets.Scripts.Generation.Painter;
using Assets.Scripts.Generation.Painter.Cells;
using Assets.Scripts.Generation.Painter.Cells.Base;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Painter_Generation.Rooms
{
    public static class Roomf
    {
        public static RoomClaim FindAvailableCellForRoomDimensions(this Region region, RoomDimensions dimensions)
        {
            switch(dimensions)
            {
                case RoomDimensions.Room_1_1:
                    return FindCellForRoom_1_1(region);
                case RoomDimensions.Room_1_2:
                    return FindCellForRoom_1_2(region);
                case RoomDimensions.Room_2_2:
                    return FindCellForRoom_2_2(region);
                case RoomDimensions.Room_2_3:
                    return FindCellForRoom_2_3(region);
                case RoomDimensions.Room_3_3:
                    return FindCellForRoom_3_3(region);
                case RoomDimensions.Room_3_4:
                    return null;
                case RoomDimensions.Room_4_4:
                    return null;
                case RoomDimensions.Room_5_5:
                default:
                    return null;
            }
        }

        #region Room Projecting

        public static RoomClaim ProjectRoom_1_2(this Region region, Cell rootCell, Direction direction)
        {
            var result = new RoomClaim();
            result.direction = direction;
            result.dimensions = RoomDimensions.Room_1_2;

            if (!region.ValidCellPoint(rootCell.position)) { result.failed = true; return result; };
            result.rootCell = rootCell;
            result.incompassedCells.Add(rootCell);

            region.ClaimCell(rootCell.Step(direction), ref result);

            result.ValidPathClaim();

            return result;
        }

        public static RoomClaim ProjectRoom_2_2(this Region region, Cell rootCell, Direction direction)
        {
            var result = new RoomClaim();
            result.direction = direction;
            result.dimensions = RoomDimensions.Room_2_2;

            if (!region.ValidCellPoint(rootCell.position)) { result.failed = true; return result; };
            result.rootCell = rootCell;
            result.incompassedCells.Add(rootCell);

            region.ClaimCell(rootCell.Step(direction), ref result);
            region.ClaimCell(rootCell.Step(direction.GetRightDirection()), ref result);
            region.ClaimCell(rootCell.StepDiagonal(direction, direction.GetRightDirection()), ref result, false);

            result.ValidPathClaim();

            return result;
        }

        public static RoomClaim ProjectRoom_2_3(this Region region, Cell rootCell, Direction direction)
        {
            var result = new RoomClaim();
            result.direction = direction;
            result.dimensions = RoomDimensions.Room_2_3;

            if (!region.ValidCellPoint(rootCell.position)) { result.failed = true; return result; };
            result.rootCell = rootCell;
            result.incompassedCells.Add(rootCell);

            region.ClaimCell(rootCell.Step(direction), ref result);
            region.ClaimCell(rootCell.Step(direction.GetRightDirection()), ref result);
            region.ClaimCell(rootCell.Step(direction.GetLeftDirection()), ref result);
            region.ClaimCell(rootCell.StepDiagonal(direction, direction.GetRightDirection()), ref result, false);
            region.ClaimCell(rootCell.StepDiagonal(direction, direction.GetLeftDirection()), ref result, false);

            result.ValidPathClaim();

            return result;
        }

        public static RoomClaim ProjectRoom_3_3(this Region region, Cell rootCell, Direction direction)
        {
            var result = new RoomClaim();
            result.direction = direction;
            result.dimensions = RoomDimensions.Room_3_3;

            if (!region.ValidCellPoint(rootCell.position)) { result.failed = true; return result; };
            result.rootCell = rootCell;
            result.incompassedCells.Add(rootCell);

            region.ClaimCell(rootCell.Step(direction), ref result);
            region.ClaimCell(rootCell.Step(direction.GetRightDirection()), ref result);
            region.ClaimCell(rootCell.Step(direction.GetLeftDirection()), ref result);
            region.ClaimCell(rootCell.Step(direction.GetOppositeDirection()), ref result);
            region.ClaimCell(rootCell.StepDiagonal(direction, direction.GetRightDirection()), ref result, false);
            region.ClaimCell(rootCell.StepDiagonal(direction, direction.GetLeftDirection()), ref result, false);
            region.ClaimCell(rootCell.StepDiagonal(direction.GetOppositeDirection(), direction.GetRightDirection()), ref result, false);
            region.ClaimCell(rootCell.StepDiagonal(direction.GetOppositeDirection(), direction.GetLeftDirection()), ref result, false);

            result.ValidPathClaim();

            return result;
        }

        private static bool ValidCellPoint(this Region region, Vector3 position, bool required = true)
        {
            if(required)
            {
                return region.CellIsHere(position) && !region.cells.collection[position].claimed;
            } else
            {
                if(region.CellIsHere(position))
                {
                    return !region.cells.collection[position].claimed;
                }
                else
                {
                    var cell = new DeadCell(position);
                    region.cells.Add(cell);
                    return true;
                }
            }
        }

        private static void ValidPathClaim(this RoomClaim claim)
        {
            if(claim.incompassedCells.Any(x => x.cellType == CellType.Main_Path_Cell))
            {
                var pathIndexesInOrder = claim.incompassedCells
                    .Where(x => x.cellType == CellType.Main_Path_Cell).OrderBy(o => (o as MainPathCell).pathSequence).ToList();

                var previousIndex = pathIndexesInOrder.First() as MainPathCell;
                foreach(var index in pathIndexesInOrder)
                {
                    if ((index as MainPathCell).pathSequence == (previousIndex as MainPathCell).pathSequence) continue;
                    if((index as MainPathCell).pathSequence == (previousIndex as MainPathCell).pathSequence + 1)
                    {
                        previousIndex = (MainPathCell)index;
                    }
                    else
                    {
                        claim.failed = true;
                        return;
                    }
                }
            }
        }

        #endregion

        #region Room Scanning

        public static RoomClaim FindCellForRoom_1_1(Region region)
        {
            var cell = region.cells.collection.Select(s => s.Value).FirstOrDefault(x => !x.claimed && x.cellType != CellType.Dead_Cell);

            if(cell != null)
            {
                return new RoomClaim()
                {
                    rootCell = cell,
                    incompassedCells = new List<Cell>()
                    { cell  },
                    direction = Direction.North,
                    dimensions = RoomDimensions.Room_1_1
                };
            }

            return null;
        }

        public static RoomClaim FindCellForRoom_1_2(Region region)
        {
            foreach(var cell in region.cells.collection.Select(s => s.Value).Where(x => !x.claimed && x.cellType != CellType.Dead_Cell).ToList())
            {
                foreach(var direction in Directionf.GetDirectionList())
                {
                    var projection = region.ProjectRoom_1_2(cell, direction);
                    if(projection.valid)
                    {
                        return projection;
                    } else
                    {
                        ClearDeadCellsByProjection(region, projection);
                    }
                }
            }

            return null;
        }

        public static RoomClaim FindCellForRoom_2_2(Region region)
        {
            foreach (var cell in region.cells.collection.Select(s => s.Value).Where(x => !x.claimed && x.cellType != CellType.Dead_Cell).ToList())
            {
                foreach (var direction in Directionf.GetDirectionList())
                {
                    var projection = region.ProjectRoom_2_2(cell, direction);
                    if (projection.valid)
                    {
                        return projection;
                    } else
                    {
                        ClearDeadCellsByProjection(region, projection);
                    }
                }
            }

            return null;
        }

        public static RoomClaim FindCellForRoom_2_3(Region region)
        {
            foreach (var cell in region.cells.collection.Select(s => s.Value).Where(x => !x.claimed && x.cellType != CellType.Dead_Cell).ToList())
            {
                foreach (var direction in Directionf.GetDirectionList())
                {
                    var projection = region.ProjectRoom_2_3(cell, direction);
                    if (projection.valid)
                    {
                        return projection;
                    }
                    else
                    {
                        ClearDeadCellsByProjection(region, projection);
                    }
                }
            }

            return null;
        }

        public static RoomClaim FindCellForRoom_3_3(Region region)
        {
            foreach (var cell in region.cells.collection.Select(s => s.Value).Where(x => !x.claimed && x.cellType != CellType.Dead_Cell).ToList())
            {
                foreach (var direction in Directionf.GetDirectionList())
                {
                    var projection = region.ProjectRoom_3_3(cell, direction);
                    if (projection.valid)
                    {
                        return projection;
                    }
                    else
                    {
                        ClearDeadCellsByProjection(region, projection);
                    }
                }
            }

            return null;
        }

        #endregion

        private static void ClaimCell(this Region region, Vector3 position, ref RoomClaim result, bool required = true)
        {
            if (!region.ValidCellPoint(position, required)) { result.failed = true; return; }

            result.incompassedCells.Add(region.cells.collection[position]);
        }

        private static void ClearDeadCellsByProjection(this Region region, RoomClaim claim)
        {
            var deadCells = claim.incompassedCells.Where(x => x.cellType == CellType.Dead_Cell);
            foreach(var cell in deadCells)
            {
                region.cells.collection.Remove(cell.position);
            }
        }
    }

    public class RoomClaim
    {
        public bool failed;
        public bool valid { get { return !failed && !incompassedCells.Any(x => x.claimed); }}
        public Direction direction = Direction.North;
        public RoomDimensions dimensions;
        public Cell rootCell;
        public List<Cell> incompassedCells = new List<Cell>();
    }
}
