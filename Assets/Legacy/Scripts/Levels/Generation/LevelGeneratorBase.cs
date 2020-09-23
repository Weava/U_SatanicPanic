using Assets.Scripts.Levels.Generation.Base;
using Assets.Scripts.Levels.Generation.Base.Mono;
using Assets.Scripts.Levels.Generation.Base.Mono.Debug;
using Assets.Scripts.Levels.Generation.CellBuilder;
using Assets.Scripts.Levels.Generation.Rendering.Suites;
using Assets.Scripts.Levels.Generation.Rendering.Suites.Base;
using Assets.Scripts.Levels.Generation.RoomBuilder;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation
{
    public class LevelGeneratorBase : MonoBehaviour
    {
        [HideInInspector] public List<Region> regions = new List<Region>();

        [HideInInspector] public static List<GameObject> roomInstances = new List<GameObject>();

        public List<Suite> globalSuites = new List<Suite>();

        #region Debug

        public CellDebug cellDebug;

        public RoomDebug roomDebug;

        public NodeDebug nodeDebug;

        public bool debugShowCells = false;
        public bool debugShowRoomBase = false;
        public bool debugShowRoomScaffolds = false;
        public bool debugShowDoors = false;
        public bool debugShowPOI = false;

        #endregion Debug

        protected void Start()
        {
            StartCoroutine("GenerateLevel");
        }

        protected virtual void GenerateLevel()
        {
            //Step 1: Initialize metadata
            Init();

            //Step 2: Build pathways and accompanying cells
            HandleCellGeneration();

            //Step 3: Scaffold rooms and doorways
            HandleRoomScaffolding();

            //Step 4: Parse Rooms
            HandleRoomParsing();
            InstantiateRoomInstances();

            //Step 5: Decorate / Associate
            HandleSuiteRendering();

            //Step 6: Render debug
            HandleDebug();

            return;
        }

        #region Rendering Steps

        protected virtual void Init()
        {
            regions = transform.GetComponentsInChildren<Region>().ToList();
            regions.ForEach(x => RegionCollection.regions.Add(x.id, x));

            //foreach (var globalSuite in globalSuites)
            //{
            //    globalSuite.Init();

            //    if (!Level.suiteCollection.ContainsKey(globalSuite.id))
            //    {
            //        Level.suiteCollection.Add(globalSuite.id, globalSuite);
            //        globalSuite.regionsAllowed.AddRange(RegionCollection.regions.Select(s => s.Value).ToList());
            //    }
            //}
        }

        protected virtual void HandleCellGeneration()
        {
            //Build Path
            regions.ForEach(x => PathBuilder.BuildPath(ref x));

            var test = CellCollection.cells;

            //Expand / Decay Cells
            regions.ForEach(x =>
            {
                PathExpander.Expand(ref x);
                PathExpander.Proliferate(ref x);
                PathExpander.DecayCells(ref x);
            });

            regions.ForEach(x => PathExpander.CleanIsolatedCells(x));
        }

        protected virtual void HandleRoomScaffolding()
        {
            regions.ForEach(RoomParser.ClaimRooms);
            regions.ForEach(RoomParser.ParseDoors);
            RoomParser.ScaffoldRoomNodes();
        }

        protected virtual void HandleRoomParsing()
        {
            foreach (var room in RoomCollection.rooms.Select(s => s.Value).ToArray())
            {
                room.ParseRoom();
            }
        }

        protected virtual void InstantiateRoomInstances()
        {
            foreach (var room in Level.roomData.Select(s => s.room))
            {
                Level.Rooms.Add(room.id,
                    new LevelRoom
                    {
                        roomId = room.id,
                        regionId = room.regionId,
                        renderContainer = new GameObject()
                    });
            }
        }

        protected virtual void HandleSuiteRendering()
        {
            SuiteRenderHandler.GlobalSuites = globalSuites;
            SuiteRenderHandler.RenderLevelSuites();
        }

        //Feature Rendering

        //Decoration Rendering

        #endregion Rendering Steps

        private void HandleDebug()
        {
            if (debugShowCells)
            {
                foreach (var cell in CellCollection.cells.Values)
                {
                    cellDebug.RenderCellDebug(cell.position, cell.type);
                }
            }
            if (debugShowRoomBase)
            {
                foreach (var room in RoomCollection.GetAll())
                {
                    roomDebug.RenderRoomDebug(room);
                }
            }
            if (debugShowDoors) nodeDebug.RenderDoorNodes();
            if (debugShowRoomScaffolds)
            {
                RoomCollection.GetAll().ForEach(x => roomDebug.RenderRoomScaffoldingDebug(x));
                roomDebug.RenderRoomScaffoldingDoorDebug();
            }
            if (debugShowPOI)
            {
                nodeDebug.RenderPOI();
            }
        }
    }
}