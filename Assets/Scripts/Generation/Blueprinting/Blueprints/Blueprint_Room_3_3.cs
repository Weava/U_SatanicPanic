using Assets.Scripts.Generation.Blueprinting.BlueprintFactories;
using Assets.Scripts.Generation.Painter.Rooms.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Generation.Blueprinting.Blueprints
{
    public class Blueprint_Room_3_3 : Blueprint
    {
        public Blueprint_Room_3_3(Room room)
        {
            this.room = room;
            doors = BPFactory_Room_3_3.GetDoorMask(room);
            cells = BPFactory_Room_3_3.GetCellMask(room);
            roomConfig = BPFactory_Room_3_3.GetRoomConfiguration(doors);
        }
    }
}