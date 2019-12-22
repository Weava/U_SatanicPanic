using Assets.Scripts.Painter_Generation.Cells;
using Assets.Scripts.Painter_Generation.Painters.Base;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Painter_Generation.Painters
{
    public class ExpandingPainter : Painter
    {
        public int paddingAmount;

        public override void PaintRegion()
        {
            var cells = region.cells.collection.Where(x => x.Value.cellType == CellType.Main_Path_Cell).Select(s => s.Value).ToList();
            foreach (var mainPathCell in cells)
            {
                var directions = Cellf.AvailableDirections(mainPathCell, region.cells);

                foreach(var direction in directions)
                {
                    var nextPosition = Cellf.Step(mainPathCell, direction);
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
