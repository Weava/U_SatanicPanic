using System.Collections.Generic;

namespace Assets.Scripts.Generation.Blueprinting.Patterns
{
    public static class Patterns_Room_1_1
    {
        public static List<Mask> patterns_Doors = new List<Mask>
        {
            new Mask((int)RoomConfiguration.EndRoom, 0b_0000_0000_0000_0000_0000_0001, 0, MatchCriteria.Exact),
            new Mask((int)RoomConfiguration.EndRoom, 0b_0000_0000_0000_0000_0001_0000, 1, MatchCriteria.Exact),
            new Mask((int)RoomConfiguration.EndRoom, 0b_0000_0000_0000_0001_0000_0000, 2, MatchCriteria.Exact),
            new Mask((int)RoomConfiguration.EndRoom, 0b_0000_0000_0001_0000_0000_0000, 3, MatchCriteria.Exact),

            //Straight across
            new Mask((int)RoomConfiguration.Connector, 0b_0000_0000_0001_0000_0001_0000, 0, MatchCriteria.Exact),
            new Mask((int)RoomConfiguration.Connector, 0b_0000_0000_0000_0001_0000_0001, 1, MatchCriteria.Exact),

            new Mask((int)RoomConfiguration.Connector, 0b_0000_0000_0000_0000_0001_0001, 0, MatchCriteria.Exact),
            new Mask((int)RoomConfiguration.Connector, 0b_0000_0000_0000_0001_0001_0000, 1, MatchCriteria.Exact),
            new Mask((int)RoomConfiguration.Connector, 0b_0000_0000_0001_0001_0000_0000, 2, MatchCriteria.Exact),
            new Mask((int)RoomConfiguration.Connector, 0b_0000_0000_0001_0000_0000_0001, 3, MatchCriteria.Exact),
        };  
    }
}
