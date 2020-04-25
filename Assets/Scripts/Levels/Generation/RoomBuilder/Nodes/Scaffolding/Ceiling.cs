using Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding.Base;

namespace Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding
{
    public class Node_CeilingMain : Scaffold_Node
    {
        public new ScaffoldNodeType type = ScaffoldNodeType.Ceiling_Main;
        public Node_FloorMain root;
        public bool elevationOverride = false;
    }

    public class Node_CeilingConnector : Scaffold_Node
    {
        public new ScaffoldNodeType type = ScaffoldNodeType.Ceiling_Connector;
        public Node_FloorConnector root;
    }

    public class Node_CeilingColumn : Scaffold_Node
    {
        public new ScaffoldNodeType type = ScaffoldNodeType.Ceiling_Column;
        public Node_FloorColumn root;
    }

}
