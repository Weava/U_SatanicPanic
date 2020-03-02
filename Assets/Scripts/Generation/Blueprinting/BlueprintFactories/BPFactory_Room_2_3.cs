using Assets.Scripts.Generation.Blueprinting.Patterns;
using Assets.Scripts.Generation.Painter.Rooms.Base;
using System.Linq;

namespace Assets.Scripts.Generation.Blueprinting.BlueprintFactories
{
    public static class BPFactory_Room_2_3
    {
        public static Mask GetDoorMask(this Room room)
        {
            var result = new Mask();
            result.configuration = (int)MetaConfiguration.DoorMask;

            var cells = room.cells.Select(s => s.Value).ToList();
            var direction = room.orientation;

            // X-     00 03
            // RX -> [01]04 ->
            // X-     02 05

            //D0
            var direction_0_connections = cells.SelectMany(s => s.connections.Where(x => x.normal == direction));
            if (direction_0_connections.Any())
            {
                if (direction_0_connections.Any(x => cells[3].connections.Contains(x)))
                    result.Add(direction, direction, 0b_0001);
                if (direction_0_connections.Any(x => cells[4].connections.Contains(x)))
                    result.Add(direction, direction, 0b_0010);
                if (direction_0_connections.Any(x => cells[5].connections.Contains(x)))
                    result.Add(direction, direction, 0b_0100);
            }

            //D1
            var direction_1_connections = cells.SelectMany(s => s.connections.Where(x => x.normal == direction.GetRightDirection()));
            if (direction_1_connections.Any())
            {
                if (direction_1_connections.Any(x => cells[5].connections.Contains(x)))
                    result.Add(direction, direction.GetRightDirection(), 0b_0001);
                if (direction_1_connections.Any(x => cells[2].connections.Contains(x)))
                    result.Add(direction, direction.GetRightDirection(), 0b_0010);
            }

            //D2
            var direction_2_connections = cells.SelectMany(s => s.connections.Where(x => x.normal == direction.GetOppositeDirection()));
            if (direction_2_connections.Any())
            {
                if (direction_2_connections.Any(x => cells[2].connections.Contains(x)))
                    result.Add(direction, direction.GetOppositeDirection(), 0b_0001);
                if (direction_2_connections.Any(x => cells[1].connections.Contains(x)))
                    result.Add(direction, direction.GetOppositeDirection(), 0b_0010);
                if (direction_2_connections.Any(x => cells[0].connections.Contains(x)))
                    result.Add(direction, direction.GetOppositeDirection(), 0b_0100);
            }

            //D3
            var direction_3_connections = cells.SelectMany(s => s.connections.Where(x => x.normal == direction.GetLeftDirection()));
            if (direction_3_connections.Any())
            {
                if (direction_3_connections.Any(x => cells[0].connections.Contains(x)))
                    result.Add(direction, direction.GetLeftDirection(), 0b_0001);
                if (direction_3_connections.Any(x => cells[3].connections.Contains(x)))
                    result.Add(direction, direction.GetLeftDirection(), 0b_0010);
            }

            return result;
        }

        public static Mask GetCellMask(this Room room)
        {
            return new Mask((int)MetaConfiguration.CellMask, 0b_0000_0000_0000_0000_0011_0111);
        }

        public static RoomConfiguration GetRoomConfiguration(Mask doorMask)
        {
            return BPFactory_Rooms.GetRoomConfiguration(doorMask, Patterns_Room_2_3.patterns_Doors);
        }
    }
}
