using Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding.Base;

namespace Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding
{
    public class Node_CeilingMain : Scaffold_Node
    {
        public Node_FloorMain root;
        public bool elevationOverride = false;

        public Node_CeilingMain()
        {
            type = ScaffoldNodeType.Ceiling_Main;
        }
    }

    public class Node_CeilingConnector : Scaffold_Node
    {
        public Node_FloorConnector root;

        public Direction normal;

        public Node_CeilingConnector()
        {
            type = ScaffoldNodeType.Ceiling_Connector;
        }
    }

    public class Node_CeilingColumn : Scaffold_Node
    {
        public Node_FloorColumn root;

        public Node_CeilingColumn()
        {
            type = ScaffoldNodeType.Ceiling_Column;
        }
    }
}