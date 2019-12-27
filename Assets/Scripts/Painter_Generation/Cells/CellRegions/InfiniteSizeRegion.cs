using Assets.Scripts.Misc;
using Assets.Scripts.Painter_Generation.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Painter_Generation.CellRegions
{
    public class InfiniteSizeRegion : CellRegion
    {
        //private List<Direction> excludedDirections = new List<Direction> { Direction.South };

        public InfiniteSizeRegion() : base()
        {
            infiniteSize = true;
        }

        //public bool BuildRegion()
        //{
        //   BuildSpawnCell();
        //   BuildMainPath();
        //   BuildEndCell();

        //   return true;
        //}

        //private void BuildSpawnCell()
        //{
        //    lastCellGenerated = new SpawnCell(new Vector3(0,0,0));
        //    cells.Add(lastCellGenerated);
        //}

        //private void BuildMainPath()
        //{ 
        //    for(int i = 0; i < mainPathLength; i++)
        //    {
        //        var direction = Directionf.RandomDirection(lastCellGenerated.AvailableDirections(cells, excludedDirections));
        //        lastCellGenerated = new MainPathCell(lastCellGenerated.Step(direction));
        //        cells.Add(lastCellGenerated);
        //    }
        //}

        //private void BuildEndCell()
        //{
        //    var direction = Directionf.RandomDirection(lastCellGenerated.AvailableDirections(cells, excludedDirections));
        //    lastCellGenerated = new EndCell(lastCellGenerated.Step(direction));
        //    cells.Add(lastCellGenerated);
        //}
    }
}
