using Assets.Scripts.Levels.Generation.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Levels.Generation.Extensions
{
    public static class Roomf
    {
        public static List<Room> NeighborRooms(this Room room)
        {
            var result = new List<Room>();

            foreach (var cell in room.GetCells())
            {
                var neighbors = cell.NeighborCellsOutOfRoom();
                foreach (var neighborRoom in neighbors.Select(s => s.GetRoom()))
                {
                    if (!result.Contains(neighborRoom))
                    {
                        result.Add(neighborRoom);
                    }
                }
            }

            return result;
        }

        public static List<Room> ConnectedRooms(this Room room)
        {
            var result = new List<Room>();

            foreach (var door in room.doors.ToArray())
            {
                var otherRoom = door.cell_1.roomId == room.id ? door.cell_2.roomId : door.cell_1.roomId;
                result.Add(RoomCollection.rooms[otherRoom]);
            }

            return result;

            //var result = new List<Room>();

            //foreach(var doorNode in Level.doors.Where(x => room.cells.Contains(x.cell_1) || room.cells.Contains(x.cell_2)))
            //{
            //    Room otherRoom;

            //    if(room.cells.Contains(doorNode.cell_1))
            //    {
            //        otherRoom = doorNode.cell_2.room;
            //    } else
            //    {
            //        otherRoom = doorNode.cell_1.room;
            //    }

            //    if(!result.Contains(otherRoom))
            //    {
            //        result.Add(otherRoom);
            //    }
            //}

            //return result;
        }

        public static List<Cell> GetBlockableCells(this Room room)
        {
            return CellCollection.GetByRoom(room.id).Where(x => !x.mustNotBeBlocked).ToList();
        }

        [Obsolete]
        public static bool VerifyCellCollectionDoesNotBlockRoomPathways(this Room room, List<Cell> blockingCells, List<Cell> additionalImportantCells)
        {
            var importantCellsInRoom = CellCollection.GetByRoom(room.id).Where(x => x.mustNotBeBlocked).ToList();
            importantCellsInRoom.AddRange(additionalImportantCells);

            return Verify_Step(importantCellsInRoom.First(), importantCellsInRoom, blockingCells, new List<Cell>(), new List<Cell>());
        }

        [Obsolete]
        private static bool Verify_Step(this Cell root, List<Cell> importantCells, List<Cell> blockingCells, List<Cell> searchedCells, List<Cell> foundImportantCells)
        {
            var foundInstance = foundImportantCells;

            searchedCells.Add(root);

            if (importantCells.Contains(root))
            { foundImportantCells.Add(root); }

            //Found all the cells, we're good.
            if (importantCells.All(x => foundInstance.Contains(x))) return true;

            var nextCellsToCheck = root.NeighborCellsInRoom()
                .Where(x => !blockingCells.Contains(x) && !searchedCells.Contains(x));

            foreach (var nextCell in nextCellsToCheck)
            {
                var result = Verify_Step(nextCell, importantCells, blockingCells, searchedCells, foundImportantCells);

                if (result) return true;
            }

            return false;
        }
    }
}