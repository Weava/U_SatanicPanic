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
    }

    #region Floor

    public class Node_FloorMain : Node
    {
        public new ScaffoldNodeType type = ScaffoldNodeType.Floor_Main;
    }

    public class Node_FloorConnector : Node
    {
        public new ScaffoldNodeType type = ScaffoldNodeType.Floor_Connector;
        public Cell rootCell;
    }

    public class Node_FloorColumn : Node
    {
        public new ScaffoldNodeType type = ScaffoldNodeType.Floor_Column;
    }

    #endregion

    #region Walls

    public class Node_WallMain : Node
    {
        public new ScaffoldNodeType type = ScaffoldNodeType.Wall_Main;
        public Cell root;
    }

    #endregion

    #endregion

    public class Scaffold
    {
        public Floor floor = new Floor();
        public Wall wall = new Wall();
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
    }

    #endregion

    public enum ScaffoldNodeType
    {
        Door,
        //Floor Types
        Floor_Main,
        Floor_Connector,
        Floor_Column,
        //Wall Types
        Wall_Main,
        Wall_Connector,
        Wall_Corner
    }

    #endregion
}
