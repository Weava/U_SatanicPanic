using Assets.Scripts.Levels.Generation.Base;
using Assets.Scripts.Levels.Generation.Extensions;
using Assets.Scripts.Misc;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Levels.Generation.RoomBuilder.ProjectionStrategies
{
    public static class Projection_Bloom
    {
        public static List<Cell> Project(Cell root, ref List<Cell> cellsLeftToClaim, int claimAmount)
        {
            var result = new List<Cell>() { root };
            var claimedAmount = result.Count;

            var currentRoots = new List<Cell> { root };

            while (claimedAmount < claimAmount)
            {
                var nextRoots = new List<Cell>();
                foreach (var currentRoot in currentRoots.ToList())
                {
                    foreach (var direction in Directionf.Directions().Shuffle())
                    {
                        var target = currentRoot.Step(direction);
                        if (CellCollection.HasCellAt(target) && cellsLeftToClaim.Any(x => x.position == target))
                        {
                            nextRoots.Add(CellCollection.cells[target]);
                            cellsLeftToClaim.Remove(CellCollection.cells[target]);
                            claimedAmount++;
                            result.Add(CellCollection.cells[target]);
                        }
                    }
                    if (!nextRoots.Any()) //Ran out of cells to claim, just take what we got
                    { return result; }
                }
                currentRoots = nextRoots;
            }

            return result;
        }
    }
}
