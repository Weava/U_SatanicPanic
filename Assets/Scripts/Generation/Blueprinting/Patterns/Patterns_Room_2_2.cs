using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Generation.Blueprinting.Patterns
{
    public static class Patterns_Room_2_2
    {
        public static List<Mask> patterns_Doors = new List<Mask>
        {
            new Mask((int)RoomConfiguration.EndRoom, 0b_0000_0000_1111_1111_1111_0000, 0, MatchCriteria.Exclusive),
            new Mask((int)RoomConfiguration.EndRoom, 0b_0000_0000_1111_1111_0000_1111, 1, MatchCriteria.Exclusive),
            new Mask((int)RoomConfiguration.EndRoom, 0b_0000_0000_1111_0000_1111_1111, 2, MatchCriteria.Exclusive),
            new Mask((int)RoomConfiguration.EndRoom, 0b_0000_0000_0000_1111_1111_1111, 3, MatchCriteria.Exclusive),

            new Mask((int)RoomConfiguration.SideRoom, 0b_0000_0000_0000_0000_1111_1111, 0, MatchCriteria.Fit, 1),
            new Mask((int)RoomConfiguration.SideRoom, 0b_0000_0000_1111_0000_0000_1111, 0, MatchCriteria.Fit, 1),
            new Mask((int)RoomConfiguration.SideRoom, 0b_0000_0000_0000_1111_1111_0000, 2, MatchCriteria.Fit, 1),
            new Mask((int)RoomConfiguration.SideRoom, 0b_0000_0000_1111_1111_0000_0000, 2, MatchCriteria.Fit, 1),
        };
    }
}
