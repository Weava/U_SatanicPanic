using Assets.Scripts.Generation.Extensions;
using Assets.Scripts.Generation.Painter.Cells.Base;
using Assets.Scripts.Generation.Painter.Rooms.Base;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Generation.Painter.Rooms
{
    public static class RoomBuilder
    {
        public static List<Room> ClaimRooms(this List<Cell> targetCells, ClaimType claimType, RoomOptions options)
        {
            switch(claimType)
            {
                case ClaimType.Greedy:
                    return ClaimRooms_Greedy(targetCells, options);
                default:
                    return new List<Room>();
            }
        }

        public static List<Room> ClaimRooms_Greedy(this List<Cell> targetCells, RoomOptions options)
        {
            var result = new List<Room>();

           
            if(options.outside)
            {
                //TODO 5x5 (Outdoors) sweep
            }

            if(!options.excludeRoomSize.Contains(RoomSize.Room_4_4))
            foreach (var cell in targetCells.Where(x => !x.claimed))
            {
                var room = TryClaimRoom(cell, RoomSize.Room_4_4, options);
                if (room != null) { result.Add(room); }
            }

            if (!options.excludeRoomSize.Contains(RoomSize.Room_3_3))
                foreach (var cell in targetCells.Where(x => !x.claimed))
            {
                if (cell.claimed) continue;
                var room = TryClaimRoom(cell, RoomSize.Room_3_3, options);
                if (room != null) { result.Add(room); }
            }

            if (!options.excludeRoomSize.Contains(RoomSize.Room_2_3))
                foreach (var cell in targetCells.Where(x => !x.claimed))
            {
                if (cell.claimed) continue;
                var room = TryClaimRoom(cell, RoomSize.Room_2_3, options);
                if (room != null) { result.Add(room); }
            }

            if (!options.excludeRoomSize.Contains(RoomSize.Room_2_2))
                foreach (var cell in targetCells.Where(x => !x.claimed))
            {
                if (cell.claimed) continue;
                var room = TryClaimRoom(cell, RoomSize.Room_2_2, options);
                if (room != null) { result.Add(room); }
            }

            if (!options.excludeRoomSize.Contains(RoomSize.Room_1_2))
                foreach (var cell in targetCells.Where(x => !x.claimed))
            {
                if (cell.claimed) continue;
                var room = TryClaimRoom(cell, RoomSize.Room_1_2, options);
                if (room != null) { result.Add(room); }
            }

            foreach (var cell in targetCells.Where(x => !x.claimed))
            {
                if (cell.claimed) continue;
                var room = TryClaimRoom(cell, RoomSize.Room_1_1, options);
                if (room != null) { result.Add(room); }
            }

            RoomCollection.collection.AddRange(result);

            return result;
        }

        private static Room TryClaimRoom(Cell cell, RoomSize roomSize, RoomOptions options)
        {
            foreach (var direction in Directionf.GetDirectionList())
            {
                var projection = Roomf.ProjectRoom(cell.position, direction, roomSize, options);
                if (projection != null)
                    Roomf.ClaimForRoom(projection, roomSize, direction, options);
            }

            return null;
        }
    }

    public enum ClaimType
    {
        Greedy,
    }
}
