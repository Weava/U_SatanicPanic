using System.Collections.Generic;
using Assets.Scripts.Levels.Generation.Extensions;
using System.Linq;
using System;
using Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding;

namespace Assets.Scripts.Levels.Generation.Base
{
    public class Room
    {
        public Room()
        {
            id = Guid.NewGuid().ToString();
        }

        public string id = Guid.NewGuid().ToString();

        public string regionId = "";
        //public List<Vector3> cells = new List<Vector3>();

        public List<Room> neighborRooms { get { return this.NeighborRooms(); } }

        public List<Room> connectedRooms { get { return this.ConnectedRooms(); } }

        public bool containsPath { get { return CellCollection.GetByRoom(id).Any(x => x.important); } }

        public bool pathConfirmedOverride = false;

        public bool preventExtraConnections = false;

        #region Parsing Properties

        //public List<Cell> potentialDoors = new List<Cell>();

        public List<Node_Door> doors = new List<Node_Door>();

        #endregion
    }

    public static class RoomCollection
    {
        public static Dictionary<string, Room> rooms = new Dictionary<string, Room>();

        public static List<Room> GetAll()
        {
            return rooms.Select(s => s.Value).ToList();
        }

        public static List<Cell> GetCells(this Room room)
        {
            return CellCollection.cells.Where(x => x.Value.roomId == room.id).Select(s => s.Value).ToList();
        }

        public static bool Add(Room room)
        {
            if (!rooms.ContainsKey(room.id))
            {
                rooms.Add(room.id, room);
                Level.roomData.Add(new RoomData(room.id));
                return true;
            }

            return false;
        }

        public static bool Update(this Room room)
        {
            rooms[room.id] = room;
            return true;
        }
    }
}
