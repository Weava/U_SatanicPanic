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

            foreach(var door in room.doors.ToArray())
            {
                var otherRoom = door.cell_1.room == room ? door.cell_2.room : door.cell_1.room;
                result.Add(otherRoom);
            }

            return result;

            //var result = new List<Room>();

            //foreach(var doorNode in Level.doors.Where(x => room.cells.Contains(x.cell_1) || room.cells.Contains(x.cell_2)))
            //{
            //    Room otherRoom;

            //    if(room.cells.Contains(doorNode.cell_1))
            //    {
            //        otherRoom = doorNode.cell_2.room;
            //    } else
            //    {
            //        otherRoom = doorNode.cell_1.room;
            //    }

            //    if(!result.Contains(otherRoom))
            //    {
            //        result.Add(otherRoom);
            //    }
            //}

            //return result;
        }

        public static List<Room> SearchForPathRoom(this Room room, bool containInRegion)
        {
            var searchedRooms = new List<Room>();

            return SearchForPathRoom_R(room, searchedRooms, containInRegion);
        }

        private static List<Room> SearchForPathRoom_R(Room room, List<Room> discoveredRooms, bool containInRegion)
        {
            //Found a path
            if(room.containsPath)
            {
                if(room.preventExtraConnections)
                { return new List<Room>(); }
                return new List<Room>() { room };
            }

            //Already saw this room, abort
            if(discoveredRooms.Contains(room))
            {  return new List<Room>(); }

            discoveredRooms.Add(room);

            var result = new List<Room>();

            foreach(var potentialRoom in room.potentialDoors.Select(s => s.room))
            {
                if (containInRegion && potentialRoom.cells.First().region != room.cells.First().region)
                { continue; }
                result.AddRange(SearchForPathRoom_R(potentialRoom, discoveredRooms, containInRegion));
                if(result.Count > 0) { break; }
            }

            return result;
        }
    }
}
