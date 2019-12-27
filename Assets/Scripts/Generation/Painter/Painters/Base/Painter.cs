using Assets.Scripts.Generation.Extensions;
using Assets.Scripts.Generation.Painter;
using Assets.Scripts.Generation.Painter.Cells;
using Assets.Scripts.Generation.Painter.Cells.Base;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Painter.Painters.Base
{
    public class Painter : MonoBehaviour
    {
        public Region region;

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
