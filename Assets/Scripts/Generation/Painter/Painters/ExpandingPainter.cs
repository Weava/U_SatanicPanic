using Assets.Scripts.Generation.Extensions;
using Assets.Scripts.Generation.Painter.Cells.Base;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Painter_Generation.Painters
{
    public class ExpandingPainter : Painter.Painters.Base.Painter
    {
        public int paddingAmount;

        public override void PaintRegion()
        {
            var cells = region.cells.collection.Where(x => x.Value.cellType == CellType.Main_Path_Cell).Select(s => s.Value).ToList();
            foreach (var mainPathCell in cells)
            {
                var directions = mainPathCell.AvailableDirections(region.cells);

                foreach(var direction in directions)
                {
                    var nextPosition = mainPathCell.Step(direction);
                    for(int i = 0; i < Random.Range(0, paddingAmount+1); i++)
                    {
                        if(!(region.CellIsHere(nextPosition)))
                        {
                            AddCell(new Cell(nextPosition));
                        }

                        nextPosition = Cellf.Step(nextPosition, direction);
                    }
                }
            }
        }
    }
}
