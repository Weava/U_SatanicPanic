using Assets.Scripts.Levels.Generation.Base;
using Assets.Scripts.Levels.Generation.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation.RoomBuilder
{
    public static class RoomParser
    {
        public static void ParseDoors()
        {
            foreach(var room in RoomCollection.rooms)
            {
                room.potentialDoors = room.cells.SelectMany(s => s.NeighborCellsOutOfRoom()).ToList();
            }

            //Establish important room connections
            foreach(var room in RoomCollection.rooms)
            {
                if(room.containsPath)
                {
                    var cellsConnectingToNeighborPath = room.potentialDoors.Where(x => x.room.containsPath);
                    var cellsGroupedByRoom = cellsConnectingToNeighborPath.GroupBy(g => g.room).ToList();
                    //Group cells by neighboring room
                    foreach(var roomDesignation in cellsGroupedByRoom)
                    {
                        var cells = new List<Cell>();
                        foreach(var cell in roomDesignation)
                        { cells.Add(cell); }

                        //Pick one of the cells at random to assign a door to
                        var targetCell = cells[Random.Range(0, cells.Count())];

                        if(!room.connectedRooms.Any(x => x.cells.Contains(targetCell)))
                        {
                            Level.doors.Add(new DoorNode() {
                                cell_1 = targetCell,
                                cell_2 = room.cells.First(x => x.NeighborCellsOutOfRoom().Contains(targetCell))
                            });
                        }
                    }
                }
            }

            //Establish other room connections - Non-critical pass
            foreach(var room in RoomCollection.rooms.Where(x => !x.connectedRooms.Any()).ToArray())
            {
                var potentialNeighbors = room.potentialDoors.GroupBy(g => g.room).ToList();

                var targetNeighbor = potentialNeighbors[Random.Range(0, potentialNeighbors.Count())];

                var cells = new List<Cell>();
                foreach(var cell in targetNeighbor)
                { cells.Add(cell); }

                var targetCell = cells[Random.Range(0, cells.Count())];

                Level.doors.Add(new DoorNode() {
                    cell_1 = targetCell,
                    cell_2 = room.cells.First(x => x.NeighborCellsOutOfRoom().Contains(targetCell))
                });
            }

            //Establish room connections to ensure all rooms are interconnected
            var roomsWithoutVerifiedPathConnection = RoomCollection.rooms.Where(x => !x.containsPath).ToList();
            while(roomsWithoutVerifiedPathConnection.Any())
            {
                var unverifiedRoom = roomsWithoutVerifiedPathConnection[Random.Range(0, roomsWithoutVerifiedPathConnection.Count())];
                var roomTrailToPath = unverifiedRoom.SearchForPathRoom();

                //TODO: Make this work
            }
        }
    }
}
