using Assets.Scripts.Levels.Generation.RoomBuilder.Nodes;
using Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Parsing.Base;
using Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding;
using Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding.Base;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Levels.Generation.Base
{
    public static class Level
    {
        public static List<RoomData> roomData = new List<RoomData>();

        #region Metadata

        //Scaffolding
        public static List<Node_Door> doors = new List<Node_Door>();
        public static Dictionary<string, Scaffold> roomScaffolds = new Dictionary<string, Scaffold>();

        //Parsing
        public static Dictionary<string, Parsing_Node> roomParsings = new Dictionary<string, Parsing_Node>();

        ////Placements
        //public static Dictionary<string, Placement> roomPlacement = new Dictionary<string, Placement>();

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

        public List<Node> UnclaimedNodes()
        {
            return scaffold.GetNodes().Where(x => !x.claimed).ToList();
        }

        public void SetNodesClaimed(List<Node> nodes)
        {
            foreach(var node in nodes)
            {
                SetNodeClaimed(node);
            }
        }

        public void SetNodeClaimed(Node node)
        {
            scaffold.SetNodeClaimed(node.id);
        }
    }
}
