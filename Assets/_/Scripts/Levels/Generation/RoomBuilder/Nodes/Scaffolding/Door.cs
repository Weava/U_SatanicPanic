using Assets.Scripts.Levels.Generation.Base;
using Assets.Scripts.Levels.Generation.Extensions;
using Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding.Base;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding
{
    public class Node_Door : Scaffold_Node
    {
        public Cell cell_1;
        public Cell cell_2;

        public new ScaffoldNodeType type = ScaffoldNodeType.Door;
        public new Vector3 position
        {
            get { return cell_1.PositionBetween(cell_2); }
        }

        public Direction ProjectDirection(Cell cell)
        {
            if (cell != cell_1 && cell != cell_2) return Direction.Up;

            var root = cell == cell_1 ? cell_1 : cell_2;
            var other = cell == cell_1 ? cell_2 : cell_1;

            return root.DirectionToNeighbor(other);
        }
    }

    public static class Node_DoorExtensions
    {
        public static bool Contains(this Node_Door node, Cell cell)
        {
            return node.cell_1 == cell || node.cell_2 == cell;
        }

        public static bool Contains(this Node_Door node, Room room)
        {
            return node.cell_1.roomId == room.id || node.cell_2.roomId == room.id;
        }
    }
}
