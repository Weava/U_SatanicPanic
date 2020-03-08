using Assets.Scripts.Levels.Generation.Base;
using Assets.Scripts.Levels.Generation.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation.CellBuilder
{
    public static class PathExpander
    {
        public static void ExpandCellPoint(Cell rootCell, int expansionAmount = 1)
        {
            var cellsToAddToCollection = new List<Cell>();

            var cellsInCurrentExpansion = new List<Cell>();
            cellsInCurrentExpansion.Add(rootCell);

            var cellsForNextExpansion = new List<Cell>();

            while(expansionAmount > 0)
            {
                foreach(var cell in cellsInCurrentExpansion.ToArray())
                {
                    foreach (var direction in Directionf.Directions().ToArray())
                    {
                        if (!CellCollection.HasCellAt(cell.Step(direction)) 
                            && !cellsToAddToCollection.Any(x => x.position == cell.Step(direction))
                            && !cellsForNextExpansion.Any(x => x.position == cell.Step(direction)))
                        {
                            var newCell = new Cell(CellType.Cell, cell.Step(direction));
                            newCell.region = cell.region;
                            newCell.parent = cell;
                            cell.children.Add(newCell);
                            cellsForNextExpansion.Add(newCell);
                        }
                    }
                }

                cellsToAddToCollection.AddRange(cellsForNextExpansion);
                cellsInCurrentExpansion = cellsForNextExpansion;
                cellsForNextExpansion = new List<Cell>();
                expansionAmount--;
            }

            CellCollection.Add(cellsToAddToCollection);
        }

        public static void DecayCells(List<Cell> cells, float decayChance)
        {
            foreach(var cell in cells.Where(x => x.type == CellType.Cell).ToArray())
            {
                var decayHit = Random.Range(0.0f, 1.0f);
                if(decayChance >= decayHit)
                {
                    cell.children = new List<Cell>();
                    CellCollection.Remove(cell);
                }
            }
        }

        public static void CleanIsolatedCells()
        {
            //These should be the only type of cells that can be potentially isolated
            var cellsToCheck = CellCollection.cells.Select(s => s.Value).Where(x => x.type == CellType.Cell).ToList();

            foreach (var cell in cellsToCheck)
            {
                if(cell.FindClosestPathway() == null)
                {
                    CellCollection.Remove(cell);
                }
            }
        }
    }
}
