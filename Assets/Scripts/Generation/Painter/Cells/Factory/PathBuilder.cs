using Assets.Scripts.Generation.Extensions;
using Assets.Scripts.Generation.Painter.Cells.Base;
using Assets.Scripts.Misc;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Generation.Painter.Cells.Factory
{
    public static class PathBuilder
    {
        public static List<Cell> BuildPath(Vector3 startPosition, PathOptions options)
        {
            var currentPosition = startPosition;
            if (CellCollection.HasCellAt(currentPosition))
            {
                currentPosition = Cellf.Step(currentPosition, options.primaryDirection);
            }

            var result = new List<Cell>();
            switch(options.pathType)
            {
                case PathType.Straight_Line:
                    result = BuildPath_StraightLine(currentPosition, options);
                    break;
                case PathType.Curved_Line:
                    result = BuildPath_CurvedLine(currentPosition, options);
                    break;
                case PathType.Arched_Line:
                    result = BuildPath_ArchedLine(currentPosition, options);
                    break;
                case PathType.Elevation:
                    result = BuildPath_Elevation(currentPosition, options);
                    break;
                default:
                    return result; //This shouldn't be called for any reason
            }
            Cellf.ResetSequence();
            return result;
        }

        public static List<Cell> BuildPath(Cell startCell, PathOptions options)
        {
            return BuildPath(startCell.position, options);
        }

        #region Builders

        /// <summary>
        /// Builds a pathway that is straight across a single direction
        /// </summary>
        /// <param name="position"></param>
        /// <param name="options">Used: [Path Length] [Primary Direction] [(?)Tags]</param>
        /// <returns></returns>
        private static List<Cell> BuildPath_StraightLine(Vector3 position, PathOptions options)
        {
            var cellsToAdd = new List<Cell>();

            var init = true;

            for(int i = 0; i < options.pathLength; i++)
            {
                if(CellCollection.HasCellAt(position)) return new List<Cell>();

                AddCell(ref init, ref cellsToAdd, i, position, options);

                position = position.Step(options.primaryDirection);
            }

            CellCollection.AddRange(cellsToAdd);

            return cellsToAdd;
        }

        /// <summary>
        /// Builds a pathway that is curved from a primary direction to a secondary direction
        /// </summary>
        /// <param name="position"></param>
        /// <param name="options">Used: [Path Length] [Primary Direction] [Secondary Direction] [(?)Tags]</param>
        /// <returns></returns>
        private static List<Cell> BuildPath_CurvedLine(Vector3 position, PathOptions options)
        {
            var primaryLength = options.primaryPathLength;
            var secondaryLength = options.secondaryPathLength;
            var cellsToAdd = new List<Cell>();

            var init = true;

            for(int i = 0; i < options.pathLength; i++)
            {
                if (CellCollection.HasCellAt(position)) return new List<Cell>();

                AddCell(ref init, ref cellsToAdd, i, position, options);

                if (primaryLength > 0 && secondaryLength > 0)
                {
                    var choice = Random.Range(0, 2);
                    if(choice == 1)
                    {
                        position = position.Step(options.primaryDirection);
                        primaryLength--;
                    } else
                    {
                        position = position.Step(options.secondaryDirection);
                        secondaryLength--;
                    }
                } else if (primaryLength > 0)
                {
                    position = position.Step(options.primaryDirection);
                    primaryLength--;
                } else
                {
                    position = position.Step(options.secondaryDirection);
                    secondaryLength--;
                }               
            }

            CellCollection.AddRange(cellsToAdd);

            return cellsToAdd;
        }

        /// <summary>
        /// Builds a path that circles back into the opposite direction of the original direction, based off a secondary direction
        /// </summary>
        /// <param name="position"></param>
        /// <param name="options">Used: [Path Length] [Primary Direction] [Secondary Direction] [(?)Tags]</param>
        /// <returns></returns>
        private static List<Cell> BuildPath_ArchedLine(Vector3 position, PathOptions options)
        {
            var primaryLength_Stage1 = Mathf.CeilToInt(options.primaryPathLength / 2.0f);
            var primaryLength_Stage2 = options.primaryPathLength - primaryLength_Stage1;

            var secondaryLength_Stage1 = Mathf.CeilToInt(options.secondaryPathLength / 2.0f);
            var secondaryLength_Stage2 = options.secondaryPathLength - secondaryLength_Stage1;

            var stage2 = false;
            var collisionFlag = false;

            var cellsToAdd = new List<Cell>();

            var init = true;

            for (int i = 0; i < options.pathLength; i++)
            {
                if (CellCollection.HasCellAt(position)) return new List<Cell>();

                AddCell(ref init, ref cellsToAdd, i, position, options);

                if (stage2)
                {
                    if(primaryLength_Stage2 > 0 && secondaryLength_Stage2 > 0 && ! collisionFlag)
                    {
                        var choice = Random.Range(0, 2);
                        if (choice == 1)
                        {
                            position = position.Step(options.primaryDirection.GetOppositeDirection());
                            primaryLength_Stage2--;
                        }
                        else
                        {
                            position = position.Step(options.secondaryDirection);
                            secondaryLength_Stage2--;
                        }
                    } else if (primaryLength_Stage2 > 0 && ! collisionFlag)
                    {
                        position = position.Step(options.primaryDirection.GetOppositeDirection());
                        primaryLength_Stage2--;
                    } else
                    {
                        if (collisionFlag) { collisionFlag = false; }
                        position = position.Step(options.secondaryDirection);
                        secondaryLength_Stage2--;
                    }
                } else
                {
                    if(primaryLength_Stage1 > 0 && secondaryLength_Stage1 > 0)
                    {
                        var choice = Random.Range(0, 2);
                        if (choice == 1)
                        {
                            position = position.Step(options.primaryDirection);
                            primaryLength_Stage1--;
                        }
                        else
                        {
                            position = position.Step(options.secondaryDirection);
                            secondaryLength_Stage1--;
                        }
                    } else if(primaryLength_Stage1 > 0)
                    {
                        position = position.Step(options.primaryDirection);
                        primaryLength_Stage1--;
                        if(primaryLength_Stage1 == 0)
                        {  collisionFlag = true; stage2 = true; }
                    }
                    else
                    {
                        position = position.Step(options.secondaryDirection);
                        secondaryLength_Stage1--;
                        if(secondaryLength_Stage1 == 0)
                        { stage2 = true; }
                    }
                }
            }

            CellCollection.AddRange(cellsToAdd);

            return cellsToAdd;
        }

        private static List<Cell> BuildPath_Elevation(Vector3 position, PathOptions options)
        {
            var cellsToAdd = new List<Cell>();

            var init = true;

            if (CellCollection.HasCellAt(position)) return new List<Cell>();

            AddCell(ref init, ref cellsToAdd, 0, position, options);

            AddCell(ref init, ref cellsToAdd, 1, position + (Directionf.DirectionToVector(options.elevationDirection) * 8), options);

            CellCollection.AddRange(cellsToAdd);

            return cellsToAdd;
        }

        #endregion

        private static void AddCell(ref bool init, ref List<Cell> cellsToAdd, int index, Vector3 position, PathOptions options)
        {
            if (init)
            {
                init = false;

                var cell = new Cell(position, CellType.Path_Cell);
                cell.tags.Add(options.tags);
                cell.tags.Add(Tags.INIT_PATH);
                cellsToAdd.Add(cell);
            }
            else
            {
                var cell = new Cell(position, CellType.Path_Cell);
                cell.tags.Add(options.tags);
                cellsToAdd.Add(cell);
            }
        }
    }

    public class PathOptions
    {
        public int pathLength { get { return primaryPathLength + secondaryPathLength; } }

        public int primaryPathLength = 0;

        public int secondaryPathLength = 0;

        public Dictionary<string, string> tags = new Dictionary<string, string>();

        public Vector3 elevationAmount = new Vector3();

        #region Enums

        //Cell that caps the end of the generated path
        public CellType capCell = CellType.Path_Cell;

        public CellType startCell = CellType.Path_Cell;

        public PathType pathType;

        public Direction primaryDirection;

        public Direction secondaryDirection;

        public Direction elevationDirection;

        #endregion
    }

    public enum PathType
    {
        Straight_Line,
        Curved_Line,
        Arched_Line,
        Elevation
    }
}
