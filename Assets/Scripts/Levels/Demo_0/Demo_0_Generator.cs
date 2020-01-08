using Assets.Scripts.Generation.Painter.Cells;
using Assets.Scripts.Generation.Painter.Cells.Base;
using Assets.Scripts.Generation.Painter.Cells.Factory;
using Assets.Scripts.Levels.Base;
using Assets.Scripts.Misc;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
            var result = BuildBasement();
            if (!result)
            { throw new System.Exception("Level could not be generated (Basement)"); }

            //Floor 1

            //Floor 2

            //Outside

            //Arena

            RenderMarkers();

            return true;
        }

        private bool BuildBasement()
        {
            var result = PathBuilder.BuildPath(new Vector3(0, 0, 0), PathType.Straight_Line, new PathOptions()
            {
                primaryPathLength = basementLength,
                primaryDirection = Direction.North,
                tags = new List<string>() { basementTag, "Region_Basement_1" }
            });
            if (!result) return false;
            nextRootCell = CellCollection.GetPathCellInSequence("Region_Basement_1", PathSequence.Last);

            var direction = Random.Range(0, 2) == 1 ? Direction.West : Direction.East;

            PathBuilder.BuildPath(nextRootCell, PathType.Arched_Line, new PathOptions()
            {
                primaryPathLength = basementLength,
                secondaryPathLength = basementLength,
                primaryDirection = Direction.North,
                secondaryDirection = direction,
                tags = new List<string> { basementTag, "Region_Basement_2" }
            });
            if (!result) return false;
            nextRootCell = CellCollection.GetPathCellInSequence("Region_Basement_2", PathSequence.Last);

            PathBuilder.BuildPath(nextRootCell, PathType.Curved_Line, new PathOptions()
            {
                primaryPathLength = basementLength,
                secondaryPathLength = basementLength,
                primaryDirection = Direction.South,
                secondaryDirection = direction,
                tags = new List<string> { basementTag, "Region_Basement_3" }
            });
            if (!result) return false;
            //nextRootCell = CellCollection.GetPathCellInSequence("Region_Basement_3", PathSequence.Last);

            return true;
        }
    }
}
