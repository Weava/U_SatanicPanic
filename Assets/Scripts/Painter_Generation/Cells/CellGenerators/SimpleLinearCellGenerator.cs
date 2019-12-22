using Assets.Scripts.Painter_Generation.CellRegions;
using Assets.Scripts.Painter_Generation.Cells.CellGenerators.Base;
using Assets.Scripts.Painter_Generation.Painters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Painter_Generation.Cells.CellGenerators
{
    public class SimpleLinearCellGenerator : CellGenerator
    {
        public bool pathPadding;

        public int paddingAmount;

        public InfiniteSizeRegion cellRegion;

        public LinearPainter mainPathPainter;

        /// <summary>
        /// Floof up the borders  of the main path with decorative cells
        /// </summary>
        public ExpandingPainter cellExpandingPainter;

        private void Start()
        {
            Generate();
        }

        public override void Generate()
        {
            mainPathPainter.region = cellRegion;
            mainPathPainter.PaintRegion();

            cellExpandingPainter.region = cellRegion;
            cellExpandingPainter.paddingAmount = paddingAmount;
            cellExpandingPainter.PaintRegion();

            foreach(var cell in cellRegion.cells.collection.Select(s => s.Value))
            {
                BuildMarker(cell.position, cell.cellType);
            }
        }
    }
}
