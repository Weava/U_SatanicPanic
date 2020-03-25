using Assets.Scripts.Levels.Generation.Extensions;
using Assets.Scripts.Levels.Generation.Base;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Assets.Scripts.Levels.Generation.Base.Mono;

using Random = UnityEngine.Random;

namespace Assets.Scripts.Levels.Generation.CellBuilder
{
    public static class PathBuilder
    {
        /// <summary>
        /// Number of times to try to generate a pathway if the pathway collides with existing cells in the cell collection
        /// </summary>
        private static int numberOfRetries;

        private const int RETRIES = 30;

        private static List<Cell> cellsToAdd;

        private static int sequence;

        private static bool elevationFlag = false;
        private static bool elevationInit = true;
        private static bool recentElevation = false;

        [Obsolete]
        public static bool BuildPath_OLD(Vector3 startPosition, Vector3 endPosition, string regionName, bool hasSpawn = false)
        {
            var direction = FindDirectionVector(startPosition, endPosition);

            var xDistance = GetDistance(startPosition.x, endPosition.x, Cellf.CELL_STEP_OFFSET);
            var yDistance = GetDistance(startPosition.y, endPosition.y, Cellf.CELL_STEP_OFFSET / 2);
            var zDistance = GetDistance(startPosition.z, endPosition.z, Cellf.CELL_STEP_OFFSET);

            if (yDistance > 0) elevationFlag = true;

            numberOfRetries = RETRIES;

            bool success = true;

            while (numberOfRetries > 0) { 

                cellsToAdd = new List<Cell>();
                sequence = 0;

                var xTemp = xDistance;
                var yTemp = yDistance;
                var zTemp = zDistance;

                Cell lastCell = null;

                var directionsLeftToGo = new List<Direction>();
                if (xTemp > 0) directionsLeftToGo.Add(direction.xDirection);
                if (yTemp > 0) directionsLeftToGo.Add(direction.yDirection);
                if (zTemp > 0) directionsLeftToGo.Add(direction.zDirection);

                if(lastCell == null)
                {
                    if(!CellCollection.HasCellAt(startPosition))
                    {
                        var type = hasSpawn ? CellType.Spawn : CellType.Pathway;
                        var newCell = new Cell(type, startPosition);
                        newCell.sequence = sequence++;
                        newCell.parent = lastCell;
                        cellsToAdd.Add(newCell);
                        lastCell = newCell;
                    }
                    else
                    {
                        lastCell = CellCollection.cells[startPosition];
                    }
                }

                while(directionsLeftToGo.Any())
                {
                    Direction nextDirection = Direction.North;
                    if(elevationInit || recentElevation) //Prevent a new pathway region or recent elevation from immediatly going up, delay it if possible
                    {
                        if(directionsLeftToGo.Any(x => Directionf.Directions().Contains(x)))
                        {
                            var directionsLeftToGoWithoutGoingUp = directionsLeftToGo.Where(x => x != Direction.Up && x != Direction.Down).ToList();
                            nextDirection = directionsLeftToGoWithoutGoingUp[Random.Range(0, directionsLeftToGoWithoutGoingUp.Count)];
                        } else
                        {
                            nextDirection = directionsLeftToGo[Random.Range(0, directionsLeftToGo.Count)];
                        }
                        elevationInit = false;
                        if(nextDirection != Direction.Up && nextDirection != Direction.Down) recentElevation = false;
                    }
                    else
                    {
                        nextDirection = directionsLeftToGo[Random.Range(0, directionsLeftToGo.Count)];
                    }

                    if(nextDirection == Direction.Up || nextDirection == Direction.Down)
                    { recentElevation = true; }
                    
                    var cellType = (nextDirection == Direction.Up || nextDirection == Direction.Down)
                        ? CellType.Elevation : CellType.Pathway;
                    if (cellType == CellType.Elevation) { lastCell.type = CellType.Elevation; recentElevation = true; } //Retroactivly change the previous celltype to elevation
                    var newCell = new Cell(cellType, lastCell.Step(nextDirection));
                    newCell.sequence = sequence++;
                    newCell.parent = lastCell;
                    lastCell.children.Add(newCell);
                    lastCell = newCell;

                    if (CellCollection.HasCellAt(lastCell.position))
                    {
                        if(xTemp + yTemp + zTemp == 1) //Implies that this is the end cell, which can be claimed by another path
                        {
                            success = true;
                            break;
                        }

                        numberOfRetries--;
                        success = false;
                        break;
                    }
                    else
                    {
                        cellsToAdd.Add(lastCell);
                    }

                    switch(nextDirection)
                    {
                        case Direction.Up:
                        case Direction.Down:
                            yTemp--;
                            if (yTemp <= 0) directionsLeftToGo.Remove(direction.yDirection); 
                            break;
                        case Direction.North:
                        case Direction.South:
                            zTemp--;
                            if (zTemp <= 0) directionsLeftToGo.Remove(direction.zDirection);
                            break;
                        case Direction.East:
                        case Direction.West:
                            xTemp--;
                            if (xTemp <= 0) directionsLeftToGo.Remove(direction.xDirection);
                            break;
                        default:
                            numberOfRetries--;//Something is wrong, kill this process
                            break;
                    }
                    success = true;
                }

                if (success)
                {
                    CellCollection.Add(cellsToAdd);
                    return true;
                };
            }

            return false;
        }

