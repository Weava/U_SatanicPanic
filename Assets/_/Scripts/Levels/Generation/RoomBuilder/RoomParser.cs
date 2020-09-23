using Assets.Scripts.Levels.Generation.Base;
using Assets.Scripts.Levels.Generation.Base.Mono;
using Assets.Scripts.Levels.Generation.Extensions;
using Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Parsing.Base;
using Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding;
using Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding.Base;
using Assets.Scripts.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Levels.Generation.RoomBuilder
{
    public static class RoomParser
    {
        public const int CELL_PARTIAL_OFFSET = 3;
        public const int CEILING_OFFSET = 4;

        #region Room Claiming

        public static void ClaimRooms(this Region region)
        {
            //Elevation cells are grouped into their own room to prevent complications
            while (region.GetCells().Where(x => x.type == CellType.Elevation).Any(x => !x.claimedByRoom))
            {
                var elevationGroups = region.GetCells().Where(x => x.type == CellType.Elevation)
                    .GroupBy(g => new Vector2(g.position.x, g.position.y));

                foreach (var elevationGroup in elevationGroups)
                {
                    var room = new Room();
                    if (!ClaimRoom(elevationGroup.ToList(), ref room))
                    { continue; }
                    room.regionId = region.id;
                    RoomCollection.Add(room);
                }
            }

            //Until all cells are claimed by a room
            while (region.GetCells().Any(x => !x.claimedByRoom))
            {
                var cellsLeftToClaim = region.GetCells().Where(x => !x.claimedByRoom).ToList();
                var rootCell = cellsLeftToClaim[Random.Range(0, cellsLeftToClaim.Count)];
                var claimSize = region.greedyClaiming ? region.maximumRoomSize : Random.Range(1, region.maximumRoomSize + 1);

                var projection = ProjectRoom(rootCell, ref cellsLeftToClaim, Random.Range(1, region.maximumRoomSize + 1), region.claimChance, region.roomClaimingStrategy);
                if (projection.Any())
                {
                    var room = new Room();
                    if (!ClaimRoom(projection, ref room))
                    { continue; }
                    room.regionId = region.id;
                    RoomCollection.Add(room);
                }
            }
        }

        #region Room Projection Strategies

        private static List<Cell> ProjectRoom(Cell root, ref List<Cell> cellsLeftToClaim, int claimAmount, float claimChance, RoomClaimingStrategy strategy)
        {
            switch (strategy)
            {
                case RoomClaimingStrategy.Bloom:
                    return ProjectionStrategies.Projection_Bloom.Project(root, ref cellsLeftToClaim, claimAmount);

                case RoomClaimingStrategy.PartialBloom:
                    return ProjectionStrategies.Projection_PartialBloom.Project(root, ref cellsLeftToClaim, claimAmount, claimChance);

                case RoomClaimingStrategy.LimitedStep:
                    return ProjectionStrategies.Projection_LimitedStep.Project(root, ref cellsLeftToClaim, claimAmount, claimChance);

                case RoomClaimingStrategy.Deterministic:
                    return ProjectionStrategies.Projection_Deterministic.Project(root, ref cellsLeftToClaim);

                default:
                    throw new Exception("A strategy must be assigned to a region.");
            }
        }

        [Obsolete]
        private static List<Cell> ProjectRoom_Bloom(Cell root, ref List<Cell> cellsLeftToClaim, int claimAmount)
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
                            nextRoots.Add(CellCollection.cells[target]);
                            cellsLeftToClaim.Remove(CellCollection.cells[target]);
                            claimedAmount++;
                            result.Add(CellCollection.cells[target]);
                        }
                    }
                    if (!nextRoots.Any()) //Ran out of cells to claim, just take what we got
                    { return result; }
                }
                currentRoots = nextRoots;
            }

            return result;
        }

        [Obsolete]
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

        #endregion Room Projection Strategies

        #region Helper Methods

        private static bool ClaimRoom(List<Cell> cells, ref Room room)
        {
            /*Rooms can only exist within one region*/
            if (cells.Select(s => s.regionId).Distinct().Count() > 1) { return false; }

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

            foreach (var cell in cells)
            {
                cell.roomId = room.id;
                CellCollection.Update(cell);
            }

            return true;
        }

        #endregion Helper Methods

        #endregion Room Claiming

        #region Door Parsing

        public static bool AllowCrossRegionConnections = false;

        public static void ParseDoors(Region region)
        {
            //Pathway door pass - Connect each important pathway room when they meetup

            #region Pathway Pass

            foreach (var importantRoom in region.GetRooms().Where(x => x.containsPath).ToArray())
            {
                importantRoom.pathConfirmedOverride = true;
                var importantNeighbors = importantRoom.neighborRooms.Where(x => x.containsPath);
                foreach (var neighbor in importantNeighbors)
                {
                    if (neighbor.connectedRooms.Count > 0 && neighbor.GetCells().Any(x => x.type == CellType.Elevation)) continue;
                    if (neighbor.connectedRooms.Any(x => x == importantRoom)) continue;

                    var potentialDoors = importantRoom.GetCells().Where(x => x.NeighborCellsOutOfRoom(true).Any(a => a.roomId == neighbor.id)).ToList();

                    if (importantRoom.connectedRooms.Count > 1 && potentialDoors.Any(x => x.type == CellType.Elevation)
                        || potentialDoors.Any(x => x.NeighborCellsOutOfRoom(true).Any(y => y.type == CellType.Elevation && y.roomId == neighbor.id && neighbor.connectedRooms.Count > 1)))
                    {
                        potentialDoors.Where(x => x.type == CellType.Elevation).ToList().ForEach(x => potentialDoors.Remove(x));
                    }

                    if (!potentialDoors.Any()) continue;

                    var selectDoor = potentialDoors[Random.Range(0, potentialDoors.Count)];
                    var door = CreateDoor(selectDoor, selectDoor.NeighborCellsOutOfRoom(true).First(x => x.roomId == neighbor.id));
                    neighbor.doors.Add(door);
                    importantRoom.doors.Add(door);
                    neighbor.pathConfirmedOverride = true;
                }
            }

            #endregion Pathway Pass

            //Other room pass

            #region Other Room Pass

            var roomsLeft = region.GetRooms().Where(x => x.connectedRooms.Count == 0).ToList();
            var retries = 10;
            while (roomsLeft.Any() && retries > 0)
            {
                var roomsNextToPathwayConfirmed = roomsLeft.Where(x => x.neighborRooms.Any(y => y.pathConfirmedOverride)).ToList();
                if (!roomsNextToPathwayConfirmed.Any()) { retries--; continue; };
                var rootRoom = roomsNextToPathwayConfirmed[Random.Range(0, roomsNextToPathwayConfirmed.Count)];

                var neighbors = rootRoom.neighborRooms.Where(x => x.pathConfirmedOverride).ToList();
                var targetNeighbor = neighbors[Random.Range(0, neighbors.Count)];

                var potentialDoors = rootRoom.GetCells().Where(x => x.NeighborCellsOutOfRoom().Any(a => a.roomId == targetNeighbor.id)
                && x.type != CellType.Elevation).ToList();

                if (!potentialDoors.Any()) { retries--; continue; };

                var targetDoor = potentialDoors[Random.Range(0, potentialDoors.Count)];
                var targetNeighborDoor = targetDoor.NeighborCellsOutOfRoom().First(x => x.roomId == targetNeighbor.id);

                var door = CreateDoor(targetDoor, targetNeighborDoor);
                rootRoom.doors.Add(door);
                targetNeighbor.doors.Add(door);
                rootRoom.pathConfirmedOverride = true;
                targetNeighbor.pathConfirmedOverride = true;

                roomsLeft = region.GetRooms().Where(x => x.connectedRooms.Count == 0).ToList();
            }

            #endregion Other Room Pass
        }

        private static Node_Door CreateDoor(Cell cell_1, Cell cell_2)
        {
            var room_1 = RoomCollection.rooms.First(x => x.Value.GetCells().Contains(cell_1));
            var room_2 = RoomCollection.rooms.First(x => x.Value.GetCells().Contains(cell_2));

            var door = new Node_Door()
            {
                cell_1 = cell_1,
                cell_2 = cell_2
            };

            CellCollection.cells[cell_1.position].mustNotBeBlocked = true;
            CellCollection.cells[cell_2.position].mustNotBeBlocked = true;

            //room_1.doors.Add(door);
            //room_2.doors.Add(door);

            Level.doors.Add(door);

            return door;
        }

        #endregion Door Parsing

        #region Room Scaffolding

        public static void ScaffoldRoomNodes()
        {
            foreach (var room in RoomCollection.GetAll())
            {
                var scaffold = new Scaffold { roomId = room.id };

                Elevation_Parse(room, ref scaffold);

                Floor_ParseMain(room, ref scaffold);
                Floor_ParseConnectors(room, ref scaffold);
                Floor_ParseColumns(room, ref scaffold);

                Wall_ParseMain(room, ref scaffold);
                Wall_ParseConnector(room, ref scaffold);

                Ceiling_Parse(room, ref scaffold);

                Level.roomScaffolds.Add(room.id, scaffold);
            }
            CleanScaffolding();
        }

        private static void Elevation_Parse(Room room, ref Scaffold scaffold)
        {
            foreach (var cell in room.GetCells().Where(x => x.type == CellType.Elevation))
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
                        node.rootCells.Add(upper);
                        node.rootCells.Add(cell);
                        cell.elevationOverride_Lower = true;
                        upper.elevationOverride_Upper = true;
                        scaffold.elevation.Add(node);
                    }
                }
            }
        }

        #region Floor

        //Floor Parse
        private static void Floor_ParseMain(Room room, ref Scaffold scaffold)
        {
            foreach (var cell in room.GetCells())
            {
                var node = new Node_FloorMain();
                node.position = cell.position;
                node.root = cell;          //This redundancy is for simplifing the scaffold rendering process
                node.rootCells.Add(cell);  // <--
                scaffold.floor.main.Add(node);
            }
        }

        //Connector Parse
        private static void Floor_ParseConnectors(Room room, ref Scaffold scaffold)
        {
            foreach (var cell in room.GetCells())
            {
                var neighborsInRoom = cell.NeighborCellsInRoom();
                foreach (var neighbor in neighborsInRoom)
                {
                    if (!scaffold.floor.connectors.Any(x => x.position == cell.PositionBetween(neighbor)))
                    {
                        var node = new Node_FloorConnector();
                        node.position = cell.PositionBetween(neighbor);
                        node.rootCells.Add(cell);
                        node.rootCells.Add(neighbor);
                        node.normal = Directionf.GetNormalTowards(node.position, node.rootCells.First().position);
                        node.rootCells = new List<Cell>() { cell, neighbor };
                        scaffold.floor.connectors.Add(node);
                    }
                }
            }
        }

        //Column Parse
        private static void Floor_ParseColumns(Room room, ref Scaffold scaffold)
        {
            var cells = room.GetCells();
            if (cells.Count < 4) return; //Columns cannot show up in rooms with less than 4 cells

            foreach (var cell in cells)
            {
                foreach (var direction in Directionf.Directions())
                {
                    var cellGrouping = new List<Cell>();

                    if (ColumnScan(cell, ref cellGrouping, direction))
                    {
                        var node = new Node_FloorColumn();
                        node.position = Cellf.PositionBetween(cellGrouping);
                        node.rootCells = cellGrouping;
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
            for (int i = 0; i < 4; i++)
            {
                if (CellCollection.HasCellAt(currentCell.Step(currentDirection)))
                {
                    var nextCell = CellCollection.cells[currentCell.Step(currentDirection)];
                    if (!nextCell.HasSameRoom(root)) break;
                    currentDirection = currentDirection.Right();
                    result.Add(currentCell);
                    currentCell = nextCell;
                }
                else
                { break; }
            }

            if (result.Count == 4)
            {
                cellGrouping = result;
                return true;
            }

            return false;
        }

        #endregion Floor

        #region Wall

        private static void Wall_ParseMain(Room room, ref Scaffold scaffold)
        {
            foreach (var cell in room.GetCells())
            {
                foreach (var direction in Directionf.Directions())
                {
                    if (CellCollection.HasCellAt(cell.Step(direction)))
                    {
                        var neighbor = CellCollection.cells[cell.Step(direction)];
                        if (neighbor.roomId != cell.roomId)
                        {
                            RenderWallNode(cell, direction, ref scaffold);
                        }
                    }
                    else
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
            node.rootCells.Add(cell);
            node.root = cell;
            node.direction = direction;
            scaffold.wall.main.Add(node);
        }

        private static void Wall_ParseConnector(Room room, ref Scaffold scaffold)
        {
            foreach (var connector in scaffold.floor.connectors)
            {
                var rootNormal = connector.rootCells.First().DirectionToNeighbor(connector.rootCells.Last());
                var directionsToTry = Directionf.Directions();
                directionsToTry.Remove(rootNormal);
                directionsToTry.Remove(rootNormal.Opposite());

                foreach (var direction in directionsToTry)
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

            if (knownDoorsForRoots.Any() || knownWallsForRoots.Any())
            {
                var connectorNode = new Node_WallConnector();
                connectorNode.position = node.position + (direction.ToVector() * CELL_PARTIAL_OFFSET);
                connectorNode.root = node;
                connectorNode.rootCells.AddRange(node.rootCells);
                connectorNode.offsetRoot = node.position;
                connectorNode.direction = direction;
                scaffold.wall.connectors.Add(connectorNode);
            }
        }

        #endregion Wall

        #region Ceiling

        //Ceiling Parse
        private static void Ceiling_Parse(Room room, ref Scaffold scaffold)
        {
            foreach (var main in scaffold.floor.main)
            {
                var node = new Node_CeilingMain();
                node.position = main.position + (Direction.Up.ToVector());
                node.root = main;
                node.rootCells.AddRange(main.rootCells);
                scaffold.ceiling.main.Add(node);
            }

            foreach (var connector in scaffold.floor.connectors)
            {
                var node = new Node_CeilingConnector();
                node.position = connector.position + (Direction.Up.ToVector());
                node.root = connector;
                node.normal = connector.normal;
                node.rootCells.AddRange(connector.rootCells);
                scaffold.ceiling.connectors.Add(node);
            }

            foreach (var column in scaffold.floor.columns)
            {
                var node = new Node_CeilingColumn();
                node.position = column.position + (Direction.Up.ToVector());
                node.root = column;
                node.rootCells.AddRange(column.rootCells);
                scaffold.ceiling.columns.Add(node);
            }
        }

        #endregion Ceiling

        private static void CleanScaffolding()
        {
        }

        #endregion Room Scaffolding

        #region Room Parsing

        public static void ParseRoom(this Room room)
        {
            var result = new Parsing_Node();

            var roomDoors = Level.doors.Where(x => x.Contains(room)).ToList();

            //Room Type
            result.roomType = room.GetRoomType(roomDoors);

            //Room Cell Evaluation
            result.pointsOfInterest = room.EvaluatePointsOfInterest();

            Level.roomParsings.Add(room.id, result);
        }

        private static RoomType GetRoomType(this Room room, List<Node_Door> doors)
        {
            var cells = room.GetCells();

            if (doors.Count() == 1)
            {
                if (cells.Count == 1)
                {
                    return RoomType.EndRoom;
                }
                else if (doors.Count <= 3)
                {
                    return RoomType.SideRoom;
                }
                else
                {
                    return RoomType.Arena;
                }
            }
            else if (doors.Count() == 2)
            {
                if (cells.Count() == 1)
                {
                    return RoomType.Connector;
                }
                else if (cells.Count() == 2)
                {
                    //All doors are on one cell
                    if (cells.Any(x => doors.All(y => y.Contains(x))))
                    {
                        return RoomType.SideRoom;
                    }
                    else
                    {
                        return RoomType.Connector;
                    }
                }
                else if (cells.Count() <= 3)
                {
                    if (cells.Any(x => doors.All(y => y.Contains(x))))
                    {
                        return RoomType.SideRoom;
                    }
                    else
                    {
                        RoomType temp = RoomType.Unknown;

                        foreach (var cell in cells)
                        {
                            var neighbors = cell.NeighborCellsInRoom();
                            var door_1 = doors.FirstOrDefault(x => x.Contains(cell));
                            var door_2 = doors.FirstOrDefault(x => neighbors.Any(y => x.Contains(y)));

                            if (door_1 == null || door_2 == null)
                            {
                                continue;
                            }
                            else if (door_1 != null && door_2 != null)
                            {
                                temp = RoomType.SideRoom;
                            }

                            if (temp != RoomType.Unknown) return temp;
                        }

                        return RoomType.Connector;
                    }
                }
                else if (cells.Count() <= 8)
                {
                    return RoomType.LargeRoom;
                }
                else
                {
                    return RoomType.Arena;
                }
            }
            else if (doors.Count() > 2)
            {
                if (cells.Count() <= 5)
                {
                    return RoomType.Connector;
                }
                else
                {
                    return RoomType.Courtyard;
                }
            }

            return RoomType.Unknown;
        }

        private static List<PointOfInterest> EvaluatePointsOfInterest(this Room room)
        {
            var result = new List<PointOfInterest>();

            var scaffold = Level.roomScaffolds[room.id];

            foreach (var cell in room.GetCells())
            {
                result.Add(cell.EvaluateCell(scaffold));
            }

            //result.AddRange(room.EvaluateCompoundPointsOfInterest(scaffold, result));

            return result;
        }

        //Determine lowest level point of interest for cell
        private static PointOfInterest EvaluateCell(this Cell cell, Scaffold roomScaffold)
        {
            var result = new PointOfInterest();

            result.cells.Add(cell.position);

            var walls = roomScaffold.wall.main.Where(x => x.root.position == cell.position).ToList();
            switch (walls.Count())
            {
                case 3:
                    result.interestType = PointOfInterestType.Endroom;
                    break;

                case 2:
                    result.interestType = cell.CellWallsAreAdjacent(walls) ? PointOfInterestType.Corner : PointOfInterestType.Hall;
                    break;

                case 1:
                    result.interestType = PointOfInterestType.Side;
                    break;

                default:
                    result.interestType = PointOfInterestType.Open_Area;
                    break;
            }

            result.hasDoors = cell.doors.Count() > 0;

            return result;
        }

        private static bool CellWallsAreAdjacent(this Cell cell, List<Node_WallMain> wallsForCell)
        {
            var directions = wallsForCell.Select(s => s.direction).ToList();
            foreach (var direction in directions)
            {
                if (directions.Any(x => x.Left() == direction || x.Right() == direction))
                {
                    return true;
                }
            }

            return false;
        }

        private static List<PointOfInterest> EvaluateCompoundPointsOfInterest(this Room room, Scaffold scaffold, List<PointOfInterest> simplePOI)
        {
            var result = new List<PointOfInterest>();

            return result;
        }

        #region Compound Evaluations

        #region Hallways

        //Hallway
        private static List<PointOfInterest> EvaluateCompound_Hallway(this Room room, List<PointOfInterest> simplePOI)
        {
            var result = new List<PointOfInterest>();
            var halls = simplePOI.Where(x => x.interestType == PointOfInterestType.Hall).ToList();

            return result;
        }

        //Long_Hallway
        //T_Hallway
        //Cross_Hallway

        #endregion Hallways

        #region Side Partitions

        //Side_Partition
        //Side_Partition_Long
        //Side_Partition_Elbow
        //Side_Partition_Elbow_Enclosed

        #endregion Side Partitions

        #region Center Pieces

        //Small_Center_Piece
        //Medium_Center_Piece
        //Large_Center_Piece

        #endregion Center Pieces

        #endregion Compound Evaluations

        //Using this as a placeholder to enforce the improved cell referencing,
        //uses the dirty cell parameter to fetch the consistent cell reference.
        private static Cell GetCell(this Cell cell)
        {
            return CellCollection.cells[cell.position];
        }

        #endregion Room Parsing
    }
}