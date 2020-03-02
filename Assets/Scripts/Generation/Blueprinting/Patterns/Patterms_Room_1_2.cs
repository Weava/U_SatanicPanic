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
            new Mask((int)RoomConfiguration.EndRoom, 0b_0000_0000_0000_0000_0000_0001, 0, MatchCriteria.Exact),
            new Mask((int)RoomConfiguration.EndRoom, 0b_0000_0000_0000_0001_0000_0000, 2, MatchCriteria.Exact),

            new Mask((int)RoomConfiguration.SideRoom, 0b_0000_0000_0010_0000_0001_0001, 0, MatchCriteria.Fit),
            new Mask((int)RoomConfiguration.SideRoom, 0b_0000_0000_0001_0001_0010_0000, 2, MatchCriteria.Fit),

            new Mask((int)RoomConfiguration.Connector, 0b_0000_0000_0011_0001_0011_0001, 0, MatchCriteria.Fit, 1),
        };
    }
}
