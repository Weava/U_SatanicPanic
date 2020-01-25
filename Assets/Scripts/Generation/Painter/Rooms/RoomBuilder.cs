using Assets.Scripts.Generation.Blueprinting.BlueprintFactories;
using Assets.Scripts.Generation.Blueprinting.Blueprints;
using Assets.Scripts.Generation.Extensions;
using Assets.Scripts.Generation.Painter.Cells.Base;
using Assets.Scripts.Generation.Painter.Rooms.Base;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Generation.Painter.Rooms
{
    public static class RoomBuilder
    {
        #region Claiming

        public static List<Room> ClaimRooms(this List<Cell> targetCells, ClaimType claimType, RoomOptions options)
        {
            switch(claimType)
            {
                case ClaimType.Greedy:
                    return ClaimRooms_Greedy(targetCells, options);
                case ClaimType.SequencedGreedy:
                    return ClaimRooms_SequencedGreedy(targetCells, options);
                default:
                    return new List<Room>();
            }
        }

        public static List<Room> ClaimRooms_Greedy(this List<Cell> targetCells, RoomOptions options)
        {
            var result = new List<Room>();

            if(!options.excludeRoomSize.Contains(RoomSize.Room_4_4))
            foreach (var cell in targetCells.Where(x => !x.claimed))
            {
                var room = TryClaimRoom(cell, RoomSize.Room_4_4, options);
                if (room != null) { result.Add(room); }
            }

            if (!options.excludeRoomSize.Contains(RoomSize.Room_3_3))
                foreach (var cell in targetCells.Where(x => !x.claimed))
            {
                if (cell.claimed) continue;
                var room = TryClaimRoom(cell, RoomSize.Room_3_3, options);
                if (room != null) { result.Add(room); }
            }

            if (!options.excludeRoomSize.Contains(RoomSize.Room_2_3))
                foreach (var cell in targetCells.Where(x => !x.claimed))
            {
                if (cell.claimed) continue;
                var room = TryClaimRoom(cell, RoomSize.Room_2_3, options);
                if (room != null) { result.Add(room); }
            }

            if (!options.excludeRoomSize.Contains(RoomSize.Room_2_2))
                foreach (var cell in targetCells.Where(x => !x.claimed))
            {
                if (cell.claimed) continue;
                var room = TryClaimRoom(cell, RoomSize.Room_2_2, options);
                if (room != null) { result.Add(room); }
            }

            if (!options.excludeRoomSize.Contains(RoomSize.Room_1_2))
                foreach (var cell in targetCells.Where(x => !x.claimed))
            {
                if (cell.claimed) continue;
                var room = TryClaimRoom(cell, RoomSize.Room_1_2, options);
                if (room != null) { result.Add(room); }
            }

            foreach (var cell in targetCells.Where(x => !x.claimed))
            {
                if (cell.claimed) continue;
                var room = TryClaimRoom(cell, RoomSize.Room_1_1, options);
                if (room != null) { result.Add(room); }
            }

            RoomCollection.collection.AddRange(result);

            return result;
        }

        public static List<Room> ClaimRooms_SequencedGreedy(this List<Cell> targetCells, RoomOptions options)
        {
            var result = new List<Room>();

            var pathCells = targetCells.Where(x => x.cellType == CellType.Path_Cell).OrderBy(o => o.pathSequence).ToList();
            foreach(var pathCell  in pathCells)
            {
                if (!pathCell.claimed)
                    result.AddRange(ClaimRooms_Greedy(new List<Cell>() { pathCell }, options));
            }

            var remainingCells = targetCells.Where(x => !x.claimed).ToList();
            result.AddRange(ClaimRooms_Greedy(remainingCells, options));

            return result;
        }

        private static Room TryClaimRoom(Cell cell, RoomSize roomSize, RoomOptions options)
        {
            foreach (var direction in Directionf.GetDirectionList())
            {
                var projection = Roomf.ProjectRoom(cell.position, direction, roomSize, options);
                if (projection != null)
                    Roomf.ClaimForRoom(projection, roomSize, direction, options);
            }

            return null;
        }

        #endregion

        #region Context

        public static void BuildPathContext(string region)
        {
            var rooms = RoomCollection.collection.Where(x => x.region == region);
            foreach(var room in rooms)
            {
                room.BuildPathContext();
            }
        }

        public static void BuildPathContext(this Room room)
        {
            var cells = room.cells.Select(s => s.Value);

            var pathCells = cells.Where(x => x.cellType == CellType.Path_Cell).OrderBy(o=>o.pathSequence);
            if(pathCells.Any()) //Path cells always connect to another path cell, either from the same region or another region
            {
                room.pathRoom = true;
                var intoRoomCell = pathCells.First();
                var outOfRoomCell = pathCells.Last();
                if(intoRoomCell == outOfRoomCell)
                {
                    var outsidePathNeighbors = intoRoomCell.NeighborCells().Where(x => x.room != room && x.cellType == CellType.Path_Cell);
                    var neighborIntoRoom = outsidePathNeighbors.FirstOrDefault(x => x.globalSequence == intoRoomCell.globalSequence - 1);
                    var neighborOutOfRoom = outsidePathNeighbors.FirstOrDefault(x => x.globalSequence == intoRoomCell.globalSequence + 1);
                    if(neighborIntoRoom != null)
                    { Cellf.EstablishConnection(intoRoomCell, neighborIntoRoom, new CellConnectionOptions() { doorType = Cellf.GetRandomDoorType() }); }
                    if (neighborOutOfRoom != null)
                    { Cellf.EstablishConnection(intoRoomCell, neighborOutOfRoom, new CellConnectionOptions() { doorType = Cellf.GetRandomDoorType() }); }
                } else
                {
                    var outsidePathNeighbors_ForIntoRoom = intoRoomCell.NeighborCells().Where(x => x.room != room && x.cellType == CellType.Path_Cell);
                    var outsidePathNeighbors_ForOutOfRoom = outOfRoomCell.NeighborCells().Where(x => x.room != room && x.cellType == CellType.Path_Cell);
                    var neighborIntoRoom = outsidePathNeighbors_ForIntoRoom.FirstOrDefault(x => x.globalSequence == intoRoomCell.globalSequence - 1);
                    var neighborOutOfRoom = outsidePathNeighbors_ForOutOfRoom.FirstOrDefault(x => x.globalSequence == outOfRoomCell.globalSequence + 1);
                    if (neighborIntoRoom != null)
                    { Cellf.EstablishConnection(intoRoomCell, neighborIntoRoom, new CellConnectionOptions() { doorType = Cellf.GetRandomDoorType() }); }
                    if (neighborOutOfRoom != null)
                    { Cellf.EstablishConnection(outOfRoomCell, neighborOutOfRoom, new CellConnectionOptions() { doorType = Cellf.GetRandomDoorType() }); }
                }
            }
        }

        public static void BuildNonPathContext(string region, RoomContextOptions options = null)
        {
            var rooms = RoomCollection.collection.Where(x => x.region == region && !x.pathRoom).ToList();
            var roomsWithoutDoors = rooms.Where(x => !x.DoorCells.Any());

            while (roomsWithoutDoors.Any())
            {
                var room = roomsWithoutDoors.First(x => x.NeighborRooms().Any(n => n.DoorCells.Any()));
                var neighborWithConnection = room.NeighborRooms().First(x => x.DoorCells.Any());
                var roomCellsNextToThatNeighbor = room.cells.Select(s => s.Value).Where(x => x.NeighborCells().Any(n => n.room == neighborWithConnection)).ToList();

                var cellToMakeDoorWith = roomCellsNextToThatNeighbor[Random.Range(0, roomCellsNextToThatNeighbor.Count)];
                var neighborCell = cellToMakeDoorWith.NeighborCells().First(x => x.room == neighborWithConnection);

                Cellf.EstablishConnection(cellToMakeDoorWith, neighborCell, new CellConnectionOptions() { doorType = Cellf.GetRandomDoorType() });

                roomsWithoutDoors = rooms.Where(x => !x.DoorCells.Any());
            }

            if(options.generateAdditionalDoors)
            {
                var regionRooms = RoomCollection.collection.Where(x => x.region == region);
                var cells = regionRooms.SelectMany(x => x.cells).Select(s => s.Value);
                foreach(var cell in cells)
                {
                    var neighbors = cell.NeighborCells().Where(x => x.room != cell.room 
                    && x.Region == region
                    && !x.connections.Any(y => y.connectedCell == cell)
                    && !cell.room.DoorCells.Any(y => y.connections.Any(s => s.connectedCell.room == x.room)));
                    foreach (var neighbor in neighbors)
                    {
                        if (Random.Range(0.0f, 1.0f) <= options.doorChance)
                        {
                            Cellf.EstablishConnection(cell, neighbor, new CellConnectionOptions() { doorType = Cellf.GetRandomDoorType() });
                        }
                    }
                }
            }
        }

        #endregion

        #region Blueprinting

        public static void BuildBlueprints()
        {
            foreach(var room in RoomCollection.collection)
            {
                if(room.roomSize == RoomSize.Room_1_1)
                {
                    room.blueprint = new Blueprint_Room_1_1(room);
                }
            }
        }

        #endregion
    }

    public class RoomContextOptions
    {
        public bool generateAdditionalDoors;

        public float doorChance;
    }

    public enum ClaimType
    {
        Greedy,
        SequencedGreedy
    }
}
