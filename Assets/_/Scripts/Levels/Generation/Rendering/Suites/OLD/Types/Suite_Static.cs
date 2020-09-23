//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using Assets.Scripts.Levels.Generation.Base;
//using Assets.Scripts.Levels.Generation.Extensions;
//using Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Parsing.Base;
//using Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding;
//using Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding.Base;
//using Assets.Scripts.Misc.Extensions;
//using UnityEngine;

//namespace Assets.Scripts.Levels.Generation.Rendering.Suites.Types
//{
//    public class Suite_Static : Suite
//    {
//        [Header("Entities")]
//        //Allows for variations to be contained in a single suite
//        public List<SuiteEntity> SuiteEntities = new List<SuiteEntity>();

//        #region Room Classification Mask

//        [Header("Classification")]
//        //Valid room classifications
//        public bool EndRoom;
//        public bool Connector;
//        public bool SideRoom;
//        public bool Arena;
//        public bool LargeRoom;
//        public bool Courtyard;
//        public bool Unknown;

//        public int minimumRoomSize = 1;
//        public int maximumRoomSize = 9;

//        #endregion

//        [HideInInspector] public Cell rootCell;
//        [HideInInspector] public Direction normal = Direction.North;

//        #region Defenition Properties

//        [Header("Space Usage")]
//        //Relative open cell space from the root target cell
//        public List<Vector3> openSpace = new List<Vector3>();

//        //Relative blocked cell space from the root target cell
//        public List<Vector3> blockedSpace = new List<Vector3>();

//        //Relative wall usage for a suite - X, Y, Z determine parent cell space, w is wall direction normal
//        //If a wall is not specified, the open/blocked space cells will not fill in the wall scaffold located there
//        public List<Vector4> wallSpace = new List<Vector4>();

//        public List<Vector4> doorSpace = new List<Vector4>();

//        #endregion

//        [Header("Debug")]
//        public bool renderDebug;

//        void Awake()
//        {
//            renderDebug = false;
//            type = SuiteType.Static;
//        }

//        public override bool IsValid()
//        {
//            //Room size is compatible?
//            var roomData = Level.roomData.FirstOrDefault(x => x.roomId == targetRoom.id);
//            //Room type is wrong
//            if (!RoomTypeIsIncluded(roomData.parsing.roomType)) return false;
//            var roomCellCount = targetRoom.GetCells().Count;

//            //Room cell count isn't in valid range
//            if (!(roomCellCount >= minimumRoomSize && roomCellCount <= maximumRoomSize)) return false;

//            var unClaimedCellCount = roomData.room.GetCells().Select(s => !s.claimedBySuite).Count();
//            var cellSpaceCount = openSpace.Count + blockedSpace.Count;
//            //Not enough cells to claim
//            if (unClaimedCellCount < cellSpaceCount) return false;

//            var success = false;
//            foreach (var direction in Directionf.Directions())
//            {
//                if (CellSpaceIsValid() && ScaffoldIsAvailable())
//                {
//                    return true;
//                } //Short cut will leave scaffold's direction normal to the last valid direction if found
//            }

//            return false; //Exhausted all directions, no valid matches found
//        }

//        public override void Render()
//        {
//            var cells = GetCellSpace();
//            var scaffoldUsage = GetScaffoldUsage();

//            var entity = SuiteEntities.Random();

//            FetchRoomRenderContainer();

//            //TODO: Note, if the instances look like they all are looking at world origin, the rotation instantiation could be fucked up here
//            var rotation = new Quaternion();
//            rotation.SetLookRotation(normal.ToVector(), Vector3.up);
//            var instance = Instantiate(entity, rootCell.position, rotation);
//            instance.transform.parent = roomInstanceContainer.transform;

//            //Set claims
//            foreach (var cellSpace in cells)
//            {
//                CellCollection.cells[cellSpace.cell.position].claimedBySuite = true;
//                CellCollection.cells[cellSpace.cell.position].mustNotBeBlocked = cellSpace.open;
//            }

