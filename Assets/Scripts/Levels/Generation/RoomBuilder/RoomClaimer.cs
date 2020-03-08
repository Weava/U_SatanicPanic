using Assets.Scripts.Levels.Generation.Base;
using Assets.Scripts.Levels.Generation.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation.RoomBuilder
{
    public static class RoomClaimer
    {
        public const int MAXIMUM_CLAIM_AMOUNT = 16;

        public static bool ClaimRoom(List<Cell> cells)
        {
            /*Rooms can only exist within one region*/
            if(cells.Select(s => s.region).Distinct().Count() > 1) { return false; }

            /*A room can only contain a complete sequence of sequenced cells*/
            if(cells.Any(x => x.type != CellType.Cell))
            {
                var sequencedCells = cells.Where(x => x.type != CellType.Cell).OrderBy(o => o.sequence).ToList();
                for(int i = 0; i < sequencedCells.Count() - 1; i++)
                {
                    if(sequencedCells[i].sequence != sequencedCells[i+1].sequence - 1)
                    { return false; }
                }
            }

            var room = new Room();
            room.cells = cells;

            foreach(var cell in cells)
            {
                cell.room = room;
            }

            RoomCollection.rooms.Add(room);

            return true;
        }

        /// <summary>
        /// For the cells that exist in a region, use a claiming pattern to claim the cells for unique rooms
        /// </summary>
        /// <param name="cells"></param>
        /// <returns></returns>
        public static bool ClaimRooms(string region, RoomClaimStrategy strategy, int claimRange)
        {
            var cells = CellCollection.GetByRegion(region);

            if (!cells.Any()) return false;

            if (claimRange > MAXIMUM_CLAIM_AMOUNT)
            { claimRange = MAXIMUM_CLAIM_AMOUNT; }

            switch(strategy)
            {
                case RoomClaimStrategy.Random:
                    return RoomClaimStrategy_Random(cells, claimRange);
                default:
                    return false;
            }
        }

        public enum RoomClaimStrategy
        {
            Random,
        }

        #region Room Claim Strategies

        private static bool RoomClaimStrategy_Random(List<Cell> cellsToClaim, int claimRangeMaximum)
        {
            var cellsLeftToClaim = cellsToClaim;

            while(cellsLeftToClaim.Any())
            {
                var claimNumberAttempt = Random.Range(1, claimRangeMaximum + 1);
                var claimedCount = 0;
                var targetRootCell = cellsLeftToClaim[Random.Range(0, cellsLeftToClaim.Count())];
                var projection = new RoomProjection();
                var primaryDirection = Directionf.Random(Directionf.Directions());
                var secondaryDireciton = Directionf.Random(new List<Direction>() { primaryDirection.Left(), primaryDirection.Right() });

                //Project 1 1
                Project_1_1(targetRootCell, ref projection, primaryDirection, secondaryDireciton);
                claimedCount++;

                //Project 2 2
                if(claimedCount < claimNumberAttempt && projection.cellProj.Count == 1)
                {
                    Project_2_2(ref projection);
                    claimedCount += projection.cellProj.Where(x => x.level == CellProjectionLevel.room_2_2).Count();
                }

                //Project 3 3
                if (claimedCount < claimNumberAttempt && projection.cellProj.Count == 4)
                {
                    Project_3_3(ref projection);
                    claimedCount += projection.cellProj.Where(x => x.level == CellProjectionLevel.room_3_3).Count();
                }

                //Project 4 4
                if (claimedCount < claimNumberAttempt && projection.cellProj.Count == 9)
                {
                    Project_4_4(ref projection);
                    claimedCount += projection.cellProj.Where(x => x.level == CellProjectionLevel.room_4_4).Count();
                }

                var resultingCells = projection.cellProj.Select(s => s.cell).ToList();
                if (ClaimRoom(resultingCells))
                {
                    resultingCells.ForEach(x => cellsLeftToClaim.Remove(x));
                }
            }

            return true;
        }

        #endregion

        #region Room Projection

        private static bool Project_1_1(Cell root, ref RoomProjection projection, Direction primaryDirection, Direction secondaryDirection)
        {
            projection.primaryDirection = primaryDirection;
            projection.seconaryDirection = secondaryDirection;
            projection.cellProj.Add(new CellProjection() {
                cell = root,
                level = CellProjectionLevel.room_1_1
            });

            return true;
        }

        private static bool Project_2_2(ref RoomProjection projection)
        {

            if (!projection.cellProj.Any(x => x.level == CellProjectionLevel.room_1_1)) return false;

            var test = projection.cellProj.First(x => x.level == CellProjectionLevel.room_1_1).cell.position;
            var root = CellCollection.cells[projection.cellProj.First(x => x.level == CellProjectionLevel.room_1_1).cell.position];

            if(CellCollection.HasCellAt(root.Step(projection.primaryDirection)))
            CellCollection.cells[root.Step(projection.primaryDirection)]
                .TryAddProjection(ref projection, CellProjectionLevel.room_2_2);

            if (CellCollection.HasCellAt(root.Step(projection.seconaryDirection)))
                CellCollection.cells[root.Step(projection.seconaryDirection)]
                .TryAddProjection(ref projection, CellProjectionLevel.room_2_2);

            //Prevent diagonal cell from being claimed without adjacent cells
            if (projection.cellProj.Where(x => x.level == CellProjectionLevel.room_2_2).Count() >= 1)
            {
                if (CellCollection.HasCellAt(root.Step(projection.primaryDirection).Step(projection.seconaryDirection)))
                    CellCollection.cells[root.Step(projection.primaryDirection).Step(projection.seconaryDirection)] //Root for room_4_4 projection
                    .TryAddProjection(ref projection, CellProjectionLevel.room_2_2);
            }

            return true;
        }

        private static bool Project_3_3(ref RoomProjection projection)
        {
            if (projection.cellProj.Where(x => x.level == CellProjectionLevel.room_2_2).Count() != 3) return false;

            var root = CellCollection.cells[projection.cellProj.First(x => x.level == CellProjectionLevel.room_1_1).cell.position];

            //Go backwards to maintain centrality for projections

            if (CellCollection.HasCellAt(root.Step(projection.primaryDirection.Opposite())))
                CellCollection.cells[root.Step(projection.primaryDirection.Opposite())]
                .TryAddProjection(ref projection, CellProjectionLevel.room_3_3);

            var anchor_1 = projection.cellProj.Last();

            if (CellCollection.HasCellAt(root.Step(projection.seconaryDirection.Opposite())))
                CellCollection.cells[root.Step(projection.seconaryDirection.Opposite())]
                .TryAddProjection(ref projection, CellProjectionLevel.room_3_3);

            var anchor_2 = projection.cellProj.Last();

            if (CellCollection.HasCellAt(root.Step(projection.primaryDirection.Opposite()).Step(projection.seconaryDirection)))
                CellCollection.cells[root.Step(projection.primaryDirection.Opposite()).Step(projection.seconaryDirection)]
                    .TryAddProjection(ref projection, CellProjectionLevel.room_3_3);

            if (CellCollection.HasCellAt(root.Step(projection.primaryDirection).Step(projection.seconaryDirection.Opposite())))
            CellCollection.cells[root.Step(projection.primaryDirection).Step(projection.seconaryDirection.Opposite())]
                .TryAddProjection(ref projection, CellProjectionLevel.room_3_3);

            //Prevent diagonal cell from being claimed without adjacent cells
            if (projection.cellProj.Contains(anchor_1) && projection.cellProj.Contains(anchor_2)){

                if (CellCollection.HasCellAt(root.Step(projection.primaryDirection.Opposite()).Step(projection.seconaryDirection.Opposite())))
                    CellCollection.cells[root.Step(projection.primaryDirection.Opposite()).Step(projection.seconaryDirection.Opposite())]
                        .TryAddProjection(ref projection, CellProjectionLevel.room_3_3);
            }

            return true;
        }

        private static bool Project_4_4(ref RoomProjection projection)
        {
            if (projection.cellProj.Where(x => x.level == CellProjectionLevel.room_3_3).Count() != 5) return false;

            //Extend root to the middle of the room_2_2 projection, the last one projected
            var root = CellCollection.cells[projection.cellProj.Last(x => x.level == CellProjectionLevel.room_2_2).cell.position];

            if (CellCollection.HasCellAt(root.Step(projection.primaryDirection)))
            CellCollection.cells[root.Step(projection.primaryDirection)]
                .TryAddProjection(ref projection, CellProjectionLevel.room_4_4);

            var anchor_1 = projection.cellProj.Last();

            if (CellCollection.HasCellAt(root.Step(projection.seconaryDirection)))
            CellCollection.cells[root.Step(projection.seconaryDirection)]
                .TryAddProjection(ref projection, CellProjectionLevel.room_4_4);

            var anchor_2 = projection.cellProj.Last();

            if (CellCollection.HasCellAt(root.Step(projection.primaryDirection).Step(projection.seconaryDirection.Opposite())))
            CellCollection.cells[root.Step(projection.primaryDirection).Step(projection.seconaryDirection.Opposite())]
                .TryAddProjection(ref projection, CellProjectionLevel.room_4_4);

            if (CellCollection.HasCellAt(root.Step(projection.primaryDirection.Opposite()).Step(projection.seconaryDirection)))
            CellCollection.cells[root.Step(projection.primaryDirection.Opposite()).Step(projection.seconaryDirection)]
                .TryAddProjection(ref projection, CellProjectionLevel.room_4_4);

            if (CellCollection.HasCellAt(root.Step(projection.primaryDirection).Step(projection.seconaryDirection.Opposite(), 2)))
            CellCollection.cells[root.Step(projection.primaryDirection).Step(projection.seconaryDirection.Opposite(), 2)]
                .TryAddProjection(ref projection, CellProjectionLevel.room_4_4);

            if (CellCollection.HasCellAt(root.Step(projection.primaryDirection.Opposite(), 2).Step(projection.seconaryDirection)))
            CellCollection.cells[root.Step(projection.primaryDirection.Opposite(), 2).Step(projection.seconaryDirection)]
                .TryAddProjection(ref projection, CellProjectionLevel.room_4_4);

            //Prevent diagonal cell from being claimed without adjacent cells
            if (projection.cellProj.Contains(anchor_1) && projection.cellProj.Contains(anchor_2))
            {
                if (CellCollection.HasCellAt(root.Step(projection.primaryDirection).Step(projection.seconaryDirection)))
                    CellCollection.cells[root.Step(projection.primaryDirection).Step(projection.seconaryDirection)]
                        .TryAddProjection(ref projection, CellProjectionLevel.room_4_4);
            }

            return true;
        }

        private class RoomProjection
        {
            public List<CellProjection> cellProj = new List<CellProjection>();
            public Direction primaryDirection;
            public Direction seconaryDirection;
            public CellProjectionLevel size { get { return cellProj.OrderByDescending(o => o.level).First().level; } }
        }

        private class CellProjection
        {
            public Cell cell;
            public CellProjectionLevel level;
        }

        private enum CellProjectionLevel
        {
            room_1_1 = 0, //1 Cell
            room_2_2 = 1, //3 Cells
            room_3_3 = 2, //5 Cells
            room_4_4 = 3  //7 Cells
        }

        private static void TryAddProjection(this Cell cell, ref RoomProjection result, CellProjectionLevel level)
        {
            if (cell.region == result.cellProj.First().cell.region && !cell.claimedByRoom)
            { result.cellProj.Add(new CellProjection() {
                cell = cell,
                level = level
            }); }
        }

        #endregion
    }
}
