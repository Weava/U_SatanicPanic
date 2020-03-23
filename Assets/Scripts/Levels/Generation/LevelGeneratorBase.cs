using Assets.Scripts.Levels.Generation.Base;
using Assets.Scripts.Levels.Generation.Base.Mono;
using Assets.Scripts.Levels.Generation.Base.Mono.Debug;
using Assets.Scripts.Levels.Generation.CellBuilder;
using Assets.Scripts.Levels.Generation.Extensions;
using Assets.Scripts.Levels.Generation.RoomBuilder;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation
{
    public class LevelGeneratorBase : MonoBehaviour
    {
        public int expansionAmount;
        public float decayAmount;

        public List<PathMarker> pathMarkers;

        public CellDebug cellDebug;

        public RoomDebug roomDebug;

        public NodeDebug nodeDebug;

        public bool debugShowCells = false;
        public bool debugShowRoomBase = false;
        public bool debugShowRoomScaffolds = false;
        public bool debugShowDoors = false;

        //TODO: Refactor this pipeline, allow regions to control their own parameters
        protected virtual void Start()
        {
            if(pathMarkers.Where(x => x.hasSpawn).Count() > 1)
            { throw new System.Exception("Only one region can have a spawn at a time"); }

            foreach(var regionAnchor in pathMarkers)
            {
                if(regionAnchor.startPosition.x % Cellf.CELL_STEP_OFFSET != 0
                    || regionAnchor.startPosition.y % (Cellf.CELL_STEP_OFFSET/2.0f) != 0
                    || regionAnchor.startPosition.z % Cellf.CELL_STEP_OFFSET != 0)
                {
                    throw new System.Exception("Region markers are not divisible by the cell step offset.");
                }
            }

            var regions = pathMarkers.OrderByDescending(o => o.hasSpawn).ToArray();

            //Build Path
            foreach (var region in regions)
            {
                PathBuilder.BuildPath(region.startPosition, region.endPosition, region.name, region.hasSpawn);
            }

            //Expand / Decay Cells
            foreach(var region in regions)
            {
                CellCollection.GetByRegion(region.name).ForEach(x => PathExpander.ExpandCellPoint(x, expansionAmount));
                PathExpander.DecayCells(CellCollection.GetByRegion(region.name), decayAmount);
            }
            PathExpander.CleanIsolatedCells();

            //Claim Rooms
            foreach (var region in regions)
            {
                RoomClaimer.ClaimRooms(region.name, RoomClaimer.RoomClaimStrategy.Random, RoomClaimer.MAXIMUM_CLAIM_AMOUNT);
            }

            RoomParser.ParseDoors();
            RoomParser.ParseRoomNodes();

            if(debugShowCells) RenderCells();
            if (debugShowRoomBase) RenderRoomScaffolds();
            if (debugShowDoors) nodeDebug.RenderDoorNodes();
            if (debugShowRoomScaffolds) {
                RoomCollection.rooms.ForEach(x => roomDebug.RenderRoomScaffoldingDebug(x));
                roomDebug.RenderRoomScaffoldingDoorDebug();
            }
        }

        public void RenderCells()
        {
            foreach(var cell in CellCollection.cells.Values)
            {
                cellDebug.RenderCellDebug(cell.position, cell.type);
            }
        }

        public void RenderRoomScaffolds()
        {
            foreach(var room in RoomCollection.rooms.OrderBy(o => o.cells.First().region))
            {
                roomDebug.RenderRoomDebug(room);
            }
        }
    }
}
