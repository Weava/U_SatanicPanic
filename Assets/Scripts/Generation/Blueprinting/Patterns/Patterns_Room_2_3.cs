using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Generation.Blueprinting.Patterns
{
    public static class Patterns_Room_2_3
    {
        public static List<Mask> patterns_Doors = new List<Mask>
        {
            new Mask((int)RoomConfiguration.EndRoom, 0b_0000_0000_0000_0000_0000_1111, 0, MatchCriteria.Fit),
            new Mask((int)RoomConfiguration.EndRoom, 0b_0000_0000_0000_0000_1111_0000, 1, MatchCriteria.Fit),
            new Mask((int)RoomConfiguration.EndRoom, 0b_0000_0000_0000_1111_0000_0000, 2, MatchCriteria.Fit),
            new Mask((int)RoomConfiguration.EndRoom, 0b_0000_0000_1111_0000_0000_0000, 3, MatchCriteria.Fit),

            new Mask((int)RoomConfiguration.SideRoom, 0b_0000_0000_0000_0000_1111_1111, 0, MatchCriteria.Fit, 1),
            new Mask((int)RoomConfiguration.SideRoom, 0b_0000_0000_1111_0000_0000_1111, 0, MatchCriteria.Fit, 1),
            new Mask((int)RoomConfiguration.SideRoom, 0b_0000_0000_0000_1111_1111_0000, 2, MatchCriteria.Fit, 1),
            new Mask((int)RoomConfiguration.SideRoom, 0b_0000_0000_1111_1111_0000_0000, 2, MatchCriteria.Fit, 1),

            new Mask((int)RoomConfiguration.Arena, 0b_0000_0000_0000_1111_0000_1111, 0, MatchCriteria.Fit, 2),
            new Mask((int)RoomConfiguration.Arena, 0b_0000_0000_0001_1101_0010_1111, 0, MatchCriteria.Fit, 2),
            new Mask((int)RoomConfiguration.Arena, 0b_0000_0000_0011_0010_0011_1111, 0, MatchCriteria.Fit, 2),
            new Mask((int)RoomConfiguration.Arena, 0b_0000_0000_0011_1111_0011_0010, 2, MatchCriteria.Fit, 2),
            new Mask((int)RoomConfiguration.Arena, 0b_0000_0000_1111_0000_1111_0000, 1, MatchCriteria.Fit, 2),

            new Mask((int)RoomConfiguration.Courtyard, 0b_0000_0000_0011_1111_0011_1111, 0, MatchCriteria.Fit, 3),
        };
    }
}
