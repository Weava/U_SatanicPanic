using Assets.Scripts.Levels.Generation.Base;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Levels.Generation.Extensions
{
    public static class Roomf
    {
        public static List<Room> NeighborRooms(this Room room)
        {
            var result = new List<Room>();

            foreach(var cell in room.cells)
            {
                var neighbors = cell.NeighborCellsOutOfRoom();
                foreach(var neighborRoom in neighbors.Select(s => s.room))
                {
                    if(!result.Contains(neighborRoom))
                    {
                        result.Add(neighborRoom);
                    }
                }
            }

            return result;
        }

        public static List<Room> ConnectedRooms(this Room room)
        {
            var result = new List<Room>();

            foreach(var doorNode in Level.doors.Where(x => room.cells.Contains(x.cell_1) || room.cells.Contains(x.cell_2)))
            {
                Room otherRoom;

                if(room.cells.Contains(doorNode.cell_1))
                {
                    otherRoom = doorNode.cell_2.room;
                } else
                {
                    otherRoom = doorNode.cell_1.room;
                }

                if(!result.Contains(otherRoom))
                {
                    result.Add(otherRoom);
                }
            }

            return result;
        }

        public static List<Room> SearchForPathRoom(this Room room)
        {
            var searchedRooms = new List<Room>();

            return SearchForPathRoom_R(room, searchedRooms);
        }

        public static bool RoomHasConnectionToPath(this Room room)
        {
            var path = room.SearchForPathRoom();

            if (!path.Any()) return false;

            return false; //TODO: Make this work
        }

        private static List<Room> SearchForPathRoom_R(Room room, List<Room> searchedRooms)
        {
            if (room.containsPath) { searchedRooms.Add(room); return searchedRooms; }
            if (searchedRooms.Contains(room)) { return new List<Room>(); }

            searchedRooms.Add(room);

            var result = new List<Room>();

            foreach(var neighbor in room.neighborRooms)
            {
                result = SearchForPathRoom_R(neighbor, searchedRooms);
                if(result.Any())
                {
                    return result;
                }
            }

            return new List<Room>();
        }

        private static bool RoomHasConnectionToPath_R(Room room, List<Room> searchedRooms)
        {
            return false;
        }
    }
}
