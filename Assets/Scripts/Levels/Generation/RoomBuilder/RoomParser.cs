using Assets.Scripts.Levels.Generation.Base;
using Assets.Scripts.Levels.Generation.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation.RoomBuilder
{
    public static class RoomParser
    {
        public static bool AllowCrossRegionConnections = false;

        [Obsolete]
        public static void ParseDoors_Old()
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
                //Prefer a pathcell if making a connection
                if (room.potentialDoors.Any(x => x.room.cells.Any(y => y.important)))
                {
                    var targetNeighbor = room.potentialDoors.Where(x => x.room.cells.Any(y => y.important));

                    var cells = new List<Cell>();
                    foreach (var cell in targetNeighbor)
                    { cells.Add(cell); }

                    var targetCell = cells[Random.Range(0, cells.Count())];

                    Level.doors.Add(new DoorNode()
                    {
                        cell_1 = targetCell,
                        cell_2 = room.cells.First(x => x.NeighborCellsOutOfRoom().Contains(targetCell))
                    });
                }
                else
                {
                    var potentialNeighbors = room.potentialDoors.GroupBy(g => g.room).ToList();

                    var targetNeighbor = potentialNeighbors[Random.Range(0, potentialNeighbors.Count())];

                    var cells = new List<Cell>();
                    foreach (var cell in targetNeighbor)
                    { cells.Add(cell); }

                    var targetCell = cells[Random.Range(0, cells.Count())];

                    Level.doors.Add(new DoorNode()
                    {
                        cell_1 = targetCell,
                        cell_2 = room.cells.First(x => x.NeighborCellsOutOfRoom().Contains(targetCell))
                    });
                }
            }

            //Establish room connections to ensure all rooms are interconnected
            var roomsWithoutVerifiedPathConnection = RoomCollection.rooms.Where(x => !x.containsPath).ToList();
            while(roomsWithoutVerifiedPathConnection.Any())
            {
                var room = roomsWithoutVerifiedPathConnection.First();
                var connectedRooms = room.connectedRooms; //One of the random neighbors assigned in step 2 of this parsing method
           
                var neighborSuccess = false;
                if(connectedRooms.Any()) //First, check if current connection has a pathway that doesn't intersect with this room
                {
                    var neighbor = connectedRooms.First();
                    if (neighbor.pathConfirmedOverride) {
                        neighborSuccess = true;
                        var targetNeighborCells = room.potentialDoors.Where(x => x.room == neighbor);

                        var cells = new List<Cell>();
                        foreach (var cell in targetNeighborCells)
                        { cells.Add(cell); }

                        var targetCell = cells[Random.Range(0, cells.Count())];

                        Level.doors.Add(new DoorNode()
                        {
                            cell_1 = targetCell,
                            cell_2 = room.cells.First(x => x.NeighborCellsOutOfRoom().Contains(targetCell))
                        });

                        room.pathConfirmedOverride = true;
                    } else {
                        var neighborConnectionToPath = neighbor.SearchForPathRoom(true);
                        if (neighborConnectionToPath.Any() && !neighborConnectionToPath.Contains(room))
                        {
                            neighborSuccess = true;
                            var targetNeighborCells = room.potentialDoors.Where(x => x.room == neighbor);

                            var cells = new List<Cell>();
                            foreach (var cell in targetNeighborCells)
                            { cells.Add(cell); }

                            var targetCell = cells[Random.Range(0, cells.Count())];

                            Level.doors.Add(new DoorNode()
                            {
                                cell_1 = targetCell,
                                cell_2 = room.cells.First(x => x.NeighborCellsOutOfRoom().Contains(targetCell))
                            });

                            room.pathConfirmedOverride = true;
                        }
                    }
                }

                if (!neighborSuccess)
                {
                    var pathConnection = room.SearchForPathRoom(true);

                    if (pathConnection.Any())
                    {
                        var pathRoom = pathConnection.Last();
                        pathConnection.Remove(room);
                        var neighborRoom = pathConnection.First();

                        var targetNeighborCells = room.potentialDoors.Where(x => x.room == neighborRoom);

                        var cells = new List<Cell>();
                        foreach (var cell in targetNeighborCells)
                        { cells.Add(cell); }

                        var targetCell = cells[Random.Range(0, cells.Count())];

                        Level.doors.Add(new DoorNode()
                        {
                            cell_1 = targetCell,
                            cell_2 = room.cells.First(x => x.NeighborCellsOutOfRoom().Contains(targetCell))
                        });

                        room.pathConfirmedOverride = true;
                    }
                }

                roomsWithoutVerifiedPathConnection.Remove(room);
            }
        }

        public static void ParseDoors()
        {
            //Fetch potential doors for all existing rooms
            foreach (var room in RoomCollection.rooms)
            { room.potentialDoors = room.cells.SelectMany(s => s.NeighborCellsOutOfRoom()).ToList(); }

            #region Pathway connections
            //Establish pathway connections between path rooms
            foreach (var room in RoomCollection.rooms.Where(x => x.containsPath))
            {
                var pathNeighbors = room.potentialDoors.Where(x => x.room.containsPath).GroupBy(g => g.room);
                foreach(var neighbor in pathNeighbors)
                {
                    if (!AllowCrossRegionConnections && room.connectedRooms.Count == 2) break;

                    var cells = neighbor.Select(s => s).ToList();
                    var targetCell = cells[Random.Range(0, cells.Count)];
                    CreateDoor(targetCell, room.cells.First(x => x.NeighborCellsOutOfRoom().Contains(targetCell)));
                }
                room.pathConfirmedOverride = true;
            }
            #endregion

            #region Other connections
            var unconfirmedRooms = RoomCollection.rooms.Where(x => !x.pathConfirmedOverride).ToList();
            while(unconfirmedRooms.Any())
            {
                var currentRoom = unconfirmedRooms.First(x => x.potentialDoors.Select(s => s.room).Any(y => y.pathConfirmedOverride));
                var neighborWithConfirmedPath = currentRoom.potentialDoors.GroupBy(g => g.room).First(x => x.Key.pathConfirmedOverride);
                var neighborCells = neighborWithConfirmedPath.Where(x => currentRoom.potentialDoors.Contains(x)).ToList();

                var targetCell = neighborCells[Random.Range(0, neighborCells.Count)];
                var cell = currentRoom.cells.First(x => x.NeighborCellsOutOfRoom().Contains(targetCell));

                CreateDoor(cell, targetCell);

                currentRoom.pathConfirmedOverride = true;

                unconfirmedRooms.Remove(currentRoom);
            }
            #endregion
        }

        private static void CreateDoor(Cell cell_1, Cell cell_2)
        {
            Level.doors.Add(new DoorNode()
            {
                cell_1 = cell_1,
                cell_2 = cell_2
            });
        }
    }
}
