using Assets.Scripts.Levels.Generation.Base;
using Assets.Scripts.Levels.Generation.Base.Mono;
using Assets.Scripts.Levels.Generation.Base.Mono.Debug;
using Assets.Scripts.Levels.Generation.CellBuilder;
using Assets.Scripts.Levels.Generation.RoomBuilder;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation
{
    public class LevelGeneratorBase : MonoBehaviour
    {
        private List<Region> regions = new List<Region>();

        #region Debug
        public CellDebug cellDebug;

        public RoomDebug roomDebug;

        public NodeDebug nodeDebug;

        public bool debugShowCells = false;
        public bool debugShowRoomBase = false;
        public bool debugShowRoomScaffolds = false;
        public bool debugShowDoors = false;
        #endregion

        //TODO: Refactor this pipeline, allow regions to control their own parameters
        protected virtual void Start()
        {
            //Step 1: Initialize metadata
            Init();

            //Step 2: Build pathways and accompanying cells
            HandleCellGeneration();

            //Step 3: Parse rooms and doorways
            HandleRoomParsing();

            //Step 4: Apply templates

            //Step 5: Decorate / Associate

            //Step 6: Render debug
            HandleDebug();

            return;
        }

        #region Rendering Steps

        protected void Init()
        {
            regions = transform.GetComponentsInChildren<Region>().ToList();
        }

        protected void HandleCellGeneration()
        {
            //Build Path
            regions.ForEach(x => PathBuilder.BuildPath(ref x));

            //Expand / Decay Cells
            regions.ForEach(x => { PathExpander.Expand(ref x); PathExpander.DecayCells(ref x); });

            PathExpander.CleanIsolatedCells();
        }

        protected void HandleRoomParsing()
        {
            //Claim Rooms
            foreach (var region in regions)
            {
                RoomClaimer.ClaimRooms(region.name, RoomClaimer.RoomClaimStrategy.Random, RoomClaimer.MAXIMUM_CLAIM_AMOUNT);
            }

            RoomParser.ParseDoors();
            RoomParser.ParseRoomNodes();
        }

        #endregion

        private void HandleDebug()
        {
            if (debugShowCells) {
                foreach (var cell in CellCollection.cells.Values)
                {
                    cellDebug.RenderCellDebug(cell.position, cell.type);
                }
            }
            if (debugShowRoomBase)
            {
                foreach (var room in RoomCollection.rooms.OrderBy(o => o.cells.First().region))
                {
                    roomDebug.RenderRoomDebug(room);
                }
            }
            if (debugShowDoors) nodeDebug.RenderDoorNodes();
            if (debugShowRoomScaffolds)
            {
                RoomCollection.rooms.ForEach(x => roomDebug.RenderRoomScaffoldingDebug(x));
                roomDebug.RenderRoomScaffoldingDoorDebug();
            }
        }
    }
}
