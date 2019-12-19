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
        public static Vector3 Step(this Cell cell, Direction direction)
        {
            switch(direction)
            {
                case Direction.East:
                    return cell.position + new Vector3(1, 0, 0);
                case Direction.South:
                    return cell.position + new Vector3(0, 0, -1);
                case Direction.West:
                    return cell.position + new Vector3(-1, 0, 0);
                case Direction.North:
                default:
                    return cell.position + new Vector3(0, 0, 1);
            }
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
