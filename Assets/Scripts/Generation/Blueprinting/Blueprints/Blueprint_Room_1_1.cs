using Assets.Scripts.Generation.Blueprinting.BlueprintFactories;
using Assets.Scripts.Generation.Painter.Rooms.Base;

namespace Assets.Scripts.Generation.Blueprinting.Blueprints
{
    public class Blueprint_Room_1_1 : Blueprint
    {
        public Blueprint_Room_1_1(Room room)
        {
            this.room = room;
            doors = BPFactory_Room_1_1.GetDoorMask(room);
            cells = BPFactory_Room_1_1.GetCellMask(room);
            roomConfig = BPFactory_Room_1_1.GetRoomConfiguration(doors);
        }
    }
}