        public static bool BuildPath(ref Region region)
        {
            //Get direction of pathway
            var directions = FindDirectionVector(region.startPosition, region.endPosition);

            var xDistance = GetDistance(region.startPosition.x, region.endPosition.x, Cellf.CELL_STEP_OFFSET);
            var yDistance = GetDistance(region.startPosition.y, region.endPosition.y, Cellf.CELL_STEP_OFFSET / 2);
            var zDistance = GetDistance(region.startPosition.z, region.endPosition.z, Cellf.CELL_STEP_OFFSET);

            var retries = RETRIES;
            var success = false;

            while(retries > 0)
            {
                retries--;

                #region Reset Values
                var finishEndCellMetadata = false;
                var cellsToAdd = new List<Cell>();
                var xTemp = xDistance;
                var yTemp = yDistance;
                var zTemp = zDistance;

                Cell currentCell = null;
                Cell lastCell = null;
                Cell secondLastCell = null; //Used to prevent the end of a path from being an elevation cell
                #endregion

                #region Init
                //Start cell
                if (CellCollection.HasCellAt(region.startPosition)) //If starting from an existing path
                {
                    currentCell = CellCollection.cells[region.startPosition];
                }
                else
                {
                    var cell = new Cell(CellType.Pathway, region.startPosition);
                    cell.sequence = sequence++;
                    cell.region = region.regionName;
                    cell.parent = currentCell;
                    cellsToAdd.Add(cell);
                    currentCell = cell;
                }

                //Force horizontal start
                if(xTemp > 0 || zTemp > 0)
                {
                    var directionsToTry = new List<Direction>();
                    if (xTemp > 0) directionsToTry.Add(directions.xDirection);
                    if (zTemp > 0) directionsToTry.Add(directions.zDirection);

                    var directionToGo = directionsToTry[Random.Range(0, directionsToTry.Count)];

                    if(directionToGo == directions.xDirection)
                    {
                        xTemp--;
                    } else
                    {
                        zTemp--;
                    }

                    if (CellCollection.HasCellAt(currentCell.Step(directionToGo))) { success = false; retries--; continue; }

                    var cell = new Cell(CellType.Pathway, currentCell.Step(directionToGo));
                    cell.sequence = sequence++;
                    cell.region = region.regionName;
                    cell.parent = currentCell;
                    currentCell.children.Add(cell);
                    cellsToAdd.Add(cell);
                    currentCell = cell;
                }

                //End cell
                if (CellCollection.HasCellAt(region.endPosition)) //If connecting to an existing path
                {
                    lastCell = CellCollection.cells[region.endPosition];
                }
                else
                {
                    var cell = new Cell(CellType.Pathway, region.endPosition);
                    //Sequence and parents will have to be added later
                    finishEndCellMetadata = true;
                    cell.region = region.regionName;
                    cellsToAdd.Add(cell);
                    lastCell = cell;
                }

                //Remove a temp increment for the end cell
                var directionsForEnd = new List<Direction>();
                if (xTemp > 0) directionsForEnd.Add(directions.xDirection);
                if (zTemp > 0) directionsForEnd.Add(directions.zDirection);
                var directionToRemoveFrom = directionsForEnd[Random.Range(0, directionsForEnd.Count)];
                if(directionToRemoveFrom == directions.xDirection)
                {
                    xTemp--;
                } else
                {
                    zTemp--;
                }

                //Force horizontal end
                if (xTemp > 0 || zTemp > 0)
                {
                    var directionsToTry = new List<Direction>();
                    if (xTemp > 0) directionsToTry.Add(directions.xDirection.Opposite());
                    if (zTemp > 0) directionsToTry.Add(directions.zDirection.Opposite());

                    var directionToGo = directionsToTry[Random.Range(0, directionsToTry.Count)];

                    if (directionToGo == directions.xDirection.Opposite())
                    {
                        xTemp--;
                    }
                    else
                    {
                        zTemp--;
                    }

                    if (CellCollection.HasCellAt(lastCell.Step(directionToGo))) { success = false; retries--; continue; }

                    var cell = new Cell(CellType.Pathway, lastCell.Step(directionToGo));
                    //Sequence and parent will have to be added later
                    cellsToAdd.Add(cell);
                    cell.children.Add(lastCell);
                    cell.region = region.regionName;

                    lastCell.parent = cell; //End cell parent finalized
                    secondLastCell = cell; //Save this for metadata finalization at the end
                }
                #endregion

                #region Generate
                while (xTemp + zTemp + yTemp > 0)
                {
                    var directionsLeft = GetDirectionsLeft(xTemp, yTemp, zTemp, directions);
                    var currentDirection = directionsLeft[Random.Range(0, directionsLeft.Count)];

                    var type = (currentDirection == Direction.Up || currentDirection == Direction.Down) ? CellType.Elevation : CellType.Pathway;
                    if(type == CellType.Elevation) //Retroactivly change the previous generated cell to be an elevation cell as well
                    {  cellsToAdd.First(x => x == currentCell).type = type; }

                    if(CellCollection.HasCellAt(currentCell.position.Step(currentDirection)))
                    {
                        success = false; retries--; continue;
                    }

                    var cell = new Cell(type, currentCell.position.Step(currentDirection));
                    cell.parent = currentCell;
                    currentCell.children.Add(cell);
                    cell.region = region.regionName;
                    cell.sequence = sequence++;
                    cellsToAdd.Add(cell);
                    currentCell = cell;

                    switch(currentDirection)
                    {
                        case Direction.North:
                        case Direction.South:
                            zTemp--;
                            break;
                        case Direction.East:
                        case Direction.West:
                            xTemp--;
                            break;
                        case Direction.Up:
                        case Direction.Down:
                            yTemp--;
                            break;
                        default:
                            success = false;
                            break;
                    }

                    if (xTemp + yTemp + zTemp == 0) success = true;
                }
                #endregion

                //The last cell generated in the above loop should be the one right before the forced horizontal cell, if that exists
                if(secondLastCell != null)
                {
                    cellsToAdd.First(x => x == secondLastCell).sequence = sequence++;
                    cellsToAdd.First(x => x == secondLastCell).parent = currentCell;
                    currentCell.children.Add(secondLastCell);
                }

                cellsToAdd.First(x => x == lastCell).sequence = sequence++;

                if (success) {
                    CellCollection.Add(cellsToAdd.OrderBy(o => o.sequence).ToList());
                    region.cells.AddRange(cellsToAdd.OrderBy(o => o.sequence));
                    break;
                }
            }

            return success;
        }

