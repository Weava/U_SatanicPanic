using Assets.Scripts.Levels.Generation.Base;
using Assets.Scripts.Levels.Generation.Base.Mono;
using Assets.Scripts.Levels.Generation.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

namespace Assets.Scripts.Levels.Generation.CellBuilder
{
    public static class PathExpander
    {
        [Obsolete]
        public static void ExpandCellPoint(Cell rootCell, int expansionAmount = 1)
        {
            if (rootCell.type == CellType.Elevation || rootCell.type == CellType.Spawn) return; //Elevation cells cannot expand

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

        public static void Expand(ref Region region)
        {
            var cellsToAdd = new List<Cell>();

            var sequenceLength = region.cells.Last().sequence;
            var sequenceMiddle = region.cells.Sum(s => s.sequence) / region.cells.Where(x => x.sequence > 0).Count();

            var pathwayCells = region.cells.Where(x => x.type == CellType.Pathway).ToArray();
            foreach (var pathwayCell in pathwayCells) //Ignore elevation cells, those cannot expand
            {
                cellsToAdd = new List<Cell>();
                var expansionAmount = 0;
                foreach(var direction in Directionf.Directions())
                {
                    if (region.cellExpansionConstant)
                    {
                        expansionAmount = region.cellExpansionAmount;
                        var currentCell = pathwayCell;
                        for (int i = 0; i < expansionAmount; i++)
                        {
                            if(CellCollection.HasCellAt(currentCell.position.Step(direction)))
                            {
                                break;
                            }
                            else
                            {
                                var cell = new Cell(CellType.Cell, currentCell.position.Step(direction));
                                cell.parent = currentCell;
                                cell.region = region.regionName;
                                cellsToAdd.Add(cell);
                                currentCell = cell;
                            }
                        }
                    } else
                    {
                        if(pathwayCell.sequence <= sequenceMiddle)
                        {
                            expansionAmount = Mathf.FloorToInt(
                                Mathf.Lerp(region.cellExpansionStart, 
                                region.cellExpansionMiddle, 
                                pathwayCell.sequence / (sequenceLength - sequenceMiddle)));
                        } else
                        {
                            expansionAmount = Mathf.FloorToInt(
                                Mathf.Lerp(region.cellExpansionMiddle,
                                region.cellExpansionEnd,
                                (pathwayCell.sequence - sequenceMiddle)  / (sequenceLength - sequenceMiddle)));
                        }
                        var currentCell = pathwayCell;
                        for (int i = 0; i < expansionAmount; i++)
                        {
                            if (CellCollection.HasCellAt(currentCell.position.Step(direction)))
                            {
                                break;
                            }
                            else
                            {
                                var cell = new Cell(CellType.Cell, currentCell.position.Step(direction));
                                cell.parent = currentCell;
                                cell.region = region.regionName;
                                cellsToAdd.Add(cell);
                                currentCell = cell;
                            }
                        }
                    }
                }
                try
                {
                    CellCollection.Add(cellsToAdd);
                    region.cells.AddRange(cellsToAdd);
                }
                catch (Exception e) {
                    continue; }
            }
        }

        public static void Proliferate(ref Region region)
        {
            var cellsWithOpenings = region.cells.Where(x => x.NeighborOpenings().Any() && x.type == CellType.Cell).ToList();
            var cellsToAdd = new List<Cell>();

            foreach(var cell in cellsWithOpenings)
            {
                //Recheck for openings
                var openings = cell.NeighborOpenings();
                if (!openings.Any()) continue;
                foreach(var opening in openings)
                {
                    var currentCell = cell;
                    for(int i = 0; i < region.proliferationAmount; i++)
                    {
                        if (CellCollection.HasCellAt(currentCell.Step(opening)))
                        {
                            break;
                        } else
                        {
                            var nextCell = new Cell(CellType.Cell, currentCell.Step(opening));
                            nextCell.parent = currentCell;
                            nextCell.region = region.regionName;
                            cellsToAdd.Add(nextCell);
                            currentCell = cell;
                        }
                    }
                }
            }

            region.cells.AddRange(cellsToAdd);
            CellCollection.Add(cellsToAdd);
        }

        public static void DecayCells(ref Region region)
        {
            foreach(var cell in region.cells.Where(x => x.type == CellType.Cell).ToArray())
            {
                var decayHit = Random.Range(0.0f, 1.0f);
                if(region.cellDecayAmount >= decayHit)
                {
                    cell.children = new List<Cell>();
                    CellCollection.Remove(cell);
                    region.cells.Remove(cell);
                }
            }
        }

        public static void CleanIsolatedCells(Region region)
        {
            //These should be the only type of cells that can be potentially isolated
            var cellsToCheck = region.cells.Select(s => s).Where(x => x.type == CellType.Cell).ToList();

            foreach (var cell in cellsToCheck)
            {
                if (cell.FindClosestPathway() == null)
                {
                    CellCollection.Remove(cell);
                    region.cells.Remove(cell);
                }
            }
        }

        [Obsolete]
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