//            var scaffold = Level.roomScaffolds[targetRoom.id];
//            foreach (var scaffoldNode in scaffoldUsage)
//            {
//                scaffold.SetNodeClaimed(scaffoldNode.id);
//            }

//            timesUsed++;
//        }

//        public override void Init()
//        {
//            type = SuiteType.Static;
//            id = Guid.NewGuid().ToString();
//        }

//        #region Helper Methods

//        #region Validators

//        protected bool ScaffoldIsAvailable()
//        {
//            var scaffoldUsage = GetScaffoldUsage();

//            foreach (var scaffoldNode in scaffoldUsage)
//            {
//                if (scaffoldNode.claimed) return false;
//            }

//            return true;
//        }

//        protected bool RoomTypeIsIncluded(RoomType roomType)
//        {
//            switch (roomType)
//            {
//                case RoomType.Arena:
//                    return Arena;
//                case RoomType.Connector:
//                    return Connector;
//                case RoomType.Courtyard:
//                    return Courtyard;
//                case RoomType.EndRoom:
//                    return EndRoom;
//                case RoomType.LargeRoom:
//                    return LargeRoom;
//                case RoomType.SideRoom:
//                    return SideRoom;
//                case RoomType.Unknown:
//                    return Unknown;
//                default:
//                    return false;
//            }
//        }

//        protected bool CellSpaceIsValid()
//        {
//            //Check for open space and blocked space interaction
//            var cells = GetCellSpace();
//            if (cells == null) return false;
//            var openCells = cells.Where(x => x.open).Select(s => s.cell).ToList();
//            var blockedCells = cells.Where(x => !x.open).Select(s => s.cell).ToList();

//            return targetRoom.VerifyCellCollectionDoesNotBlockRoomPathways(blockedCells, openCells);
//        }

//        #endregion

//        protected List<Scaffold_Node> GetScaffoldUsage()
//        {
//            var cells = GetCellSpace();
//            var walls = GetWallSpace();
//            var scaffold = Level.roomScaffolds[targetRoom.id];

//            var result = new List<Scaffold_Node>();

//            //Floor Nodes
//            result.AddRange(scaffold.floor.main
//                .Where(x => x.rootCells
//                    .All(a => cells.Select(s => s.cell.position).Contains(a.position))));

//            result.AddRange(scaffold.floor.connectors
//                .Where(x => x.rootCells
//                    .All(a => cells.Select(s => s.cell.position).Contains(a.position))));

//            result.AddRange(scaffold.floor.columns
//                .Where(x => x.rootCells
//                    .All(a => cells.Select(s => s.cell.position).Contains(a.position))));

//            //Wall Nodes
//            var wallMains = new List<Node_WallMain>();
//            foreach (var wallSpace in walls)
//            {
//                var wallNode = scaffold.wall.main
//                    .FirstOrDefault(x => x.direction == wallSpace.normal
//                                         && x.rootCells.All(a => a == wallSpace.cell /*Only one cell here anyways*/));
//                wallMains.Add(wallNode);
//            }

//            result.AddRange(wallMains);

//            result.AddRange(scaffold.wall.connectors
//                .Where(x => x.rootCells
//                    .All(a => wallMains.Select(s => s.position).Contains(a.position))));

//            //Ceiling Nodes
//            result.AddRange(scaffold.ceiling.main
//                .Where(x => x.rootCells
//                    .All(a => cells.Select(s => s.cell.position).Contains(a.position))));

//            result.AddRange(scaffold.ceiling.connectors
//                .Where(x => x.rootCells
//                    .All(a => cells.Select(s => s.cell.position).Contains(a.position))));

//            result.AddRange(scaffold.ceiling.columns
//                .Where(x => x.rootCells
//                    .All(a => cells.Select(s => s.cell.position).Contains(a.position))));

//            return result;
//        }

//        protected List<CellSpace> GetCellSpace()
//        {
//            var result = new List<CellSpace>();

