using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Level_Generation
{
    public class Room : MonoBehaviour
    {
        #region Metadata
        #region Level

        public int id { get { return gameObject.GetInstanceID(); } }
        public int rootDistance = 0;
        public RoomTheme theme;
        public RoomType roomType;

        public LevelSchema parentSchema;

        #endregion
        #region Generation
        public List<Door> doors;
        public Room parentRoom;
        public List<Room> childrenRooms = new List<Room>();
        public List<Room> conflictRooms = new List<Room>();
        public bool inConflict { get { return conflictRooms.Any(); } }
        public bool ready = false;
        #endregion
        #endregion

        #region Interface

        public Room SpawnChildRoom(Door rootDoor, Room childRoomPrefab, int childIndex)
        {
            return SpawnChildRoom(rootDoor.direction, rootDoor.index, childRoomPrefab, childIndex);
        }

        public Room SpawnChildRoom(Direction direction, int index, Room childRoomPrefab, int childIndex)
        {
            var door = GetDoor(direction, index);
            var childRoom = Instantiate(childRoomPrefab, door.transform.position, new Quaternion());
            var offset = GetRoomOffsetToDoor(door, childRoom, childIndex);
            var childDoor = childRoom.GetDoor(Directionf.GetOppositeDirection(direction), childIndex);
            childDoor.connectedRoom = this;
            door.connectedRoom = childRoom;
            childRoom.transform.position = offset;
            childrenRooms.Add(childRoom);
            childRoom.parentRoom = this;
            childRoom.rootDistance = rootDistance+1;
            childRoom.parentSchema = this.parentSchema;
            return childRoom;
        }

        public void RemoveRoom(Room room)
        {
            var connectDoors = doors.Where(x => x.connectedRoom != null && x.connectedRoom.id == room.id).ToList();
            connectDoors.ForEach(x => { x.connectedRoom = null;});
            childrenRooms.Remove(room);
            if (conflictRooms.Contains(room)) conflictRooms.Remove(room);
            Destroy(room.gameObject);
        }

        public Door GetDoor(Direction direction, int index)
        {
            try
            { return doors.First(x => x.direction == direction && x.index == index);
            } catch (Exception)
            { return null; }
        }

        public List<Door> GetDoors()
        {
            return doors;
        }

        public List<Door> GetAvailableDoors()
        {
            var result = new List<Door>();

            result = doors.Where(x => !x.occupied).ToList();

            return result;
        }

        public List<Door> GetAvailableDoors(Direction direction, bool mustBeAvailable = true)
        {
            if (mustBeAvailable)
                return doors.Where(x => x.direction == direction && !x.occupied).ToList();
            else
                return doors.Where(x => x.direction == direction).ToList();
        }

        public Door GetRandomAvailableDoor(List<Direction> directionMask, List<Door> doorMask)
        {
            if (directionMask == null || directionMask.Count == 0)
            { directionMask = Directionf.GetDirectionList(); }

            var availableDoors = new List<Door>();

            if (doorMask == null || doorMask.Count == 0)
                availableDoors = doors.Where(x => directionMask.Contains(x.direction) && !x.occupied).ToList();
            else
                availableDoors = doors.Where(x => doorMask.Contains(x) && directionMask.Contains(x.direction) && !x.occupied).ToList();

            if (availableDoors.Count == 0) return null;

            return availableDoors[Random.Range(0, availableDoors.Count())];
        }

        public bool IsEndRoom()
        {
            return !childrenRooms.Any();
        }

        public void Remove()
        {
            parentRoom.RemoveRoom(this);
        }

        #endregion

        private Vector3 GetRoomOffsetToDoor(Door door, Room childRoom, int childIndex)
        {
            var childDoor = childRoom.GetDoor(Directionf.GetOppositeDirection(door.direction), childIndex);
            return door.transform.position - childDoor.transform.localPosition;
        }

        #region Mono Methods
        private void LateUpdate()
        {
            ready = true;
        }
        private void Awake()
        {
            doors.ForEach(x => x.rootRoom = this);
        }
        private void OnTriggerEnter(Collider other)
        {
            if(other.transform.tag == "Room")
            {
                var room = other.transform.gameObject.GetComponent<Room>();
                if(!conflictRooms.Contains(room))
                    conflictRooms.Add(room);
            }
        }
        #endregion
    }

    public enum RoomSize
    {
        Small_1x1,
        Small_1x2,
        Medium_2x2,
        Medium_2x3,
        Large_3x3,
        ExtraLarge_4x4,
        XXLarge_5x5
    }

    public enum RoomType
    {
        Hallway,
        Room,
        Arena,
        Outdoors
    }
}
