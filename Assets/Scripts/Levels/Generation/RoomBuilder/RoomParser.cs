using Assets.Scripts.Levels.Generation.Base;
using Assets.Scripts.Levels.Generation.Base.Mono;
using Assets.Scripts.Levels.Generation.Extensions;
using Assets.Scripts.Misc;
using System;
using System.Collections.Generic;
using System.Linq;

using Random = UnityEngine.Random;

namespace Assets.Scripts.Levels.Generation.RoomBuilder
{
    public static class RoomParser
    {
        public const int CELL_PARTIAL_OFFSET = 3;
        public const int CEILING_OFFSET = 4;

        #region Room Claiming

        public static void ClaimRooms(Region region)
        {
            //Until all cells are claimed by a room
            while(region.cells.Any(x => ! x.claimedByRoom))
            {
                var cellsLeftToClaim = region.cells.Where(x => !x.claimedByRoom).ToList();
                var rootCell = cellsLeftToClaim[Random.Range(0, cellsLeftToClaim.Count)];

                var projection = ProjectRoom(rootCell, ref cellsLeftToClaim, Random.Range(1, region.maximumRoomSize+1), region.claimChance, region.roomClaimingStrategy);
                if(projection.Any())
                {
                    var room = new Room();
                    if(!ClaimRoom(projection, ref room))
                    { continue; }
                    region.rooms.Add(room);
                    RoomCollection.rooms.Add(room);
                }
            }
        }

        #region Room Projection Strategies

        private static List<Cell> ProjectRoom(Cell root, ref List<Cell> cellsLeftToClaim, int claimAmount, float claimChange, RoomClaimingStrategy strategy)
        {
            switch(strategy)
            {
                case RoomClaimingStrategy.Bloom:
                    return ProjectRoom_Bloom(root, ref cellsLeftToClaim, claimAmount);
                case RoomClaimingStrategy.PartialBloom:
                    return ProjectRoom_PartialBloom(root, ref cellsLeftToClaim, claimAmount, claimChange);
                default:
                    throw new Exception("A strategy must be assigned to a region.");
            }
        }

        private static List<Cell> ProjectRoom_Bloom(Cell root, ref List<Cell> cellsLeftToClaim, int claimAmount)
        {
            var result = new List<Cell>() { root };
            var claimedAmount = result.Count;

            var currentRoots = new List<Cell> { root };

            while(claimedAmount < claimAmount)
            {
                var nextRoots = new List<Cell>();
                foreach (var currentRoot in currentRoots.ToList())
                {
                    foreach (var direction in Directionf.Directions().Shuffle())
                    {
                        var target = currentRoot.Step(direction);
                        if (CellCollection.HasCellAt(target) && cellsLeftToClaim.Any(x => x.position == target))
                        {
                            nextRoots.Add(CellCollection.cells[target]);
                            cellsLeftToClaim.Remove(CellCollection.cells[target]);
                            claimedAmount++;
                            result.Add(CellCollection.cells[target]);
                        }
                    }
                    if(!nextRoots.Any()) //Ran out of cells to claim, just take what we got
                    {  return result; }
                }
                currentRoots = nextRoots;
            }

            return result;
        }

        private static List<Cell> ProjectRoom_PartialBloom(Cell root, ref List<Cell> cellsLeftToClaim, int claimAmount, float claimChance)
        {
            var result = new List<Cell>() { root };
            var claimedAmount = result.Count;

            var currentRoots = new List<Cell> { root };

            while (claimedAmount < claimAmount)
            {
                var nextRoots = new List<Cell>();
                foreach (var currentRoot in currentRoots.ToList())
                {
                    foreach (var direction in Directionf.Directions().Shuffle())
                    {
                        var target = currentRoot.Step(direction);
                        if (CellCollection.HasCellAt(target) && cellsLeftToClaim.Any(x => x.position == target))
                        {
                            var chanceRoll = Random.Range(0.0f, 1.0f);
                            if (chanceRoll <= claimChance)
                            {

                                nextRoots.Add(CellCollection.cells[target]);
                                cellsLeftToClaim.Remove(CellCollection.cells[target]);
                                claimedAmount++;
                                result.Add(CellCollection.cells[target]);
                            }
                            else
                            {
                                claimedAmount--;
                            }
                        }
                    }
                    if (!nextRoots.Any()) //Ran out of cells to claim, just take what we got
                    { return result; }
                }
                currentRoots = nextRoots;
            }

            return result;
        }

        #endregion