//            foreach (var space in openSpace)
//            {
//                var projection = ProjectToCellSpace(rootCell.position, space);
//                if (CellCollection.HasCellAt(projection))
//                {
//                    var cell = CellCollection.cells[projection];
//                    result.Add(new CellSpace {cell = cell, open = true});
//                }
//                else
//                {
//                    return null;
//                }
//            }

//            foreach (var space in blockedSpace)
//            {
//                var projection = ProjectToCellSpace(rootCell.position, space);
//                if (CellCollection.HasCellAt(projection))
//                {
//                    var cell = CellCollection.cells[projection];
//                    result.Add(new CellSpace {cell = cell, open = false});
//                }
//                else
//                {
//                    return null;
//                }
//            }

//            return result;
//        }

//        protected List<WallSpace> GetWallSpace()
//        {
//            var result = new List<WallSpace>();

//            foreach (var space in wallSpace)
//            {
//                var cell = CellCollection.cells[ProjectToCellSpace(rootCell.position, space)];
//                result.Add(new WallSpace {cell = cell, isDoor = false, normal = ToDirection((int) space.w)});
//            }

//            foreach (var space in doorSpace)
//            {
//                var cell = CellCollection.cells[ProjectToCellSpace(rootCell.position, space)];
//                result.Add(new WallSpace {cell = cell, isDoor = true, normal = ToDirection((int) space.w)});
//            }

//            return result;
//        }

//        protected Direction ToDirection(int value)
//        {
//            switch (value)
//            {
//                case 4:
//                    return Direction.West;
//                case 3:
//                    return Direction.South;
//                case 2:
//                    return Direction.East;
//                default:
//                    return Direction.North;
//            }
//        }

//        #endregion

//        #region Draw Gizmos

//        void OnDrawGizmos()
//        {
//            RenderDebug();
//        }

//        private void RenderDebug()
//        {
//            if (renderDebug)
//            {
//                Gizmos.color = Color.magenta;
//                Gizmos.DrawCube(new Vector3(), new Vector3(1,1,1)); //Root

//                foreach (var space in openSpace)
//                {
//                    RenderCellSpace(space);
//                }

//                foreach (var space in blockedSpace)
//                {
//                    RenderCellSpace(space, true);
//                }

//                foreach (var space in wallSpace)
//                {
//                    RenderWallSpace(space);
//                }

//                foreach (var space in doorSpace)
//                {
//                    RenderWallSpace(space, true);
//                }
//            }
//        }

//        private void RenderCellSpace(Vector3 position, bool blocked = false)
//        {
//            if (blocked)
//            { Gizmos.color = Color.red; }
//            else
//            { Gizmos.color = Color.cyan; }

//            position = ProjectToCellSpace(position);

//            var floorOffset = Cellf.CELL_MAIN_OFFSET / 2.0f;
//            var heightOffset = Cellf.CELL_ELEVATION_OFFSET;

//            var wireFrame_floor = new List<Vector3>()
//            {
//                new Vector3(position.x + floorOffset, position.y, position.z + floorOffset),
//                new Vector3(position.x + floorOffset, position.y, position.z - floorOffset),
//                new Vector3(position.x - floorOffset, position.y, position.z - floorOffset),
//                new Vector3(position.x - floorOffset, position.y, position.z + floorOffset),
//            };

//            var wireFrame_elevated = new List<Vector3>()
//            {
//                new Vector3(position.x + floorOffset, position.y + heightOffset, position.z + floorOffset),
//                new Vector3(position.x + floorOffset, position.y + heightOffset, position.z - floorOffset),
//                new Vector3(position.x - floorOffset, position.y + heightOffset, position.z - floorOffset),
//                new Vector3(position.x - floorOffset, position.y + heightOffset, position.z + floorOffset),
//            };

//            var fullLineList = wireFrame_elevated;
//            fullLineList.AddRange(wireFrame_floor);

//            for (int i = 0; i < 3; i++)
//            {
//                Gizmos.DrawLine(wireFrame_floor[i], wireFrame_elevated[i]);
//                Gizmos.DrawLine(wireFrame_floor[i], wireFrame_floor[i + 1]);
//                Gizmos.DrawLine(wireFrame_elevated[i], wireFrame_elevated[i + 1]);
//            }

