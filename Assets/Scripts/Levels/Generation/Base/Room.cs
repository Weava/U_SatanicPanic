using System.Collections.Generic;

namespace Assets.Scripts.Levels.Generation.Base
{
    public class Room
    {
        public List<Cell> cells = new List<Cell>();
        public List<Room> connectedRooms = new List<Room>();
    }

    public static class RoomCollection
    {
        public static List<Room> rooms = new List<Room>();
    }
}
