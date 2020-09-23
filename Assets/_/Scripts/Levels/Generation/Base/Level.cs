using Assets.Scripts.Levels.Generation.Rendering.Suites;
using Assets.Scripts.Levels.Generation.RoomBuilder.Nodes;
using Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Parsing.Base;
using Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding;
using Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding.Base;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Levels.Generation.Rendering.Suites.Base;
using Assets.Scripts.Levels.Generation.RoomBuilder;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation.Base
{
    public static class Level
    {
        public static List<RoomData> roomData = new List<RoomData>();

        #region Metadata

        public static bool spawnIsPlaced = false;

        //Scaffolding
        public static List<Node_Door> doors = new List<Node_Door>();
        public static Dictionary<string, Scaffold> roomScaffolds = new Dictionary<string, Scaffold>();
        //public static Dictionary<string, Suite> suiteCollection = new Dictionary<string, Suite>();

        //Parsing
        public static Dictionary<string, Parsing_Node> roomParsings = new Dictionary<string, Parsing_Node>();

        //Generated Level Data
        public static Dictionary<string, LevelRoom> Rooms = new Dictionary<string, LevelRoom>();

        #endregion

        #region Getters

        public static RoomData Data(this Room room)
        {
            return roomData.First(x => x.roomId == room.id);
        }

        public static RoomData GetRoomData(string roomId)
        {
            return roomData.First(x => x.roomId == roomId);
        }

        #endregion
    }

    public class RoomData
    {
        public string roomId;
        public Room room { get { return RoomCollection.rooms[roomId]; } }

        public Scaffold scaffold { get { return Level.roomScaffolds[roomId]; } }

        public Parsing_Node parsing { get { return Level.roomParsings[roomId]; } }

        public RoomData(string id)
        {
            roomId = id;
        }

        public List<Scaffold_Node> UnclaimedNodes()
        {
            return scaffold.GetFlattenedNodes().Where(x => !x.claimed).ToList();
        }

        public void SetNodesClaimed(List<Node> nodes)
        {
            foreach(var node in nodes)
            {
                SetNodeClaimed(node);
            }
        }

        public bool IsNodeClaimed(Node node)
        {
            return scaffold.GetFlattenedNodes().First(x => x.id == node.id).claimed;
        }

        public void SetNodeClaimed(Node node)
        {
            scaffold.SetNodeClaimed(node.id);
        }
    }

    public class LevelRoom
    {
        public string roomId;
        public string regionId;

        public RoomTypeEnum roomType;

        //Physical container for rendering room in scene
        public GameObject renderContainer;

        public void SaveChanges()
        {
            Level.Rooms[roomId] = this;
        }
    }
}
