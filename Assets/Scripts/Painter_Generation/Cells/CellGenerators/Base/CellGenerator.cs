using Assets.Scripts.Painter_Generation.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Painter_Generation.Cells.CellGenerators.Base
{
    public abstract class CellGenerator : MonoBehaviour
    {
        #region Markers
        public GameObject Marker;
        public GameObject SpawnMarker;
        public GameObject EndMarker;
        public GameObject PathMarker;
        #region Room Markers
        public GameObject Room_1_1_Marker;
        public GameObject Room_1_2_Marker;
        public GameObject Room_2_2_Marker;
        public GameObject Room_2_3_Marker;
        public GameObject Room_3_3_Marker;
        #endregion
        #endregion

        public LevelMap levelMap;

        public virtual void Generate()
        {
            throw new NotImplementedException("Level generator needs a generation definition.");
        }

        public virtual void Map()
        {
            throw new NotImplementedException("Level generator needs a mapping definition.");
        }

        public void BuildMarker(Vector3 position, CellType markerType)
        {
            switch(markerType)
            {
                case CellType.End_Cell:
                    Instantiate(EndMarker, position, new Quaternion(), transform);
                    return;
                case CellType.Spawn_Cell:
                    Instantiate(SpawnMarker, position, new Quaternion(), transform);
                    return;
                case CellType.Main_Path_Cell:
                    Instantiate(PathMarker, position, new Quaternion(), transform);
                    return;
                case CellType.Cell:
                default:
                    Instantiate(Marker, position, new Quaternion(), transform);
                    return;
            }
        }

        public void BuildRoomMarker(Vector3 position, RoomDimensions roomDimensions, Direction direction)
        {
            switch(roomDimensions)
            {
                case RoomDimensions.Room_3_3:
                    BuildRoomMarkerInstance(Room_3_3_Marker, position, direction);
                    return;
                case RoomDimensions.Room_2_3:
                    BuildRoomMarkerInstance(Room_2_3_Marker, position, direction);
                    return;
                case RoomDimensions.Room_2_2:
                    BuildRoomMarkerInstance(Room_2_2_Marker, position, direction);
                    return;
                case RoomDimensions.Room_1_2:
                    BuildRoomMarkerInstance(Room_1_2_Marker, position, direction);
                    return;
                case RoomDimensions.Room_1_1:
                default:
                    BuildRoomMarkerInstance(Room_1_1_Marker, position, direction);
                    return;
            }
        }

        private void BuildRoomMarkerInstance(GameObject instance, Vector3 position, Direction direction)
        {
            var marker = Instantiate(instance, position, new Quaternion(), transform);
            marker.transform.Rotate(new Vector3(0, direction.RotationAngle(), 0));
        }
    }
}
