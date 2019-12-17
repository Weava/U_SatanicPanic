using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Level_Generation
{
    public abstract class LevelSchema_OLD : MonoBehaviour
    {
        #region Properties
        public string seed;

        public List<Room> roomPool = new List<Room>();

        public Room spawnRoom;

        #endregion

        #region Meta Data
        protected Room lastRoomGenerated;
        protected Room lastRoomPrefabUsed;

        #region Conflict Meta
        protected Door originalConflictedDoor;
        protected int originalConflictedIndex;
        protected Room originalConflictedParent;

        protected List<Room> availableRoomsToResolveTo;
        protected Room selectedRoomForResolution;
        protected Door selectedDoorForResolution;
        protected List<Door> availableDoorsInCurrentRoom = new List<Door>(); //Used for resolving a conflict
        protected List<int> availableIndexesInCurrentChild = new List<int>();
        protected bool initResolution = true;
        #endregion

        public List<Room> levelRooms = new List<Room>();
        #endregion

        private void Start()
        {
            if (seed != "")
            {
                Random.InitState(ConvertSeed(seed));
            }
            else
            {
                Random.InitState((int)DateTime.Now.Ticks);
            }
        }

        public void InitSpawnRoom()
        {
            lastRoomGenerated = Instantiate(spawnRoom, transform.position, transform.rotation);
            levelRooms.Add(lastRoomGenerated);
        }

        /// <summary>
        /// From the last room generated, build the next room
        /// </summary>
        public virtual void BuildNextRoom(Door door, Room prefab, int index = -1, List<Direction> directionMask = null, List<Door> doorMask = null)
        {
            if (door == null)
            { door = lastRoomGenerated.GetRandomAvailableDoor(directionMask, doorMask); }

            if(index == -1)
            {
                var prefabAvailableDoors = prefab.GetAvailableDoors(Directionf.GetOppositeDirection(door.direction));
                var indexOptions = prefabAvailableDoors.Select(s => s.index).ToList();
                index = indexOptions[Random.Range(0, indexOptions.Count)];
            }

            if(lastRoomGenerated != null)
            {
                BuildRoom(lastRoomGenerated, door, prefab, index);
            } else
            {
                throw new Exception("An initial room must exist to call this function.");
            }
        }

        /// <summary>
        /// From a random room, build an additional room, if possible
        /// </summary>
        public virtual void BuildRoom(Room rootRoom, Door rootDoor, Room prefab, int index)
        {
            var room = rootRoom.SpawnChildRoom(rootDoor, prefab, index);
            lastRoomGenerated = room;
            levelRooms.Add(lastRoomGenerated);
        }

        /// <summary>
        /// Attempt to resolve the last generated room that is conflicted. Order of operations: child door indexes, root directions, root parent, repeat
        /// </summary>
        /// <param name="recursive">Resolve by parent tree, false will be a randomly selected room</param>
        /// <returns>Returns true if the room has found a nonconflicting connection. Returns false if all available room
        /// options have been exhausted and still cannot resolve the room.</returns>
        public virtual bool AttemptResolveConflictedRoom(bool recursive = false)
        {
            if(initResolution) //Setup tracking to resolve conflicting room
            {
                ResetConflictProperties(recursive);
                initResolution = false;
            }
            if(lastRoomGenerated != null)
            {
                lastRoomGenerated.Remove();
                lastRoomGenerated = null;
            }
                
            if (availableRoomsToResolveTo.Count() > 0) //Pool of rooms left to attempt resolution
            {
                if(selectedRoomForResolution == null) //Null room when no options available in it to resolve to
                {
                    //Get next root room
                    if(recursive)
                    {
                        selectedRoomForResolution = availableRoomsToResolveTo[0]; //These are ordered by their relationship to lastRoomGenerated
                        availableRoomsToResolveTo.Remove(selectedRoomForResolution);
                    } else
                    {
                        selectedRoomForResolution = availableRoomsToResolveTo[Random.Range(0, availableRoomsToResolveTo.Count())];
                        availableRoomsToResolveTo.Remove(selectedRoomForResolution);
                    }

                    availableDoorsInCurrentRoom = selectedRoomForResolution.GetAvailableDoors();
                    if (selectedRoomForResolution == originalConflictedParent && originalConflictedParent.doors.Contains(originalConflictedDoor))
                    {  availableDoorsInCurrentRoom.Add(originalConflictedDoor); }
                }

                if(availableDoorsInCurrentRoom.Count() > 0)
                {
                    if(selectedDoorForResolution == null)
                    {
                        selectedDoorForResolution = availableDoorsInCurrentRoom[Random.Range(0, availableDoorsInCurrentRoom.Count())];
                        availableDoorsInCurrentRoom.Remove(selectedDoorForResolution);

                        availableIndexesInCurrentChild = lastRoomPrefabUsed.doors
                            .Where(x => x.direction == Directionf.GetOppositeDirection(selectedDoorForResolution.direction))
                            .Select(s => s.index)
                            .ToList();

                        //Don't use the orignal configuration that caused the conflict in the first place
                        if(selectedDoorForResolution == originalConflictedDoor)
                        { availableIndexesInCurrentChild.Remove(originalConflictedIndex); }
                    }

                    if (availableIndexesInCurrentChild.Count() > 0)
                    {
                        var index = availableIndexesInCurrentChild[Random.Range(0, availableIndexesInCurrentChild.Count())];
                        availableIndexesInCurrentChild.Remove(index);

                        //Weird bug
                        if (!selectedRoomForResolution.doors.Contains(selectedDoorForResolution))
                        {
                            selectedDoorForResolution = null;
                            AttemptResolveConflictedRoom(recursive);
                            return true;
                        }

                        BuildRoom(selectedRoomForResolution, selectedDoorForResolution, lastRoomPrefabUsed, index); //Probably not clearing the indexes when switching rooms... 1x1 -> 1x2
                    }
                    else
                    {
                        selectedDoorForResolution = null;
                        availableIndexesInCurrentChild = new List<int>();
                        AttemptResolveConflictedRoom(recursive);
                    }

                } else //Room depleted, next room
                {
                    selectedRoomForResolution = null;
                    availableDoorsInCurrentRoom = new List<Door>();
                    AttemptResolveConflictedRoom(recursive);
                }

                return true;

            } else /*Resolution Failed*/
            {
                return false;
            }
        }

        public virtual Room GetRandomRoomFromPool(/*Roomsize Mask*/)
        {
            return roomPool[Random.Range(0, roomPool.Count)];
        }

        protected void ResetConflictProperties(bool recursive)
        {
            availableRoomsToResolveTo = new List<Room>();
            availableDoorsInCurrentRoom = new List<Door>();
            availableIndexesInCurrentChild = new List<int>();
            selectedDoorForResolution = null;
            selectedRoomForResolution = null;

            originalConflictedDoor = lastRoomGenerated.parentRoom.doors.First(x => x.connectedRoom == lastRoomGenerated);
            originalConflictedIndex = lastRoomGenerated.doors.First(x => x.connectedRoom == lastRoomGenerated.parentRoom).index;
            originalConflictedParent = lastRoomGenerated.parentRoom;

            GetRoomsForResolution(recursive);
        }

        protected void GetRoomsForResolution(bool recursive)
        {
            if (recursive)
            {
                var roomStep = lastRoomGenerated.parentRoom;
                while (true)
                {
                    availableRoomsToResolveTo.Add(roomStep);
                    roomStep = roomStep.parentRoom;

                    if (roomStep == null) break;
                }
            }
            else
            {
                availableRoomsToResolveTo = levelRooms.Where(x => x != lastRoomGenerated).ToList();
            }
        }

        private int ConvertSeed(string seed)
        {
            var temp = "";

            foreach(char c in seed)
            {
                temp += ((int)c).ToString();
            }

            return (int)long.Parse(temp);
        }
    }
}
