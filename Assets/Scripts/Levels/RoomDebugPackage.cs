using Assets.Scripts.Generation.Painter.Cells.Base;
using Assets.Scripts.Generation.Painter.Rooms.Base;
using Assets.Scripts.Levels.Base;
using Assets.Scripts.Misc;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Levels
{
    public class RoomDebugPackage : MonoBehaviour
    {
        public GameObject Marker;
        public GameObject InitMarker;
        public GameObject ElevationMarker;
        public GameObject DoorMarker;

        public RoomInstance Room_1_1;
        public RoomInstance Room_1_2;
        public RoomInstance Room_2_2;
        public RoomInstance Room_2_3;
        public RoomInstance Room_3_3;
        public RoomInstance Room_4_4;

        public void RenderMarkers()
        {
            var container = new GameObject("Cells");

            foreach (var cell in CellCollection.collection.Select(s => s.Value))
            {
                var name = cell.Region + " " + cell.Subregion + " ";
                foreach (var tag in cell.tags.ToList())
                {
                    name += tag + " ";
                }

                if (cell.cellType == CellType.Path_Cell)
                {
                    name += cell.pathSequence;
                }

                if (cell.tags.Contains(Tags.INIT_PATH))
                {
                    var instance = Instantiate(InitMarker, cell.position, new Quaternion());
                    instance.name = name;
                    instance.transform.SetParent(container.transform);
                }
                else if (cell.tags.Contains(Tags.CELL_ELEVATION))
                {
                    var instance = Instantiate(ElevationMarker, cell.position, new Quaternion());
                    instance.name = name;
                    instance.transform.SetParent(container.transform);
                }
                else
                {
                    var instance = Instantiate(Marker, cell.position, new Quaternion());
                    instance.name = name;
                    instance.transform.SetParent(container.transform);
                }
            }
        }

        public void RenderRoomTemplates()
        {
            var container = new GameObject("Room Templates");

            RoomInstance roomInstance;
            foreach (var room in RoomCollection.collection)
            {
                switch (room.roomSize)
                {
                    case RoomSize.Room_1_1:
                        InitRoom(Room_1_1, room, container);
                        break;
                    case RoomSize.Room_1_2:
                        InitRoom(Room_1_2, room, container);
                        break;
                    case RoomSize.Room_2_2:
                        InitRoom(Room_2_2, room, container);
                        break;
                    case RoomSize.Room_2_3:
                        InitRoom(Room_2_3, room, container);
                        break;
                    case RoomSize.Room_3_3:
                        InitRoom(Room_3_3, room, container);
                        break;
                    case RoomSize.Room_4_4:
                        InitRoom(Room_4_4, room, container);
                        break;
                    default:
                        break;
                }
            }
        }

        private void InitRoom(RoomInstance instance, Room room, GameObject container)
        {
            var roomInstance = Instantiate(instance, room.rootPosition, Quaternion.Euler(new Vector3(0, Directionf.RotationAngle(room.orientation), 0)));
            roomInstance.orientation = room.orientation;
            roomInstance.root = room.rootPosition;
            roomInstance.name = room.blueprint == null ? "Room" : room.blueprint.roomConfig.ToString() + "-" + room.blueprint.doors.mask.ToString();
            if (room.blueprint != null)
                roomInstance.name += room.blueprint.doors.bias == Scripts.Generation.Blueprinting.OffsetBias.None ? "" : room.blueprint.doors.bias.ToString();
            roomInstance.transform.SetParent(container.transform);
        }

        public void RenderDoors()
        {
            var container = new GameObject("Door Markers");

            var doors = RoomCollection.collection.SelectMany(s => s.DoorCells).ToList();
            foreach (var door in doors)
            {
                foreach (var connection in door.connections)
                {
                    var doorInstance = Instantiate(DoorMarker, ((door.position + connection.connectedCell.position) * 0.5f), new Quaternion());
                    doorInstance.transform.SetParent(container.transform);
                }
            }
        }
    }
}