        #region Helper Methods

        private static bool ClaimRoom(List<Cell> cells, ref Room room)
        {
            /*Rooms can only exist within one region*/
            if (cells.Select(s => s.region).Distinct().Count() > 1) { return false; }

            /*A room can only contain a complete sequence of sequenced cells*/
            if (cells.Any(x => x.type != CellType.Cell))
            {
                var sequencedCells = cells.Where(x => x.type != CellType.Cell).OrderBy(o => o.sequence).ToList();
                for (int i = 0; i < sequencedCells.Count() - 1; i++)
                {
                    if (sequencedCells[i].sequence != sequencedCells[i + 1].sequence - 1)
                    { return false; }
                }
            }

            room.cells = cells;

            foreach (var cell in cells)
            {
                cell.room = room;
            }

            return true;
        }

        #endregion

        #endregion

        #region Door Parsing

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
                            Level.doors.Add(new Node_Door() {
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

                    Level.doors.Add(new Node_Door()
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

                    Level.doors.Add(new Node_Door()
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

                        Level.doors.Add(new Node_Door()
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

                            Level.doors.Add(new Node_Door()
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

                        Level.doors.Add(new Node_Door()
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

        public static void ParseDoors(Region region)
        {
            //Pathway door pass - Connect each important pathway room when they meetup
            #region Pathway Pass
            foreach (var importantRoom in region.rooms.Where(x => x.containsPath).ToArray())
            {
                importantRoom.pathConfirmedOverride = true;
                var importantNeighbors = importantRoom.neighborRooms.Where(x => x.containsPath);
                foreach(var neighbor in importantNeighbors)
                {
                    if (neighbor.connectedRooms.Count > 0 && neighbor.cells.Any(x => x.type == CellType.Elevation)) continue;
                    if (neighbor.connectedRooms.Any(x => x == importantRoom)) continue;

                    var potentialDoors = importantRoom.cells.Where(x => x.NeighborCellsOutOfRoom(true).Any(a => a.room == neighbor)).ToList();

                    if(importantRoom.connectedRooms.Count > 1 && potentialDoors.Any(x => x.type == CellType.Elevation)
                        || potentialDoors.Any(x => x.NeighborCellsOutOfRoom(true).Any(y => y.type == CellType.Elevation && y.room == neighbor && neighbor.connectedRooms.Count > 1))){
                        potentialDoors.Where(x => x.type == CellType.Elevation).ToList().ForEach(x => potentialDoors.Remove(x));
                    }

                    if (!potentialDoors.Any()) continue;

                    var selectDoor = potentialDoors[Random.Range(0, potentialDoors.Count)];
                    var door = CreateDoor(selectDoor, selectDoor.NeighborCellsOutOfRoom(true).First(x => x.room == neighbor));
                    neighbor.doors.Add(door);
                    importantRoom.doors.Add(door);
                    neighbor.pathConfirmedOverride = true;
                }
            }
            #endregion


            //Other room pass
            #region Other Room Pass
            var roomsLeft = region.rooms.Where(x => x.connectedRooms.Count == 0).ToList();
            var retries = 10;
            while (roomsLeft.Any() && retries > 0)
            {
                var roomsNextToPathwayConfirmed = roomsLeft.Where(x => x.neighborRooms.Any(y => y.pathConfirmedOverride)).ToList();
                if (!roomsNextToPathwayConfirmed.Any()) { retries--; continue; };
                var rootRoom = roomsNextToPathwayConfirmed[Random.Range(0, roomsNextToPathwayConfirmed.Count)];

                var neighbors = rootRoom.neighborRooms.Where(x => x.pathConfirmedOverride).ToList();
                var targetNeighbor = neighbors[Random.Range(0, neighbors.Count)];

                var potentialDoors = rootRoom.cells.Where(x => x.NeighborCellsOutOfRoom().Any(a => a.room == targetNeighbor)
                && x.type != CellType.Elevation).ToList();

                if (!potentialDoors.Any()) { retries--; continue; };

                var targetDoor = potentialDoors[Random.Range(0, potentialDoors.Count)];
                var targetNeighborDoor = targetDoor.NeighborCellsOutOfRoom().First(x => x.room == targetNeighbor);

                var door = CreateDoor(targetDoor, targetNeighborDoor);
                rootRoom.doors.Add(door);
                targetNeighbor.doors.Add(door);
                rootRoom.pathConfirmedOverride = true;
                targetNeighbor.pathConfirmedOverride = true;

                roomsLeft = region.rooms.Where(x => x.connectedRooms.Count == 0).ToList();
            }
            #endregion
        }

        private static Node_Door CreateDoor(Cell cell_1, Cell cell_2)
        {
            var room_1 = RoomCollection.rooms.First(x => x.cells.Contains(cell_1));
            var room_2 = RoomCollection.rooms.First(x => x.cells.Contains(cell_2));

            var door = new Node_Door()
            {
                cell_1 = cell_1,
                cell_2 = cell_2
            };

            room_1.doors.Add(door);
            room_2.doors.Add(door);

            Level.doors.Add(door);

            return door;
        }

        #endregion

        #region Room Parsing
        public static void ParseRoomNodes()
        {
            foreach(var room in RoomCollection.rooms)
            {
                var scaffold = new Scaffold();

                //TODO: Bug where elevations that are lowers does not render correctly
                Elevation_Parse(room, ref scaffold);

                Floor_ParseMain(room, ref scaffold);
                Floor_ParseConnectors(room, ref scaffold);
                Floor_ParseColumns(room, ref scaffold);

                Wall_ParseMain(room, ref scaffold);
                Wall_ParseConnector(room, ref scaffold);

                Ceiling_Parse(room, ref scaffold);

                Level.roomScaffolds.Add(room, scaffold);
            }
            CleanScaffolding();
        }

        private static void Elevation_Parse(Room room, ref Scaffold scaffold)
        {
            foreach (var cell in room.cells.Where(x => x.type == CellType.Elevation))
            {
                //Upper Scan
                if (CellCollection.HasCellAt(cell.Step(Direction.Up)))
                {
                    var upper = CellCollection.cells[cell.Step(Direction.Up)];
                    if (!scaffold.elevation.Any(x => x.lower == cell))
                    {
                        var node = new Node_Elevation();
                        node.lower = cell;
                        node.upper = upper;
                        node.position = upper.position;
                        cell.elevationOverride_Lower = true;
                        upper.elevationOverride_Upper = true;
                        scaffold.elevation.Add(node);
                    }
                }

                //Lower Scan
                if (CellCollection.HasCellAt(cell.Step(Direction.Down)))
                {
                    var lower = CellCollection.cells[cell.Step(Direction.Down)];
                    if (!scaffold.elevation.Any(x => x.upper == cell))
                    {
                        var node = new Node_Elevation();
                        node.upper = cell;
                        node.lower = lower;
                        node.position = cell.position;
                        lower.elevationOverride_Lower = true;
                        cell.elevationOverride_Upper = true;
                        scaffold.elevation.Add(node);
                    }
                }
            }
        }

        #region Floor

        //Floor Parse
        private static void Floor_ParseMain(Room room, ref Scaffold scaffold)
        {
            foreach(var cell in room.cells)
            {
                var node = new Node_FloorMain();
                node.position = cell.position;
                node.root = cell;
                scaffold.floor.main.Add(node);
            }
        }

        //Connector Parse
        private static void Floor_ParseConnectors(Room room, ref Scaffold scaffold)
        {
            foreach(var cell in room.cells)
            {
                var neighborsInRoom = cell.NeighborCellsInRoom();
                foreach(var neighbor in neighborsInRoom)
                {
                    if( ! scaffold.floor.connectors.Any(x => x.position == cell.PositionBetween(neighbor)))
                    {
                        var node = new Node_FloorConnector();
                        node.position = cell.PositionBetween(neighbor);
                        node.rootCells = new List<Cell>() { cell, neighbor };
                        scaffold.floor.connectors.Add(node);
                    }
                }
            }
        }

        //Column Parse
        private static void Floor_ParseColumns(Room room, ref Scaffold scaffold)
        {
            if (room.cells.Count < 4) return; //Columns cannot show up in rooms with less than 4 cells

            foreach(var cell in room.cells)
            {
                foreach(var direction in Directionf.Directions())
                {
                    var cellGrouping = new List<Cell>();

                    if(ColumnScan(cell, ref cellGrouping, direction))
                    {
                        var node = new Node_FloorColumn();
                        node.position = Cellf.PositionBetween(cellGrouping);
                        node.roots = cellGrouping;
                        scaffold.floor.columns.Add(node);
                    }
                }
            }
        }

        private static bool ColumnScan(Cell root, ref List<Cell> cellGrouping, Direction startingDirection)
        {
            var currentDirection = startingDirection;
            var currentCell = root;
            var result = new List<Cell>();

            //A circle has 4 steps of scanning
            for(int i = 0; i < 4; i++)
            {
                if(CellCollection.HasCellAt(currentCell.Step(currentDirection)))
                {
                    var nextCell = CellCollection.cells[currentCell.Step(currentDirection)];
                    if (!nextCell.HasSameRoom(root)) break;
                    currentDirection = currentDirection.Right();
                    result.Add(currentCell);
                    currentCell = nextCell;
                } else
                { break; }
            }

            if (result.Count == 4) {
                cellGrouping = result;
                return true;
            }

            return false;
        }

        #endregion

        #region Wall

        private static void Wall_ParseMain(Room room, ref Scaffold scaffold)
        {
            foreach(var cell in room.cells)
            {
                foreach(var direction in Directionf.Directions())
                {
                    if(CellCollection.HasCellAt(cell.Step(direction)))
                    {
                        var neighbor = CellCollection.cells[cell.Step(direction)];
                        if(neighbor.room != cell.room)
                        {
                            RenderWallNode(cell, direction, ref scaffold);
                        }
                    } else
                    {
                        RenderWallNode(cell, direction, ref scaffold);
                    }
                }
            }
        }
  
        private static void RenderWallNode(Cell cell, Direction direction, ref Scaffold scaffold)
        {
            if (Level.doors.Any(x => x.cell_1 == cell || x.cell_2 == cell))
            {
                var door = Level.doors.FirstOrDefault(x => (x.cell_1 == cell || x.cell_2 == cell) && x.ProjectDirection(cell) == direction);
                if (door != null)
                {
                    var otherCell = door.cell_1 == cell ? door.cell_2 : door.cell_1;
                    if (cell.Step(direction) == otherCell.position) return;
                }
            }

            var node = new Node_WallMain();
            node.position = cell.position + (direction.ToVector() * CELL_PARTIAL_OFFSET);
            node.root = cell;
            node.direction = direction;
            scaffold.wall.main.Add(node);
        }

        private static void Wall_ParseConnector(Room room, ref Scaffold scaffold)
        {
            foreach(var connector in scaffold.floor.connectors)
            {
                var rootNormal = connector.rootCells.First().DirectionToNeighbor(connector.rootCells.Last());
                var directionsToTry = Directionf.Directions();
                directionsToTry.Remove(rootNormal);
                directionsToTry.Remove(rootNormal.Opposite());

                foreach(var direction in directionsToTry)
                {
                    RenderWallConnectorNode(connector, direction, ref scaffold);
                }
            }
        }

        private static void RenderWallConnectorNode(Node_FloorConnector node, Direction direction, ref Scaffold scaffold)
        {
            var knownDoorsForRoots = Level.doors.Where(x => node.rootCells.Contains(x.cell_1) || node.rootCells.Contains(x.cell_2));
            knownDoorsForRoots = knownDoorsForRoots.Where(x => x.ProjectDirection(node.rootCells.First()) == direction 
            || x.ProjectDirection(node.rootCells.Last()) == direction);
            var knownWallsForRoots = scaffold.wall.main.Where(x => node.rootCells.Contains(x.root) && x.direction == direction);

            if(knownDoorsForRoots.Any() || knownWallsForRoots.Any())
            {
                var connectorNode = new Node_WallConnector();
                connectorNode.position = node.position + (direction.ToVector() * CELL_PARTIAL_OFFSET);
                connectorNode.root = node;
                connectorNode.direction = direction;
                scaffold.wall.connectors.Add(connectorNode);
            }
        }

        #endregion

        #region Ceiling

        //Ceiling Parse
        private static void Ceiling_Parse(Room room, ref Scaffold scaffold)
        {
            foreach(var main in scaffold.floor.main)
            {
                var node = new Node_CeilingMain();
                node.position = main.position + (Direction.Up.ToVector());
                node.root = main;
                scaffold.ceiling.main.Add(node);
            }

            foreach (var connector in scaffold.floor.connectors)
            {
                var node = new Node_CeilingConnector();
                node.position = connector.position + (Direction.Up.ToVector());
                node.root = connector;
                scaffold.ceiling.connectors.Add(node);
            }

            foreach (var column in scaffold.floor.columns)
            {
                var node = new Node_CeilingColumn();
                node.position = column.position + (Direction.Up.ToVector());
                node.root = column;
                scaffold.ceiling.columns.Add(node);
            }
        }

        #endregion

        private static void CleanScaffolding()
        {
            
        }

        #endregion
    }
}
