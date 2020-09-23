using Assets.Scripts.Levels.Generation.Base;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding.Base
{
    public class Scaffold_Node : Node
    {
        public ScaffoldNodeType type;
        public List<Cell> rootCells = new List<Cell>();
        public Vector3 offsetRoot; //Used for nodes that base their position off of another position
    }

    public class Scaffold
    {
        public string roomId;

        public Floor floor = new Floor();
        public Wall wall = new Wall();
        public Ceiling ceiling = new Ceiling();

        public List<Node_Elevation> elevation = new List<Node_Elevation>();

        public List<Scaffold_Node> GetFlattenedNodes()
        {
            var result = new List<Scaffold_Node>();

            result.AddRange(floor.columns);
            result.AddRange(floor.connectors);
            result.AddRange(floor.main);

            result.AddRange(wall.connectors);
            result.AddRange(wall.main);

            result.AddRange(ceiling.columns);
            result.AddRange(ceiling.connectors);
            result.AddRange(ceiling.main);

            result.AddRange(elevation);

            return result;
        }

        public bool SetNodeClaimed(string nodeId)
        {
            if (floor.columns.Any(x => x.id == nodeId))
            {
                floor.columns.First(x => x.id == nodeId).claimed = true;
                SaveChanges();
                return true;
            }
            else if (floor.connectors.Any(x => x.id == nodeId))
            {
                floor.connectors.First(x => x.id == nodeId).claimed = true;
                SaveChanges();
                return true;
            }
            else if (floor.main.Any(x => x.id == nodeId))
            {
                floor.main.First(x => x.id == nodeId).claimed = true;
                SaveChanges();
                return true;
            }
            else if (wall.main.Any(x => x.id == nodeId))
            {
                wall.main.First(x => x.id == nodeId).claimed = true;
                SaveChanges();
                return true;
            }
            else if (wall.connectors.Any(x => x.id == nodeId))
            {
                wall.connectors.First(x => x.id == nodeId).claimed = true;
                SaveChanges();
                return true;
            }
            else if (ceiling.columns.Any(x => x.id == nodeId))
            {
                ceiling.columns.First(x => x.id == nodeId).claimed = true;
                SaveChanges();
                return true;
            }
            else if (ceiling.connectors.Any(x => x.id == nodeId))
            {
                ceiling.connectors.First(x => x.id == nodeId).claimed = true;
                SaveChanges();
                return true;
            }
            else if (ceiling.main.Any(x => x.id == nodeId))
            {
                ceiling.main.First(x => x.id == nodeId).claimed = true;
                SaveChanges();
                return true;
            }
            else if (elevation.Any(x => x.id == nodeId))
            {
                elevation.First(x => x.id == nodeId).claimed = true;
                SaveChanges();
                return true;
            }

            return false;
        }

        public void SaveChanges()
        {
            Level.roomScaffolds[roomId] = this;
        }
    }

    #region Scaffold Containers

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

    #endregion Scaffold Containers

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

        //Ceiling Types
        Ceiling_Main,

        Ceiling_Connector,
        Ceiling_Column,
    }
}