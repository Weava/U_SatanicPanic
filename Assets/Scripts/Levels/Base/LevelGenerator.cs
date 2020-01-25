using Assets.Scripts.Generation.Painter.Cells.Base;
using Assets.Scripts.Generation.Painter.Rooms.Base;
using Assets.Scripts.Misc;
using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Levels.Base
{
    public abstract class LevelGenerator : MonoBehaviour
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

        public virtual bool BuildLevel()
        {
            throw new NotImplementedException("A level generator instance must be defined to build level.");
        }

        public void CleanUnclaimedCells()
        {
            var cellsToClean = CellCollection.collection.Where(x => !x.Value.claimed).Select(s => s.Key);
            foreach(var cell in cellsToClean.ToList())
            {
                CellCollection.collection.Remove(cell);
            }
        }

        public void RenderMarkers()
        {
            foreach(var cell in CellCollection.collection.Select(s => s.Value))
            {
                var name = cell.Region + " " + cell.Subregion + " ";
                foreach(var tag in cell.tags.ToList())
                {
                    name += tag + " ";
                }

                if(cell.cellType == CellType.Path_Cell)
                {
                    name += cell.pathSequence;
                }

                if (cell.tags.Contains(Tags.INIT_PATH))
                {
                    var instance = Instantiate(InitMarker, cell.position, new Quaternion());
                    instance.name = name;
                } else if (cell.tags.Contains(Tags.CELL_ELEVATION)) {
                    var instance = Instantiate(ElevationMarker, cell.position, new Quaternion());
                    instance.name = name;
                } else
                {
                    var instance = Instantiate(Marker, cell.position, new Quaternion());
                    instance.name = name;
                }
            }
        }

        public void RenderRooms()
        {
            RoomInstance roomInstance;
            foreach (var room in RoomCollection.collection)
            {
                switch(room.roomSize)
                {
                    case RoomSize.Room_1_1:
                        roomInstance = Instantiate(Room_1_1, room.rootPosition, Quaternion.Euler(new Vector3(0, Directionf.RotationAngle(room.orientation), 0)));
                        roomInstance.orientation = room.orientation;
                        roomInstance.root = room.rootPosition;
                        roomInstance.name = room.blueprint == null ? "Room" : room.blueprint.roomConfig.ToString();
                        break;
                    case RoomSize.Room_1_2:
                        roomInstance = Instantiate(Room_1_2, room.rootPosition, Quaternion.Euler(new Vector3(0, Directionf.RotationAngle(room.orientation), 0)));
                        roomInstance.orientation = room.orientation;
                        roomInstance.root = room.rootPosition;
                        roomInstance.name = room.blueprint == null ? "Room" : room.blueprint.roomConfig.ToString();
                        break;
                    case RoomSize.Room_2_2:
                        roomInstance = Instantiate(Room_2_2, room.rootPosition, Quaternion.Euler(new Vector3(0, Directionf.RotationAngle(room.orientation), 0)));
                        roomInstance.orientation = room.orientation;
                        roomInstance.root = room.rootPosition;
                        roomInstance.name = room.blueprint == null ? "Room" : room.blueprint.roomConfig.ToString();
                        break;
                    case RoomSize.Room_2_3:
                        roomInstance = Instantiate(Room_2_3, room.rootPosition, Quaternion.Euler(new Vector3(0, Directionf.RotationAngle(room.orientation), 0)));
                        roomInstance.orientation = room.orientation;
                        roomInstance.root = room.rootPosition;
                        roomInstance.name = room.blueprint == null ? "Room" : room.blueprint.roomConfig.ToString();
                        break;
                    case RoomSize.Room_3_3:
                        roomInstance = Instantiate(Room_3_3, room.rootPosition, Quaternion.Euler(new Vector3(0, Directionf.RotationAngle(room.orientation), 0)));
                        roomInstance.orientation = room.orientation;
                        roomInstance.root = room.rootPosition;
                        roomInstance.name = room.blueprint == null ? "Room" : room.blueprint.roomConfig.ToString();
                        break;
                    case RoomSize.Room_4_4:
                        roomInstance = Instantiate(Room_4_4, room.rootPosition, Quaternion.Euler(new Vector3(0, Directionf.RotationAngle(room.orientation), 0)));
                        roomInstance.orientation = room.orientation;
                        roomInstance.root = room.rootPosition;
                        roomInstance.name = room.blueprint == null ? "Room" : room.blueprint.roomConfig.ToString();
                        break;
                    default:
                        break;
                }
            }
        }

        public void RenderDoors()
        {
            var doors = RoomCollection.collection.SelectMany(s => s.DoorCells).ToList();
            foreach(var door in doors)
            {
                foreach(var connection in door.connections)
                {
                    Instantiate(DoorMarker, ((door.position + connection.connectedCell.position) * 0.5f), new Quaternion());
                }
            }
        }
    }
}
