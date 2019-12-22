using Assets.Scripts.Painter_Generation.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Painter_Generation
{
    public static class Cellf
    {
        private static readonly int CELL_SCALE = 8;

        public static Vector3 Step(Vector3 position, Direction direction)
        {
            switch (direction)
            {
                case Direction.East:
                    return position + new Vector3(CELL_SCALE, 0, 0);
                case Direction.South:
                    return position + new Vector3(0, 0, -CELL_SCALE);
                case Direction.West:
                    return position + new Vector3(-CELL_SCALE, 0, 0);
                case Direction.North:
                default:
                    return position + new Vector3(0, 0, CELL_SCALE);
            }
        }

        public static Vector3 Step(this Cell cell, Direction direction)
        {
            return Step(cell.position, direction);
        }

        public static List<Direction> AvailableDirections(this Cell cell, CellCollection collection, List<Direction> excludeDirections = null)
        {
            var directions = Directionf.GetDirectionList();
            if(excludeDirections != null)
            {
                foreach(var direction in excludeDirections)
                {
                    if (directions.Contains(direction)) directions.Remove(direction);
                }
            }
            var result = new List<Direction>();

            foreach(var direction in directions)
            {
                if( ! collection.HasCellAt(cell.Step(direction)))
                    result.Add(direction);
            }

            return result;
        }
    }
}
