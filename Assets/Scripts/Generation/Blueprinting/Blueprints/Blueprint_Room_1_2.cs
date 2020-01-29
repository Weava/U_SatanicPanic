using Assets.Scripts.Generation.Blueprinting.BlueprintFactories;
using Assets.Scripts.Generation.Painter.Rooms.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Generation.Blueprinting.Blueprints
{
    public class Blueprint_Room_1_2 : Blueprint
    {
        public Blueprint_Room_1_2(Room room)
        {
            this.room = room;
            doors = BPFactory_Room_1_2.GetDoorMask(room);
            cells = BPFactory_Room_1_2.GetCellMask(room);
            roomConfig = BPFactory_Room_1_2.GetRoomConfiguration(doors);
        }
    }
}
