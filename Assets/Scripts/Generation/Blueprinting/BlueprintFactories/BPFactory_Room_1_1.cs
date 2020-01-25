﻿using Assets.Scripts.Generation.Blueprinting.Blueprints;
using Assets.Scripts.Generation.Blueprinting.Patterns;
using Assets.Scripts.Generation.Extensions;
using Assets.Scripts.Generation.Painter.Rooms.Base;
using System.Linq;

namespace Assets.Scripts.Generation.Blueprinting.BlueprintFactories
{
    public static class BPFactory_Room_1_1
    {
        public static Mask GetDoorMask(this Room room)
        {
            var result = new Mask();
            result.configuration = (int)MetaConfiguration.DoorMask;

            var root = room.cells[room.rootPosition];
            var direction = room.orientation;

            if (root.connections.Any(x => x.connectedCell.position == root.Step(direction)))
            { result.Add(direction, direction, 0001); }

            if (root.connections.Any(x => x.connectedCell.position == root.Step(direction.GetRightDirection())))
            { result.Add(direction, direction.GetRightDirection(), 0001); }

            if (root.connections.Any(x => x.connectedCell.position == root.Step(direction.GetOppositeDirection())))
            { result.Add(direction, direction.GetOppositeDirection(), 0001); }

            if (root.connections.Any(x => x.connectedCell.position == root.Step(direction.GetLeftDirection())))
            { result.Add(direction, direction.GetLeftDirection(), 0001); }

            return result;
        }

        public static Mask GetCellMask(this Room room)
        {
            return new Mask((int)MetaConfiguration.CellMask, 0b_0000_0000_0000_0000_0000_0001);
        }

        public static RoomConfiguration GetRoomConfiguration(Mask doorMask)
        {
            var match = doorMask.FindMatchs(Patterns_Room_1_1.patterns_Doors).OrderByDescending(o => o.precidence).FirstOrDefault();

            if(match != null)
            { return (RoomConfiguration)match.configuration; }

            return RoomConfiguration.Room;
        }
    }
}
