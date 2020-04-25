using Assets.Scripts.Levels.Generation.Base;
using Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding.Base;

namespace Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding
{
    public class Node_WallMain : Scaffold_Node
    {
        public new ScaffoldNodeType type = ScaffoldNodeType.Wall_Main;
        public Direction direction;
        public Cell root;
    }

    public class Node_WallConnector : Scaffold_Node
    {
        public new ScaffoldNodeType type = ScaffoldNodeType.Wall_Connector;
        public Direction direction;
        public Node_FloorConnector root;
    }
}
