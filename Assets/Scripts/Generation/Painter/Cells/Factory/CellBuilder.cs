using Assets.Scripts.Generation.Extensions;
using Assets.Scripts.Generation.Painter.Cells.Base;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Generation.Painter.Cells.Factory
{
    public static class CellBuilder
    {
        public static List<Cell> Expand(List<Cell> rootCells, CellOptions options)
        {
            var cellsToAdd = new List<Cell>();

            foreach(var cell in rootCells)
            {
                cellsToAdd.AddRange(Expand(cell, options));
            }

            return cellsToAdd;
        }

        public static List<Cell> Expand(Cell rootCell, CellOptions options)
        {
            var cellsToAdd = new List<Cell>();

            var expansionStep = 1;
            var expansionDirections = Directionf.GetDirectionList();

            while(true)
            {
                if (expansionStep >= options.expansionAmount) break;
                foreach(var direction in expansionDirections.ToList())
                {
                    if(CellCollection.HasCellAt(rootCell.Step(direction, expansionStep)))
                    {  expansionDirections.Remove(direction); }
                    else
                    {
                        AddCell(rootCell.Step(direction, expansionStep), rootCell, ref cellsToAdd);
                    }
                }
                expansionStep++;
            }

            CellCollection.AddRange(cellsToAdd);

            return cellsToAdd;
        }

        public static void Decay(List<Cell> cells, CellOptions options)
        {
            foreach(var cell in cells.Where(x => !x.important).ToList())
            {
                var chance = Random.Range(0.0f, 1.0f);
                if(chance <= options.decayRate)
                {
                    CellCollection.Remove(cell);
                }
            }
        }

        public static void CleanUpIsolatedCells()
        {
            var cells = CellCollection.collection.Where(x => !x.Value.important).Select(s => s.Value).ToList();
            foreach(var cell in cells.ToList())
            {
                if(!cell.CellConnections(true).Any(x => x.important))
                {
                    CellCollection.Remove(cell);
                }
            }
        }

        private static void AddCell(Vector3 position, Cell rootCell, ref List<Cell> cellsToAdd)
        {
            var cell = new Cell(position, CellType.Cell);
            cell.tags = rootCell.tags;
            cell.Region = rootCell.Region;
            cell.Subregion = rootCell.Subregion;
            cellsToAdd.Add(cell);
        }
    }

    public class CellOptions
    {
        public int expansionAmount;
        public float decayRate;
    }
}
