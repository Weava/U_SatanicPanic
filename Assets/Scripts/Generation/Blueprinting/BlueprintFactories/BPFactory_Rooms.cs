using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Generation.Blueprinting.BlueprintFactories
{
    public static class BPFactory_Rooms
    {
        public static RoomConfiguration GetRoomConfiguration(Mask doorMask, List<Mask> doorPatterns)
        {
            var match = doorMask.FindMatchs(doorPatterns).OrderBy(o => o.precidence).FirstOrDefault();

            if (match != null)
            { return (RoomConfiguration)match.configuration; }

            return RoomConfiguration.Room;
        }

        internal static RoomConfiguration GetRoomConfiguration(Mask doorMask, object patterns_Doors)
        {
            throw new NotImplementedException();
        }
    }
}
