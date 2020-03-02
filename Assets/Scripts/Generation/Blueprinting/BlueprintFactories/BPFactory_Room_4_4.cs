using Assets.Scripts.Generation.Blueprinting.Patterns;
using Assets.Scripts.Generation.Painter.Rooms.Base;
using System.Linq;

namespace Assets.Scripts.Generation.Blueprinting.BlueprintFactories
{
    public static class BPFactory_Room_4_4
    {
        public static Mask GetDoorMask(this Room room)
        {
            var result = new Mask();
            result.configuration = (int)MetaConfiguration.DoorMask;

            var cells = room.cells.Select(s => s.Value).ToList();
            var direction = room.orientation;

            // -XX-    00 04 08 12
            // XRXX -> 01[05]09 13 ->
            // XXXX    02 06 10 14
            // -XX-    03 07 11 15

            //D0
            var direction_0_connections = cells.SelectMany(s => s.connections.Where(x => x.normal == direction));
            if (direction_0_connections.Any())
            {
                if (direction_0_connections.Any(x => cells[12].connections.Contains(x)))
                    result.Add(direction, direction, 0b_0001);
                if (direction_0_connections.Any(x => cells[13].connections.Contains(x)))
                    result.Add(direction, direction, 0b_0010);
                if (direction_0_connections.Any(x => cells[14].connections.Contains(x)))
                    result.Add(direction, direction, 0b_0100);
                if (direction_0_connections.Any(x => cells[15].connections.Contains(x)))
                    result.Add(direction, direction, 0b_1000);
            }

            //D1
            var direction_1_connections = cells.SelectMany(s => s.connections.Where(x => x.normal == direction.GetRightDirection()));
            if (direction_1_connections.Any())
            {
                if (direction_1_connections.Any(x => cells[15].connections.Contains(x)))
                    result.Add(direction, direction.GetRightDirection(), 0b_0001);
                if (direction_1_connections.Any(x => cells[11].connections.Contains(x)))
                    result.Add(direction, direction.GetRightDirection(), 0b_0010);
                if (direction_1_connections.Any(x => cells[7].connections.Contains(x)))
                    result.Add(direction, direction.GetRightDirection(), 0b_0100);
                if (direction_1_connections.Any(x => cells[3].connections.Contains(x)))
                    result.Add(direction, direction.GetRightDirection(), 0b_1000);
            }

            //D2
            var direction_2_connections = cells.SelectMany(s => s.connections.Where(x => x.normal == direction.GetOppositeDirection()));
            if (direction_2_connections.Any())
            {
                if (direction_2_connections.Any(x => cells[3].connections.Contains(x)))
                    result.Add(direction, direction.GetOppositeDirection(), 0b_0001);
                if (direction_2_connections.Any(x => cells[2].connections.Contains(x)))
                    result.Add(direction, direction.GetOppositeDirection(), 0b_0010);
                if (direction_2_connections.Any(x => cells[1].connections.Contains(x)))
                    result.Add(direction, direction.GetOppositeDirection(), 0b_0100);
                if (direction_2_connections.Any(x => cells[0].connections.Contains(x)))
                    result.Add(direction, direction.GetOppositeDirection(), 0b_1000);
            }

            //D3
            var direction_3_connections = cells.SelectMany(s => s.connections.Where(x => x.normal == direction.GetLeftDirection()));
            if (direction_3_connections.Any())
            {
                if (direction_3_connections.Any(x => cells[0].connections.Contains(x)))
                    result.Add(direction, direction.GetLeftDirection(), 0b_0001);
                if (direction_3_connections.Any(x => cells[4].connections.Contains(x)))
                    result.Add(direction, direction.GetLeftDirection(), 0b_0010);
                if (direction_3_connections.Any(x => cells[8].connections.Contains(x)))
                    result.Add(direction, direction.GetLeftDirection(), 0b_0100);
                if (direction_3_connections.Any(x => cells[12].connections.Contains(x)))
                    result.Add(direction, direction.GetLeftDirection(), 0b_1000);
            }

            return result;
        }

        public static Mask GetCellMask(this Room room)
        {
            return new Mask((int)MetaConfiguration.CellMask, 0b_0000_0000_0000_0000_1111_1111);
        }

        public static RoomConfiguration GetRoomConfiguration(Mask doorMask)
        {
            return BPFactory_Rooms.GetRoomConfiguration(doorMask, Patterns_Room_4_4.patterns_Doors);
        }
    }
}
