using Assets.Scripts.Generation.Painter.Rooms.Base;
using System.Collections.Generic;

namespace Assets.Scripts.Generation.Blueprinting
{
    /// <summary>
    /// Metadata for specifying configurations for the Room Factories
    /// </summary>
    public abstract class Blueprint
    {
        public RoomConfiguration roomConfig;

        public Mask doors;
        public Mask cells;

        public Room room;
    }

    public enum MetaConfiguration
    {
        DoorMask,
        CellMask
    }

    public enum RoomConfiguration
    {
        Room,
        EndRoom,
        SideRoom,
        Connector,
        Courtyard,
        Arena,
    }
}
