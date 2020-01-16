using Assets.Scripts.Generation.Painter.Cells;
using Assets.Scripts.Generation.Painter.Rooms;
using Assets.Scripts.Generation.Painter.Cells.Base;
using Assets.Scripts.Generation.Painter.Cells.Factory;
using Assets.Scripts.Levels.Base;
using Assets.Scripts.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Assets.Scripts.Generation.Extensions;

namespace Assets.Scripts.Levels.Demo_0
{
    public class Demo_0_Generator : LevelGenerator
    {
        public int basementLength;
        private string basementTag = "Region_Basement";

        //public int floor1Length;

        //public int floor2Length;

        //public int outsideLength;

        //public int arenaLength;

        private Cell nextRootCell;

        #region Monobehaviour

        private void Start()
        {
            BuildLevel();
        }

        #endregion

        public override bool BuildLevel()
        {
            try
            {
                BuildBasementPath();
                BuildBasementCells();
                BuildBasementRooms();

                //Floor 1

                //Floor 2

                //Outside

                //Arena

                CleanUnclaimedCells();
                RenderMarkers();
                RenderRooms();
            } catch (Exception e)
            {
                throw new System.Exception("Level could not be generated.", e);
            }
            return true;
        }

        private bool BuildBasementPath()
        {
             var result = PathBuilder.BuildPath(new Vector3(0, 0, 0), new PathOptions()
            {
                pathType = PathType.Straight_Line,

                primaryPathLength = basementLength,
                primaryDirection = Direction.North,

                tags = new Dictionary<string, string>() { [Tags.REGION] = basementTag, [Tags.SUBREGION] = "Basement_1" }
            });

            nextRootCell = result.Last();

            var direction = Random.Range(0, 2) == 1 ? Direction.West : Direction.East;

            result = PathBuilder.BuildPath(nextRootCell, new PathOptions()
            {
                pathType = PathType.Arched_Line,

                primaryPathLength = basementLength,
                secondaryPathLength = basementLength,

                primaryDirection = Direction.North,
                secondaryDirection = direction,

                tags = new Dictionary<string, string>() { [Tags.REGION] = basementTag, [Tags.SUBREGION] = "Basement_2" }
            });

            nextRootCell = result.Last();

            result = PathBuilder.BuildPath(nextRootCell, new PathOptions()
            {
                pathType = PathType.Curved_Line,
                primaryPathLength = basementLength,
                secondaryPathLength = basementLength,

                primaryDirection = Direction.South,
                secondaryDirection = direction,

                tags = new Dictionary<string, string>() { [Tags.REGION] = basementTag, [Tags.SUBREGION] = "Basement_3" }
            });

            //Elevation cell at this point
            nextRootCell = result.Last();

            //result = PathBuilder.BuildPath(nextRootCell, new PathOptions()
            //{
            //    pathType = PathType.Elevation,

            //    primaryDirection = Direction.South,
            //    elevationDirection = Direction.Up,

            //    tags = new Dictionary<string, string>() { [Tags.REGION] = basementTag, [Tags.SUBREGION] = "Basement_Stairs" }
            //});

            //nextRootCell = result.Last();

            return true;
        }

        private bool BuildBasementCells()
        {
            var cells = CellCollection.collection.Where(x => x.Value.tags.Contains(Tags.REGION, basementTag)).Select(s => s.Value).ToList();
            cells.Remove(CellCollection.collection
                .First(x => x.Value.tags.Contains(Tags.INIT_PATH) 
                && x.Value.tags.Contains(Tags.SUBREGION, "Basement_1")).Value);
            CellBuilder.Expand(cells, new CellOptions()
            {
                expansionAmount = 3
            });
            return true;
        }

        private bool BuildBasementRooms()
        {
            var cells = CellCollection.collection.Where(x => x.Value.tags.Contains(Tags.REGION, basementTag)).Select(s => s.Value).ToList();

            cells.ClaimRooms(ClaimType.Greedy, new RoomOptions() {
                    Region = basementTag,
            });

            return true;
        }
    }
}
