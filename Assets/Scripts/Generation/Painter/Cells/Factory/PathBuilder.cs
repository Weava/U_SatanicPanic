using Assets.Scripts.Generation.Extensions;
using Assets.Scripts.Generation.Painter.Cells.Base;
using Assets.Scripts.Misc;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Generation.Painter.Cells.Factory
{
    public static class PathBuilder
    {
        public static bool BuildPath(Vector3 startPosition, PathType pathType, PathOptions options)
        {
            var currentPosition = startPosition;
            if (CellCollection.HasCellAt(currentPosition))
            {
                currentPosition = Cellf.Step(currentPosition, options.primaryDirection);
            }

            var result = true;
            switch(pathType)
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
                default:
                    return false; //This shouldn't be called for any reason
            }
            PathCell.ResetSequence();
            return result;
        }

        public static bool BuildPath(Cell startCell, PathType pathType, PathOptions options)
        {
            return BuildPath(startCell.position, pathType, options);
        }

        //TODO: Detect collissions and reroute pathways as appropriate

        #region Builders

        /// <summary>
        /// Builds a pathway that is straight across a single direction
        /// </summary>
        /// <param name="position"></param>
        /// <param name="options">Used: [Path Length] [Primary Direction] [(?)Tags]</param>
        /// <returns></returns>
        private static bool BuildPath_StraightLine(Vector3 position, PathOptions options)
        {
            var cellsToAdd = new List<Cell>();

            var init = true;

            for(int i = 0; i < options.pathLength; i++)
            {
                if(CellCollection.HasCellAt(position)) return false;

                if(init)
                {
                    init = false;
                    cellsToAdd.Add(new PathCell(position, BuildInitTags(options.tags)));
                } else
                {
                    cellsToAdd.Add(new PathCell(position, options.tags));
                }
                position = position.Step(options.primaryDirection);
            }

            CellCollection.AddRange(cellsToAdd);

            return true;
        }

        /// <summary>
        /// Builds a pathway that is curved from a primary direction to a secondary direction
        /// </summary>
        /// <param name="position"></param>
        /// <param name="options">Used: [Path Length] [Primary Direction] [Secondary Direction] [(?)Tags]</param>
        /// <returns></returns>
        private static bool BuildPath_CurvedLine(Vector3 position, PathOptions options)
        {
            var primaryLength = options.primaryPathLength;
            var secondaryLength = options.secondaryPathLength;
            var cellsToAdd = new List<Cell>();

            var init = true;

            for(int i = 0; i < options.pathLength; i++)
            {
                if (CellCollection.HasCellAt(position)) return false;

                if (init)
                {
                    init = false;
                    cellsToAdd.Add(new PathCell(position, BuildInitTags(options.tags)));
                }
                else
                {
                    cellsToAdd.Add(new PathCell(position, options.tags));
                }

                if(primaryLength > 0 && secondaryLength > 0)
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

            return true;
        }

        /// <summary>
        /// Builds a path that circles back into the opposite direction of the original direction, based off a secondary direction
        /// </summary>
        /// <param name="position"></param>
        /// <param name="options">Used: [Path Length] [Primary Direction] [Secondary Direction] [(?)Tags]</param>
        /// <returns></returns>
        private static bool BuildPath_ArchedLine(Vector3 position, PathOptions options)
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
                if (CellCollection.HasCellAt(position)) return false;

                if (init)
                {
                    init = false;
                    cellsToAdd.Add(new PathCell(position, BuildInitTags(options.tags)));
                }
                else
                {
                    cellsToAdd.Add(new PathCell(position, options.tags));
                }

                if(stage2)
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

            return true;
        }

        #endregion

        private static List<string> BuildInitTags(List<string> tags)
        {
            var result = new List<string>();
            result.AddRange(tags);
            result.Add(Tags.INIT_PATH);
            return result;
        }
    }

    public class PathOptions
    {
        public int pathLength { get { return primaryPathLength + secondaryPathLength; } }

        public int primaryPathLength = 0;

        public int secondaryPathLength = 0;

        public List<string> tags  = new List<string>();

        public Direction primaryDirection;

        public Direction secondaryDirection;
    }
     
    public enum PathType
    {
        Straight_Line,
        Curved_Line,
        Arched_Line
    }
}
