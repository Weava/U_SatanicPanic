using Assets.Scripts.Levels.Generation.Base;
using Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding.Base;

namespace Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding
{
    public class Node_Elevation : Scaffold_Node
    {
        public new ScaffoldNodeType type = ScaffoldNodeType.Elevation;
        public Cell lower;
        public Cell upper;
        public Direction direction;
    }
}