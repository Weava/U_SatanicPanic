using Assets.Scripts.Levels.Generation.Base;
using Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding.Base;
using System.Collections.Generic;

namespace Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding
{
    public class Node_FloorMain : Scaffold_Node
    {
        public bool elevationOverride = false;
        public Cell root;

        public Node_FloorMain()
        {
            type = ScaffoldNodeType.Floor_Main;
        }
    }

    public class Node_FloorConnector : Scaffold_Node
    {
        //public List<Cell> rootCells = new List<Cell>();
        public Direction normal;

        public Node_FloorConnector()
        {
            type = ScaffoldNodeType.Floor_Connector;
        }
    }

    public class Node_FloorColumn : Scaffold_Node
    {
        //public List<Cell> roots = new List<Cell>();

        public Node_FloorColumn()
        {
            type = ScaffoldNodeType.Floor_Column;
        }
    }
}
