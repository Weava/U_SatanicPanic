using Assets.Scripts.Generation.Painter.Cells.Base;
using Assets.Scripts.Painter_Generation.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Painter_Generation.Rooms
{
    public class Room
    {
        public Direction roomDirection;
        public RoomDimensions roomDimensions;
        public RoomType roomType;
        public Cell rootCell;
        public List<Cell> cells = new List<Cell>();

        public Room(RoomType type, RoomDimensions dimensions, RoomClaim roomClaim)
        {
            roomDirection = roomClaim.direction;
            roomDimensions = dimensions;
            roomType = type;
            rootCell = roomClaim.rootCell;
            cells = roomClaim.incompassedCells;
            foreach(var cell in cells)
            {
                cell.claimed = true;
            }
        }
    }

    public enum RoomType
    {
        Room,
        Hallway,
        Arena
    }

    public enum RoomDimensions
    {
        Room_1_1,
        Room_1_2,
        Room_2_2,
        Room_2_3,
        Room_3_3,
        Room_3_4,
        Room_4_4,
        Room_5_5
    }
}
