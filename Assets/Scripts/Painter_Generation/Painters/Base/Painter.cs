using Assets.Scripts.Painter_Generation.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Painter_Generation.Painters.Base
{
    public class Painter : MonoBehaviour
    {
        public CellRegion region;

        public List<Direction> excludedDirections = new List<Direction> { Direction.South };

        public virtual void PaintRegion()
        {

        }

        protected void BuildCell(Vector3 position)
        {
             AddCell(new Cell(position));
        }

        protected void BuildSpawnCell()
        {
            AddCell(new SpawnCell(new Vector3(0, 0, 0)));
        }

        protected void BuildMainPath()
        {
            for (int i = 0; i < region.mainPathLength; i++)
            {
                var direction = Directionf.RandomDirection(region.lastCellGenerated.AvailableDirections(region.cells, excludedDirections));
                AddCell(new MainPathCell(region.lastCellGenerated.Step(direction)));
            }
        }

        protected void BuildEndCell()
        {
            var direction = Directionf.RandomDirection(region.lastCellGenerated.AvailableDirections(region.cells, excludedDirections));
            AddCell(new EndCell(region.lastCellGenerated.Step(direction)));
        }

        protected void AddCell(Cell cell)
        {
            region.lastCellGenerated = cell;
            region.cells.Add(region.lastCellGenerated);
        }
    }
}
