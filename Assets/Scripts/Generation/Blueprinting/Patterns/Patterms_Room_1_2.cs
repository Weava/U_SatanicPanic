using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Generation.Blueprinting.Patterns
{
    public static class Patterms_Room_1_2
    {
        public static List<Mask> patterns_Doors = new List<Mask>
        {
            //Door on either of the short walls
            new Mask((int)RoomConfiguration.EndRoom, 0b_0000_0000_0000_0000_0000_0001, 0, MatchCriteria.Exact),
            new Mask((int)RoomConfiguration.EndRoom, 0b_0000_0000_0000_0001_0000_0000, 2, MatchCriteria.Exact),

            //Only door on either long end of room
            new Mask((int)RoomConfiguration.SideRoom, 0b_0000_0000_0000_0000_0001_0000, 1, MatchCriteria.Exact),
            new Mask((int)RoomConfiguration.SideRoom, 0b_0000_0000_0000_0000_0010_0000, 1, MatchCriteria.Exact),
            new Mask((int)RoomConfiguration.SideRoom, 0b_0000_0000_0001_0000_0000_0000, 3, MatchCriteria.Exact),
            new Mask((int)RoomConfiguration.SideRoom, 0b_0000_0000_0010_0000_0000_0000, 3, MatchCriteria.Exact),

            //Straight across
            new Mask((int)RoomConfiguration.Connector, 0b_0000_0000_0000_0001_0000_0001, 0),
        };
    }
}