        #region Helper methods

        private static List<Direction> GetDirectionsLeft(int x, int y, int z, DirectionVector directions)
        {
            var result = new List<Direction>();

            if(x > 0)
            { result.Add(directions.xDirection); }

            if(y > 0)
            { result.Add(directions.yDirection); }

            if(z > 0)
            { result.Add(directions.zDirection); }

            return result;
        }

        private static DirectionVector FindDirectionVector(Vector3 startPosition, Vector3 endPosition)
        {
            var result = new DirectionVector();

            var start_x = startPosition.x;
            var start_z = startPosition.z;
            var end_x = endPosition.x;
            var end_z = endPosition.z;

            var positive_x = end_x > start_x;
            var positive_z = end_z > start_z;

            result.yDirection = endPosition.y >= startPosition.y ? Direction.Up : Direction.Down;

            if(positive_x)
            { result.xDirection = Direction.East; }
            else
            { result.xDirection = Direction.West; }

            if (positive_z)
            { result.zDirection = Direction.North; }
            else
            { result.zDirection = Direction.South; }

            return result;
        }

        private static int GetDistance(float postition_1, float position_2 , int offsetAmount)
        {
            return (int)(Mathf.Sqrt(Mathf.Pow(position_2 - postition_1, 2)) / offsetAmount);
        }

        private class DirectionVector
        {
            public Direction xDirection;
            public Direction zDirection;
            public Direction yDirection;

            public DirectionVector()
            {
            }
        }

        #endregion
    }
}
