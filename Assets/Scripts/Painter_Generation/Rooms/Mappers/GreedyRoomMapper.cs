using Assets.Scripts.Generation.Painter.Cells.Base;
using Assets.Scripts.Painter_Generation.Rooms.Mappers.Base;
using System.Linq;

namespace Assets.Scripts.Painter_Generation.Rooms.Mappers
{
    /// <summary>
    /// Asigns the largest rooms possible in a region
    /// </summary>
    public class GreedyRoomMapper : RoomMapper
    {
        public override void Map()
        {
            Reset();
            foreach (var region in map.regions.ToList())
            {
                while(region.region.cells.collection.Any(x => !x.Value.claimed && x.Value.cellType != CellType.Dead_Cell))
                {
                    if(!room_3_3_Complete)
                    {
                        var roomClaim = region.region.FindAvailableCellForRoomDimensions(RoomDimensions.Room_3_3);
                        ClaimRoom(roomClaim, region, ref room_3_3_Complete);
                    }
                    else if(!room_2_3_Complete)
                    {
                        var roomClaim = region.region.FindAvailableCellForRoomDimensions(RoomDimensions.Room_2_3);
                        ClaimRoom(roomClaim, region, ref room_2_3_Complete);
                    }else if (!room_2_2_Complete)
                    {
                        var roomClaim = region.region.FindAvailableCellForRoomDimensions(RoomDimensions.Room_2_2);
                        ClaimRoom(roomClaim, region, ref room_2_2_Complete);
                    } else if (!room_1_2_Complete)
                    {
                        var roomClaim = region.region.FindAvailableCellForRoomDimensions(RoomDimensions.Room_1_2);
                        ClaimRoom(roomClaim, region, ref room_1_2_Complete);
                    } else
                    {
                        var roomClaim = region.region.FindAvailableCellForRoomDimensions(RoomDimensions.Room_1_1);
                        var room = new Room(RoomType.Room, RoomDimensions.Room_1_1, roomClaim);
                        region.rooms.Add(room);
                    }
                }
            }
        }
    }
}
