using System.Collections.Generic;
using Assets.Scripts.Levels.Generation.Extensions;
using System.Linq;

namespace Assets.Scripts.Levels.Generation.Base
{
    public class Room
    {
        public List<Cell> cells = new List<Cell>();

        public List<Room> neighborRooms { get { return this.NeighborRooms(); } }

        public List<Room> connectedRooms { get { return this.ConnectedRooms(); } }

        public bool containsPath { get { return cells.Any(x => x.important); } }

        public bool pathConfirmedOverride = false;

        public bool preventExtraConnections = false;

        #region Parsing Properties

        public List<Cell> potentialDoors = new List<Cell>();

        #endregion
    }

    public static class RoomCollection
    {
        public static List<Room> rooms = new List<Room>();
    }
}
