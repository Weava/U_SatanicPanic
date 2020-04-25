using Assets.Scripts.Levels.Generation.Base;
using Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding.Base;
using System.Collections.Generic;

namespace Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding
{
    public class Node_FloorMain : Scaffold_Node
    {
        public new ScaffoldNodeType type = ScaffoldNodeType.Floor_Main;
        public bool elevationOverride = false;
        public Cell root;
    }

    public class Node_FloorConnector : Scaffold_Node
    {
        public new ScaffoldNodeType type = ScaffoldNodeType.Floor_Connector;
        public List<Cell> rootCells = new List<Cell>();
    }

    public class Node_FloorColumn : Scaffold_Node
    {
        public new ScaffoldNodeType type = ScaffoldNodeType.Floor_Column;
        public List<Cell> roots = new List<Cell>();
    }
}
