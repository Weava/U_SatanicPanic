using Assets.Scripts.Levels.Generation.Extensions;
using Assets.Scripts.Levels.Generation.Base;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        public static bool BuildPath(Vector3 startPosition, Vector3 endPosition, string regionName, bool hasSpawn = false)
        {
            var direction = FindDirectionVector(startPosition, endPosition);

            var xDistance = GetDistance(startPosition.x, endPosition.x);
            var yDistance = GetDistance(startPosition.y, endPosition.y);
            var zDistance = GetDistance(startPosition.z, endPosition.z);

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
                        newCell.region = regionName;
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
                    newCell.region = regionName;
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

        //TODO: Rework path building

        #region Helper methods

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

        private static float GetDistance(float postition_1, float position_2)
        {
            return (Mathf.Sqrt(Mathf.Pow(position_2 - postition_1, 2)) / Cellf.CELL_STEP_OFFSET);
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
