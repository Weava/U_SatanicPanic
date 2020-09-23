using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Levels.Generation.Base;
using Assets.Scripts.Levels.Generation.Extensions;
using Assets.Scripts.Levels.Generation.RoomBuilder;
using Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding;
using Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding.Base;
using Assets.Scripts.Misc.Extensions;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation.Rendering.Suites.Base
{
    /// <summary>
    /// Base class for rendering a room's geometry
    /// </summary>
    public class Suite : MonoBehaviour
    {
        public string suiteName;

        public EntityPool_Fill FillEntityPool;

        public string overrideGlobalTag;

        [HideInInspector] public bool isGlobalSuite;

        #region Rendering Rules

        /// <summary>
        /// At least one instance of this suite must be rendered in the scene
        /// </summary>
        public bool required;

        /// <summary>
        /// Number of times this suite can repeat
        /// </summary>
        public int renderAmountCap = 1;

        /// <summary>
        /// Override to how many times a suite can render to an infinite amount of times
        /// </summary>
        public bool noLimitToRenderCap = false;

        #region Rendering Bias - Nudges and Weights to were a suite would prefer to render

        //public SuiteLocationBias locationBias;

        #endregion

        #endregion

        #region Rendering Properties

        [HideInInspector] public SuiteRenderingContainer renderContainer;
        [HideInInspector] public SuiteRenderingContainer nextContainerInstance;

        #endregion

        #region Virtual Rendering Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public virtual bool Build(LevelRoom room)
        {
            ResetRenderContainer(room);
            BuildFill(room);
            return true;
        }

        /// <summary>
        /// Checks if a target room meets the render requirements for this suite before attempting a render.
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public virtual bool ValidateRoom(LevelRoom room)
        {
            return true;
        }

        /// <summary>
        /// Determines the match rate for this suite's bias against a target room. The lower, the better.
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public virtual int GetBiasPreference(LevelRoom room)
        {
            return 0;
        }

        #endregion

        #region Suite Default Rendering Methods

        public void ResetRenderContainer(LevelRoom room)
        {
            renderContainer = new SuiteRenderingContainer();

            var roomCells = CellCollection.GetByRoom(room.roomId);
            var doorways = Level.doors.Where(x => roomCells.Any(c => x.rootCells.Contains(c)));
            var openCells = roomCells.Where(x => doorways.Any(a => a.rootCells.Contains(x)));

            renderContainer.spaces.AddRange(openCells.Select(s => (Vector4)s.position));

            nextContainerInstance = renderContainer.Copy();
        }

        public void RollbackRenderContainer()
        {
            nextContainerInstance = renderContainer.Copy();
        }

        public bool RenderEntity(LevelRoom room, SuiteEntity entity)
        {
            if (renderContainer == null) { ResetRenderContainer(room); }

            var projectionScaffolds = new List<Scaffold_Node>();

            var success = false;

            //Shortcut to prevent expensive calculations if something obvious can prevent more computation
            if (!ValidateRoom(room)) return false;

            foreach (var cell in CellCollection.GetByRoom(room.roomId).ToList().Shuffle())
            {
                foreach (var direction in Directionf.Directions())
                {
                    var projection = entity.BuildProjection(cell.position, direction);
                    //Projection cannot clip room cell space
                    if (projection.spaces.Any(x => !CellCollection.HasCellAt(x) || CellCollection.cells[x].roomId != room.roomId))
                    { continue; }

                    //Projection cannot intersect existing projections
                    if (nextContainerInstance.cellPositionsTaken.Any(a => projection.spaces.Contains(a)))
                    { continue; }

                    //Project cannot intersect with existing scaffold claims
                    if (!VerifyProjectionScaffoldDoesntIntersect(projection, entity, room, cell.position, direction, out projectionScaffolds))
                    { continue;}

                    //Projection cannot block navigation through room
                    if (!VerifyProjectionDoesNotBlockRoomPathways(projection))
                    { continue; }

                    //If we made it this far, this projection is valid, we will claim it
                    ClaimProjection(projection, projectionScaffolds, cell, direction, entity);
                    success = true;
                    break;
                }

                if (success)
                {
                    renderContainer = nextContainerInstance;
                    renderContainer.claimedScaffolds.ForEach(x => Level.roomScaffolds[room.roomId].SetNodeClaimed(x.id));
                    RollbackRenderContainer();
                    break; //A claim was found
                }
            }

            return success;
        }

        public bool BuildFill(LevelRoom room)
        {
            var roomScaffolds_Flat = Level.roomScaffolds[room.roomId].GetFlattenedNodes();
            var remainingScaffolds = roomScaffolds_Flat
                .Where(x => !renderContainer.claimedScaffolds.Select(s => s.id).Contains(x.id));

            foreach (var scaffold in remainingScaffolds)
            {
                if (scaffold.type == ScaffoldNodeType.Floor_Main)
                {
                    renderContainer.fillEntities.Add(new Tuple<ScaffoldNodeType, List<Vector3>, Direction>
                            (ScaffoldNodeType.Floor_Main, scaffold.rootCells.Select(s => s.position).ToList(), Direction.Up)
                            , FillEntityPool.floor_main);
                } else if (scaffold.type == ScaffoldNodeType.Floor_Connector)
                {
                    renderContainer.fillEntities.Add(new Tuple<ScaffoldNodeType, List<Vector3>, Direction>
                            (ScaffoldNodeType.Floor_Connector, scaffold.rootCells.Select(s => s.position).ToList(), (scaffold as Node_FloorConnector).normal)
                        , FillEntityPool.floor_connector);
                }
                else if (scaffold.type == ScaffoldNodeType.Floor_Column)
                {
                    renderContainer.fillEntities.Add(new Tuple<ScaffoldNodeType, List<Vector3>, Direction>
                            (ScaffoldNodeType.Floor_Column, scaffold.rootCells.Select(s => s.position).ToList(), Direction.Up)
                        , FillEntityPool.floor_column);
                }
                else if (scaffold.type == ScaffoldNodeType.Wall_Main)
                {
                    renderContainer.fillEntities.Add(new Tuple<ScaffoldNodeType, List<Vector3>, Direction>
                            (ScaffoldNodeType.Wall_Main, scaffold.rootCells.Select(s => s.position).ToList(), (scaffold as Node_WallMain).direction)
                        , FillEntityPool.wall_main);
                }
                else if (scaffold.type == ScaffoldNodeType.Wall_Connector)
                {
                    renderContainer.fillEntities.Add(new Tuple<ScaffoldNodeType, List<Vector3>, Direction>
                            (ScaffoldNodeType.Wall_Connector, scaffold.rootCells.Select(s => s.position).ToList(), (scaffold as Node_WallConnector).direction)
                        , FillEntityPool.wall_connector);
                }
                else if (scaffold.type == ScaffoldNodeType.Ceiling_Main)
                {
                    renderContainer.fillEntities.Add(new Tuple<ScaffoldNodeType, List<Vector3>, Direction>
                            (ScaffoldNodeType.Ceiling_Main, scaffold.rootCells.Select(s => s.position).ToList(), Direction.Up)
                        , FillEntityPool.ceiling_main);
                }
                else if (scaffold.type == ScaffoldNodeType.Ceiling_Connector)
                {
                    renderContainer.fillEntities.Add(new Tuple<ScaffoldNodeType, List<Vector3>, Direction>
                            (ScaffoldNodeType.Ceiling_Connector, scaffold.rootCells.Select(s => s.position).ToList(), (scaffold as Node_CeilingConnector).normal)
                        , FillEntityPool.ceiling_connector);
                }
                else if (scaffold.type == ScaffoldNodeType.Ceiling_Column)
                {
                    renderContainer.fillEntities.Add(new Tuple<ScaffoldNodeType, List<Vector3>, Direction>
                            (ScaffoldNodeType.Ceiling_Column, scaffold.rootCells.Select(s => s.position).ToList(), Direction.Up)
                        , FillEntityPool.ceiling_column);
                }

                Level.roomScaffolds[room.roomId].SetNodeClaimed(scaffold.id);
            }

            return true;
        }

        public void Render(ref LevelRoom room)
        {
            var suiteGameobject = new GameObject("Suite");
            suiteGameobject.transform.parent = room.renderContainer.transform;

            //Main Entity Pass
            foreach (var entity in renderContainer.entities)
            {
                var position = entity.Key.Item1;
                var direction = entity.Key.Item2;

                var instance = Instantiate(entity.Value, position, new Quaternion());
                instance.transform.Rotate(new Vector3(0,1,0), (int) direction.ToAngle());
                instance.transform.parent = suiteGameobject.transform;
            }

            //Fill Entity Pass
            foreach (var entity in renderContainer.fillEntities)
            {
                SuiteEntity_Scaffold instance;
                switch (entity.Key.Item1) //Node Type
                {
                    case ScaffoldNodeType.Floor_Main:
                    case ScaffoldNodeType.Floor_Column:
                        instance = Instantiate(entity.Value, entity.Key.Item2.PositionBetween(), new Quaternion());
                        instance.transform.parent = suiteGameobject.transform;
                        break;
                    case ScaffoldNodeType.Floor_Connector:
                        instance = Instantiate(entity.Value, entity.Key.Item2.PositionBetween(), new Quaternion());
                        instance.transform.Rotate(new Vector3(0, 1, 0), entity.Key.Item3.Opposite().ToAngle());
                        instance.transform.parent = suiteGameobject.transform;
                        break;
                    case ScaffoldNodeType.Wall_Main:
                    case ScaffoldNodeType.Wall_Connector:
                        instance = Instantiate(entity.Value, OffsetWall(entity.Key.Item2.PositionBetween(), entity.Key.Item3), new Quaternion());
                        instance.transform.Rotate(new Vector3(0,1,0), entity.Key.Item3.Opposite().ToAngle());
                        instance.transform.parent = suiteGameobject.transform;
                        break;
                    case ScaffoldNodeType.Ceiling_Main:
                    case ScaffoldNodeType.Ceiling_Column:
                        instance = Instantiate(entity.Value, entity.Key.Item2.PositionBetween(), new Quaternion());
                        instance.transform.parent = suiteGameobject.transform;
                        break;
                    case ScaffoldNodeType.Ceiling_Connector:
                        instance = Instantiate(entity.Value, entity.Key.Item2.PositionBetween(), new Quaternion());
                        instance.transform.Rotate(new Vector3(0, 1, 0), entity.Key.Item3.Opposite().ToAngle());
                        instance.transform.parent = suiteGameobject.transform;
                        break;
                    default:
                        break;
                }
            }
        }

        #region Rendering Helpers

        public void ClaimProjection(SuiteProjection projection, List<Scaffold_Node> projectionScaffold, Cell root, Direction direction, SuiteEntity entity)
        {
            nextContainerInstance.spaces.AddRange(projection.spaces);
            nextContainerInstance.claimedScaffolds.AddRange(projectionScaffold);
            nextContainerInstance.entities.Add(new Tuple<Vector3, Direction>(root.position, direction), entity);
        }

        public bool VerifyProjectionDoesNotBlockRoomPathways(SuiteProjection projection)
        {
            return Verify_Step(nextContainerInstance.spaces.FirstOrDefault(x => x.w == 0), new List<Vector3>());
        }

        private bool Verify_Step(Vector3 root, List<Vector3> searchedSpaces)
        {
            searchedSpaces.Add(root);

            ////Found all the open spaces, we're good.
            if (nextContainerInstance.spaces.Where(x => x.w == 0).All(x => searchedSpaces.Contains(x))) return true;

            var nextPositionsToCheck = CellCollection.cells[root].NeighborCellsInRoom().Select(s => s.position)
                .Where(x => !searchedSpaces.Contains(x) && !IsBlockedPath(root, x)/*!nextContainerInstance.blockedSpaces.Contains(x)*/);

            foreach (var nextPosition in nextPositionsToCheck)
            {
                var result = Verify_Step(nextPosition, searchedSpaces);

                if (result) return true;
            }

            return false;
        }

        public bool VerifyProjectionScaffoldDoesntIntersect(SuiteProjection projection, SuiteEntity entity, LevelRoom room, Vector3 root, Direction normal, out List<Scaffold_Node> projectionNodes)
        {
            if (entity.partial)
            {
                return VerifyScaffoldsForPartialSpace(projection, entity, room, out projectionNodes);
            }

            return VerifyScaffoldsForFullSpace(projection, entity, room, root, normal, out projectionNodes);
        }

        private bool VerifyScaffoldsForFullSpace(SuiteProjection projection, SuiteEntity entity, LevelRoom room, Vector3 root, Direction normal, out List<Scaffold_Node> projectionNodes)
        {
            projectionNodes = new List<Scaffold_Node>();
            var roomScaffold = Level.roomScaffolds[room.roomId];

            var projectionSpace = projection.spaces;

            var projectionCells = CellCollection.cells.Select(s => s.Value).Where(x => projectionSpace.Contains(x.position));

            //Floor + Ceiling, both are handled in the same move when not a partial
            var floor_main = roomScaffold.floor.main.Where(x => x.rootCells.All(a => projectionCells.Contains(a)));
            if (nextContainerInstance.claimedScaffolds.Any(a => floor_main.Select(s => s.id).Contains(a.id))) { return false;}
            projectionNodes.AddRange(floor_main);
            var floor_connectors = roomScaffold.floor.connectors.Where(x => x.rootCells.All(a => projectionCells.Contains(a)));
            if (nextContainerInstance.claimedScaffolds.Any(a => floor_connectors.Select(s => s.id).Contains(a.id))) { return false; }
            projectionNodes.AddRange(floor_connectors);
            var floor_columns = roomScaffold.floor.columns.Where(x => x.rootCells.All(a => projectionCells.Contains(a)));
            if (nextContainerInstance.claimedScaffolds.Any(a => floor_columns.Select(s => s.id).Contains(a.id))) { return false; }
            projectionNodes.AddRange(floor_columns);

            var ceiling_main = roomScaffold.ceiling.main.Where(x => x.rootCells.All(a => projectionCells.Contains(a)));
            if (nextContainerInstance.claimedScaffolds.Any(a => ceiling_main.Select(s => s.id).Contains(a.id))) { return false; }
            projectionNodes.AddRange(ceiling_main);
            var ceiling_connectors = roomScaffold.ceiling.connectors.Where(x => x.rootCells.All(a => projectionCells.Contains(a)));
            if (nextContainerInstance.claimedScaffolds.Any(a => ceiling_connectors.Select(s => s.id).Contains(a.id))) { return false; }
            projectionNodes.AddRange(ceiling_connectors);
            var ceiling_columns = roomScaffold.ceiling.columns.Where(x => x.rootCells.All(a => projectionCells.Contains(a)));
            if (nextContainerInstance.claimedScaffolds.Any(a => ceiling_columns.Select(s => s.id).Contains(a.id))) { return false; }
            projectionNodes.AddRange(ceiling_columns);

            //Walls
            var wall_main = new List<Node_WallMain>();
            var wall_connector = new List<Node_WallConnector>();

            var wallProjections = entity.BuildWallProjection(root, normal);

            foreach (var wall in wallProjections.spaces)
            {
                var direction = (Direction)wall.w;

                var result = roomScaffold.wall.main.FirstOrDefault(x => x.rootCells.All(a => a.position == (Vector3)wall) && x.direction == direction);
                if (result == null) return false;
                wall_main.Add(result);
                if (nextContainerInstance.claimedScaffolds.Any(a => wall_main.Select(s => s.id).Contains(a.id))) { return false; }
                projectionNodes.AddRange(wall_main);

                var result_C = roomScaffold.wall.connectors.FirstOrDefault(x =>
                    x.rootCells.All(a => a.position == (Vector3)wall) && x.direction == direction);
                if (result_C != null) wall_connector.Add(result_C);
                if (nextContainerInstance.claimedScaffolds.Any(a => wall_connector.Select(s => s.id).Contains(a.id))) { return false; }
                projectionNodes.AddRange(wall_connector);
            }

            //If we got to this point, all scaffold points are available
            return true;
        }

        private bool VerifyScaffoldsForPartialSpace(SuiteProjection projection, SuiteEntity entity, LevelRoom room, out List<Scaffold_Node> projectionNodes)
        {
            throw new NotImplementedException();
        }

        //private bool VerifyProjectionFitsRoomConfiguration(SuiteProjection projection, SuiteEntity entity, LevelRoom room, Direction direction)
        //{
        //    var wallScaffolds = Level.roomScaffolds[room.roomId].wall.main .Where(x => projection.spacesAsVec3.Contains(x.root.position));

        //    foreach (var wallScaffold in wallScaffolds)
        //    {
        //        var root = projection.spacesAsVec3.FirstOrDefault(x =>
        //            wallScaffolds.Select(s => s.root.position).Contains(x));
        //        if (root != null)
        //        {

        //        }
        //    }
        //}

        private bool IsBlockedPath(Vector3 root, Vector3 direction)
        {
            var root_Vec4Rep =
                nextContainerInstance.spaces.FirstOrDefault(x => x.x == root.x && x.y == root.y && x.z == root.z);
            var direction_Vec4Rep =
                nextContainerInstance.spaces.FirstOrDefault(x =>
                    x.x == direction.x && x.y == direction.y && x.z == direction.z);

            var normal = Directionf.GetNormalTowards(root, direction);

            return root_Vec4Rep != null && ((int)root.z).GetDirectionsFromByte().Contains(normal)
                   || direction_Vec4Rep != null && ((int)root.z).GetDirectionsFromByte().Contains(normal.Opposite());
        }

        private Vector3 OffsetWall(Vector3 root, Direction normal)
        {
            var offset = new Vector3(0,0,Cellf.CELL_MAIN_OFFSET/2.0f);
            offset = offset.ProjectOffsetToNormal(normal);
            return new Vector3(root.x + offset.x, root.y + offset.y, root.z + offset.z);
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// A container class for the rendered entities for a suite before they are actually committed.
    /// Acts as a temporary holding point if a suite is decided it can no longer validly render.
    /// </summary>
    public class SuiteRenderingContainer
    {
        public string roomId;

        public List<Vector4> cellPositionsTaken => spaces;

        public List<Scaffold_Node> claimedScaffolds = new List<Scaffold_Node>();

        public List<Vector4> spaces = new List<Vector4>();

        public Dictionary<Tuple<Vector3, Direction>, SuiteEntity> entities = new Dictionary<Tuple<Vector3, Direction>, SuiteEntity>();

        public Dictionary<Tuple<ScaffoldNodeType, List<Vector3>, Direction>, SuiteEntity_Scaffold> fillEntities
            = new Dictionary<Tuple<ScaffoldNodeType, List<Vector3>, Direction>, SuiteEntity_Scaffold>();

        public SuiteRenderingContainer Copy()
        {
            return (SuiteRenderingContainer)MemberwiseClone();
        }
    }

    public enum SuiteLocationBias
    {
        Anywhere,
        Towards_End,
        Towards_Beginning,
        Either_Ends,
        Middle
    }
}
