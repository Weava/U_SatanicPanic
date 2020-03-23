using Assets.Scripts.Levels.Generation.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation.Base
{
    public static class Level
    {
        public static List<Node_Door> doors = new List<Node_Door>();
        public static Dictionary<Room, Scaffold> roomScaffolds = new Dictionary<Room, Scaffold>();
    }

    #region Meta Containers

    public class Node
    {
        public Node() { }

        public Node(Vector3 position)
        {
            this.position = position;
        }

        public Vector3 position;
        public ScaffoldNodeType type;
    }

    #region Node Types

    public class Node_Door : Node
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

            var root  = cell == cell_1 ? cell_1 : cell_2;
            var other = cell == cell_1 ? cell_2 : cell_1;

            return root.DirectionToNeighbor(other);
        }
    }

    #region Floor

    public class Node_FloorMain : Node
    {
        public new ScaffoldNodeType type = ScaffoldNodeType.Floor_Main;
        public bool elevationOverride = false;
        public Cell root;
    }

    public class Node_FloorConnector : Node
    {
        public new ScaffoldNodeType type = ScaffoldNodeType.Floor_Connector;
        public List<Cell> rootCells = new List<Cell>();
    }

    public class Node_FloorColumn : Node
    {
        public new ScaffoldNodeType type = ScaffoldNodeType.Floor_Column;
        public List<Cell> roots = new List<Cell>();
    }

    #endregion

    #region Walls

    public class Node_WallMain : Node
    {
        public new ScaffoldNodeType type = ScaffoldNodeType.Wall_Main;
        public Direction direction;
        public Cell root;
    }

    public class Node_WallConnector : Node
    {
        public new ScaffoldNodeType type = ScaffoldNodeType.Wall_Connector;
        public Direction direction;
        public Node_FloorConnector root;
    }

    #endregion

    #region Ceiling

    public class Node_CeilingMain : Node
    {
        public new ScaffoldNodeType type = ScaffoldNodeType.Ceiling_Main;
        public Node_FloorMain root;
        public bool elevationOverride = false;
    }

    public class Node_CeilingConnector : Node
    {
        public new ScaffoldNodeType type = ScaffoldNodeType.Ceiling_Connector;
        public Node_FloorConnector root;
    }

    public class Node_CeilingColumn : Node
    {
        public new ScaffoldNodeType type = ScaffoldNodeType.Ceiling_Column;
        public Node_FloorColumn root;
    }

    #endregion

    public class Node_Elevation : Node
    {
        public new ScaffoldNodeType type = ScaffoldNodeType.Elevation;
        public Cell lower;
        public Cell upper;
        public Direction direction;
    }

    #endregion

    public class Scaffold
    {
        public Floor floor = new Floor();
        public Wall wall = new Wall();
        public Ceiling ceiling = new Ceiling();

        public List<Node_Elevation> elevation = new List<Node_Elevation>();
    }

    #region Floor

    public class Floor
    {
        public List<Node_FloorMain> main = new List<Node_FloorMain>();
        public List<Node_FloorConnector> connectors = new List<Node_FloorConnector>();
        public List<Node_FloorColumn> columns = new List<Node_FloorColumn>();
    }

    public class Wall
    {
        public List<Node_WallMain> main = new List<Node_WallMain>();
        public List<Node_WallConnector> connectors = new List<Node_WallConnector>();
    }

    public class Ceiling
    {
        public List<Node_CeilingMain> main = new List<Node_CeilingMain>();
        public List<Node_CeilingConnector> connectors = new List<Node_CeilingConnector>();
        public List<Node_CeilingColumn> columns = new List<Node_CeilingColumn>();
    }

    #endregion

    public enum ScaffoldNodeType
    {
        Door,
        Elevation,

        //Floor Types
        Floor_Main,
        Floor_Connector,
        Floor_Column,

        //Wall Types
        Wall_Main,
        Wall_Connector,
        Wall_Corner,

        //Ceiling Types
        Ceiling_Main,
        Ceiling_Connector,
        Ceiling_Column,
    }

    #endregion
}