//            Gizmos.DrawLine(wireFrame_floor[3], wireFrame_floor[0]);
//            Gizmos.DrawLine(wireFrame_elevated[3], wireFrame_elevated[0]);
//            Gizmos.DrawLine(wireFrame_floor[3], wireFrame_elevated[3]);

//            Gizmos.color = Color.red;
//            Gizmos.DrawLine(wireFrame_floor[0], wireFrame_floor[2]);
//        }

//        private void RenderWallSpace(Vector4 root, bool door = false)
//        {
//            if (door)
//            { Gizmos.color = Color.green; }
//            else
//            { Gizmos.color = Color.yellow; }

//            var rootBase = ProjectToCellSpace(root);
//            root.x = rootBase.x;
//            root.y = rootBase.y;
//            root.z = rootBase.z;

//            var floorOffset = Cellf.CELL_MAIN_OFFSET / 2.0f;
//            var heightOffset = Cellf.CELL_ELEVATION_OFFSET;

//            List<Vector3> wireFrame;
//            switch (root.w)
//            {
//                case 1:
//                    wireFrame = new List<Vector3>()
//                    {
//                        new Vector3(root.x + floorOffset, root.y, root.z + floorOffset),
//                        new Vector3(root.x + floorOffset, root.y, root.z - floorOffset),
//                        new Vector3(root.x + floorOffset, root.y + heightOffset, root.z - floorOffset),
//                        new Vector3(root.x + floorOffset, root.y + heightOffset, root.z + floorOffset)
//                    };
//                    break;
//                case 2:
//                    wireFrame = new List<Vector3>()
//                    {
//                        new Vector3(root.x + floorOffset, root.y, root.z - floorOffset),
//                        new Vector3(root.x - floorOffset, root.y, root.z - floorOffset),
//                        new Vector3(root.x - floorOffset, root.y + heightOffset, root.z - floorOffset),
//                        new Vector3(root.x + floorOffset, root.y + heightOffset, root.z - floorOffset)
//                    };
//                    break;
//                case 3:
//                    wireFrame = new List<Vector3>()
//                    {
//                        new Vector3(root.x - floorOffset, root.y, root.z - floorOffset),
//                        new Vector3(root.x - floorOffset, root.y, root.z + floorOffset),
//                        new Vector3(root.x - floorOffset, root.y + heightOffset, root.z + floorOffset),
//                        new Vector3(root.x - floorOffset, root.y + heightOffset, root.z - floorOffset)
//                    };
//                    break;
//                default:
//                    wireFrame = new List<Vector3>()
//                    {
//                        new Vector3(root.x - floorOffset, root.y, root.z + floorOffset),
//                        new Vector3(root.x + floorOffset, root.y, root.z + floorOffset),
//                        new Vector3(root.x + floorOffset, root.y + heightOffset, root.z + floorOffset),
//                        new Vector3(root.x - floorOffset, root.y + heightOffset, root.z + floorOffset)
//                    };
//                    break;
//            }

//            for (int i = 0; i < 3; i++)
//            {
//                Gizmos.DrawLine(wireFrame[i], wireFrame[i + 1]);
//            }

//            Gizmos.DrawLine(wireFrame[3], wireFrame[0]);
//            Gizmos.DrawLine(wireFrame[0], wireFrame[2]);
//        }

//        private static Vector3 ProjectToCellSpace(Vector3 offset)
//        {
//            return new Vector3(
//                offset.x * Cellf.CELL_STEP_OFFSET,
//                offset.y * Cellf.CELL_ELEVATION_OFFSET,
//                offset.z * Cellf.CELL_STEP_OFFSET
//                );
//        }

//        #endregion
//    }

//    public class CellSpace
//    {
//        public Cell cell;
//        public bool open;
//    }

//    public class WallSpace
//    {
//        public Cell cell;
//        public bool isDoor;
//        public Direction normal;
//    }
//}