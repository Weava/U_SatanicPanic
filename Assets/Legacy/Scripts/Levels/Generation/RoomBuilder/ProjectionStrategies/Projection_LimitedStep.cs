using Assets.Scripts.Levels.Generation.Base;
using Assets.Scripts.Levels.Generation.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation.RoomBuilder.ProjectionStrategies
{
    public static class Projection_LimitedStep
    {
        public static List<Cell> Project(Cell root, ref List<Cell> cellsLeftToClaim, int claimAmount, float claimChance)
        {
            var successfulProjection = new List<Cell>();

            foreach (var direction in Directionf.Directions())
            {
                var secondaryDirections = new List<Direction>() { direction.Left(), direction.Right() };

                foreach (var subDirection in secondaryDirections)
                {
                    var cellsLefttoClaimCopy = cellsLeftToClaim.ToList();
                    var attemptedProjection = ProjectionAttempt(root, ref cellsLefttoClaimCopy, claimAmount, claimChance, direction, subDirection);
                    if (attemptedProjection.Count > successfulProjection.Count)
                    {
                        successfulProjection = attemptedProjection;
                    }

                    if (successfulProjection.Count >= claimAmount) break;
                }
            }

            //Fetched cells, now claim them
            foreach (var cell in successfulProjection)
            {
                cellsLeftToClaim.Remove(cell);
            }

            return successfulProjection;
        }

        //This projection type will attempt to claim the most successful projection that claims the most cells possible
        private static List<Cell> ProjectionAttempt(Cell root, ref List<Cell> cellsLeftToClaim, int claimAmount, float claimChance, Direction primaryDirection, Direction secondaryDirection)
        {
            bool terminate = false;

            #region Step 1

            var result = new List<Cell>() { root };
            var claimedAmount = result.Count;
            cellsLeftToClaim.Remove(root);

            if (claimedAmount >= claimAmount) return result;

            #endregion Step 1

            #region Step 2

            var steps = new List<Vector3>()
            {
                root.Step(primaryDirection),    //2_1
                root.Step(secondaryDirection)   //2_2
            };

            foreach (var step in steps)
            {
                if (CellCollection.HasCellAt(step) && cellsLeftToClaim.Contains(CellCollection.cells[step]))
                {
                    var cell = CellCollection.cells[step];
                    result.Add(cell);
                    cellsLeftToClaim.Remove(cell);
                    claimedAmount = result.Count;
                }
                else { terminate = true; }
            }

            if (terminate || result.Count >= claimAmount) return result;

            #endregion Step 2

            #region Step 3

            steps = new List<Vector3>()
            {
                root.Step(primaryDirection).Step(secondaryDirection),           //3_1
                root.Step(primaryDirection.Opposite()).Step(secondaryDirection),//3_2
                root.Step(primaryDirection).Step(secondaryDirection.Opposite()),//3_3
            };

            foreach (var step in steps)
            {
                if (CellCollection.HasCellAt(step) && cellsLeftToClaim.Contains(CellCollection.cells[step]))
                {
                    var cell = CellCollection.cells[step];
                    result.Add(cell);
                    cellsLeftToClaim.Remove(cell);
                    claimedAmount = result.Count;
                }
                else { terminate = true; }
            }

            if (terminate || result.Count >= claimAmount) return result;

            #endregion Step 3

            #region Step 4

            steps = new List<Vector3>()
            {
                root.Step(primaryDirection.Opposite()), //4_1
                root.Step(secondaryDirection.Opposite())//4_2
            };

            foreach (var step in steps)
            {
                if (CellCollection.HasCellAt(step) && cellsLeftToClaim.Contains(CellCollection.cells[step]))
                {
                    var cell = CellCollection.cells[step];
                    result.Add(cell);
                    cellsLeftToClaim.Remove(cell);
                    claimedAmount = result.Count;
                }
                else { terminate = true; }
            }

            if (terminate || result.Count >= claimAmount) return result;

            #endregion Step 4

            #region Step 5

            steps = new List<Vector3>()
            {
                root.Step(primaryDirection.Opposite()).Step(secondaryDirection.Opposite())//5_1
            };

            foreach (var step in steps)
            {
                if (CellCollection.HasCellAt(step) && cellsLeftToClaim.Contains(CellCollection.cells[step]))
                {
                    var cell = CellCollection.cells[step];
                    result.Add(cell);
                    cellsLeftToClaim.Remove(cell);
                    claimedAmount = result.Count;
                }
                else { terminate = true; }
            }

            if (terminate || result.Count >= claimAmount) return result;

            #endregion Step 5

            #region Step 6

            steps = new List<Vector3>()
            {
                root.Step(primaryDirection, 2),                                     //6_1
                root.Step(secondaryDirection, 2),                                   //6_2
                root.Step(primaryDirection, 2).Step(secondaryDirection),            //6_3
                root.Step(primaryDirection).Step(secondaryDirection, 2),            //6_4
                root.Step(primaryDirection.Opposite()).Step(secondaryDirection, 2), //6_5
                root.Step(primaryDirection, 2).Step(secondaryDirection.Opposite()), //6_6
            };

            foreach (var step in steps)
            {
                if (CellCollection.HasCellAt(step) && cellsLeftToClaim.Contains(CellCollection.cells[step]))
                {
                    var cell = CellCollection.cells[step];
                    result.Add(cell);
                    cellsLeftToClaim.Remove(cell);
                    claimedAmount = result.Count;
                }
                else { terminate = true; }
            }

            if (terminate || result.Count >= claimAmount) return result;

            #endregion Step 6

            #region Step 7

            steps = new List<Vector3>()
            {
                root.Step(primaryDirection, 2).Step(secondaryDirection, 2) //7_1
            };

            foreach (var step in steps)
            {
                if (CellCollection.HasCellAt(step) && cellsLeftToClaim.Contains(CellCollection.cells[step]))
                {
                    var cell = CellCollection.cells[step];
                    result.Add(cell);
                    cellsLeftToClaim.Remove(cell);
                    claimedAmount = result.Count;
                }
                else { terminate = true; }
            }

            if (terminate || result.Count >= claimAmount) return result;

            #endregion Step 7

            return result;
        }
    }
}