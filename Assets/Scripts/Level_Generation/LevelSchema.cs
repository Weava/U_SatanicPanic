using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Level_Generation
{
    public abstract class LevelSchema : MonoBehaviour
    {
        #region Metadata

        [SerializeField]
        protected bool recursiveConflictResolution = false;

        protected bool done = false;
        protected bool resolvingConflict = false;

        [SerializeField]
        protected int numberOfRooms; //Number of rooms to generate
        protected int numberOfRoomsLeft; //Number of rooms left to generate

        [SerializeField]
        protected RootChildSchemaPosition spawnChildSchemaAt;

        //Schema will make each room generated inside follow a specific theme for continuity
        [SerializeField]
        protected RoomTheme schemaTheme;

        [SerializeField]
        protected Room rootRoom; //Point of origin for a level schema

        protected Room lastRoomGenerated = null;
        protected Room lastRoomPrefabUsed = null; //For resolving a conflicted room, need a copy of the room we're trying to build

        [SerializeField]
        protected List<Direction> directionMask;

        [SerializeField]
        protected List<Room> roomPool = new List<Room>();
        [SerializeField]
        protected List<Room> requiredRooms = new List<Room>(); //After generating the level, these rooms will replace an exisitng room that matches the specific room type
        protected List<Room> schemaRooms = new List<Room>(); //Rooms that exist in this schema partition

        protected LevelSchema parentSchema;

        protected LevelGenerator levelGenerator;

        #region Conflict Data

        protected bool initResolution = true;

        //Original Conflicted Room Configuration
        protected int originalConflictedIndex;
        protected Door originalConflictedDoor;
        protected Room originalConflictedParent;

        //In-Progress Conflict Resoultion Tracking
        protected Room selectedRoomForResolution;
        protected Door selectedDoorForResolution;

        protected List<int> availableIndexesInCurrentChild = new List<int>();
        protected List<Door> availableDoorsInCurrentRoom = new List<Door>();
        protected List<Room> availableRoomsToResolveTo;

        #endregion Conflict Data
        #endregion Metadata

        #region Generation Methods

        /// <summary>
        /// From the last room generated, build the next room
        /// </summary>
        public virtual void BuildNextRoom(Door door, Room prefab, int index = -1, List<Direction> directionMask = null, List<Door> doorMask = null)
        {
            if (door == null)
            { door = lastRoomGenerated.GetRandomAvailableDoor(directionMask, doorMask); }

            if (index == -1)
            {
                var prefabAvailableDoors = prefab.GetAvailableDoors(Directionf.GetOppositeDirection(door.direction));
                var indexOptions = prefabAvailableDoors.Select(s => s.index).ToList();
                index = indexOptions[Random.Range(0, indexOptions.Count)];
            }

            if (lastRoomGenerated != null)
            {
                BuildRoom(lastRoomGenerated, door, prefab, index);
            }
            else
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
            schemaRooms.Add(lastRoomGenerated);
            lastRoomGenerated.transform.parent = this.transform;
        }

        /// <summary>
        /// Attempt to resolve the last generated room that is conflicted. Order of operations: child door indexes, root directions, root parent, repeat
        /// </summary>
        /// <param name="recursive">Resolve by parent tree, false will be a randomly selected room</param>
        /// <returns>Returns true if the room has found a nonconflicting connection. Returns false if all available room
        /// options have been exhausted and still cannot resolve the room.</returns>
        public virtual bool AttemptResolveConflictedRoom(bool recursive = false)
        {
            if (initResolution) //Setup tracking to resolve conflicting room
            {
                ResetConflictProperties(recursive);
                initResolution = false;
            }
            if (lastRoomGenerated != null)
            {
                lastRoomGenerated.Remove();
                lastRoomGenerated = null;
            }

            if (availableRoomsToResolveTo.Count() > 0) //Pool of rooms left to attempt resolution
            {
                if (selectedRoomForResolution == null) //Null room when no options available in it to resolve to
                {
                    //Get next root room
                    if (recursive)
                    {
                        selectedRoomForResolution = availableRoomsToResolveTo[0]; //These are ordered by their relationship to lastRoomGenerated
                        availableRoomsToResolveTo.Remove(selectedRoomForResolution);
                    }
                    else
                    {
                        selectedRoomForResolution = availableRoomsToResolveTo[Random.Range(0, availableRoomsToResolveTo.Count())];
                        availableRoomsToResolveTo.Remove(selectedRoomForResolution);
                    }

                    availableDoorsInCurrentRoom = selectedRoomForResolution.GetAvailableDoors();
                    if (selectedRoomForResolution == originalConflictedParent && originalConflictedParent.doors.Contains(originalConflictedDoor))
                    { availableDoorsInCurrentRoom.Add(originalConflictedDoor); }
                }

                if (availableDoorsInCurrentRoom.Count() > 0)
                {
                    if (selectedDoorForResolution == null)
                    {
                        selectedDoorForResolution = availableDoorsInCurrentRoom[Random.Range(0, availableDoorsInCurrentRoom.Count())];
                        availableDoorsInCurrentRoom.Remove(selectedDoorForResolution);

                        availableIndexesInCurrentChild = lastRoomPrefabUsed.doors
                            .Where(x => x.direction == Directionf.GetOppositeDirection(selectedDoorForResolution.direction))
                            .Select(s => s.index)
                            .ToList();

                        //Don't use the orignal configuration that caused the conflict in the first place
                        if (selectedDoorForResolution == originalConflictedDoor)
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

                }
                else //Room depleted, next room
                {
                    selectedRoomForResolution = null;
                    availableDoorsInCurrentRoom = new List<Door>();
                    AttemptResolveConflictedRoom(recursive);
                }

                return true;

            }
            else /*Resolution Failed*/
            {
                return false;
            }
        }

        public virtual Room GetRandomRoomFromPool(/*Roomsize Mask*/)
        {
            return roomPool[Random.Range(0, roomPool.Count)];
        }

        #endregion

        #region Monobehaviour

        protected virtual void Awake()
        {
            numberOfRoomsLeft = numberOfRooms;
            if (rootRoom == null)
            { throw new Exception("Schema must have a root room."); }

            if (rootRoom.parentSchema == null) rootRoom.parentSchema = this;

            lastRoomGenerated = rootRoom;
        }

        #endregion

        #region Protected Methods

        protected virtual void BuildStep()
        {
            if (lastRoomGenerated != null && !resolvingConflict)
            {
                if (lastRoomGenerated.inConflict)
                {
                    resolvingConflict = true;
                    return;
                }
                else
                {
                    if (!lastRoomGenerated.ready) return;

                    lastRoomPrefabUsed = GetRandomRoomFromPool();
                    BuildNextRoom(null, lastRoomPrefabUsed, -1, directionMask);
                }
            }
            else if(lastRoomGenerated == null)
            {
                throw new Exception("A root room has not been defined.");
            }

            if (--numberOfRoomsLeft <= 0) done = true;
        }

        protected virtual void ResolveConflictStep()
        {
            if (lastRoomGenerated != null)
            {
                if (lastRoomGenerated.ready && !lastRoomGenerated.inConflict)
                {
                    resolvingConflict = false;
                    initResolution = true;
                    ResetConflictProperties(recursiveConflictResolution);
                    return;
                }
                else if (lastRoomGenerated.inConflict)
                {
                    if (!AttemptResolveConflictedRoom(recursiveConflictResolution))
                    {
                        //The resolution failed
                        throw new Exception("Level could not be resolved.");
                    }
                }
            }
            else if (!AttemptResolveConflictedRoom(recursiveConflictResolution))
            {
                //The resolution failed
                throw new Exception("Level could not be resolved.");
            }
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

                    if (roomStep == null || roomStep == rootRoom) break;
                }
            }
            else
            {
                availableRoomsToResolveTo = schemaRooms.Where(x => x != lastRoomGenerated).ToList();
            }
        }

        #endregion

        #region Getters and Setters

        public List<Room> GetSchemaRooms()
        {
            return schemaRooms;
        }

        /// <summary>
        /// Depends on the intended use case for each schema. Determines where the next schema in the generator will begin.
        /// </summary>
        /// <returns></returns>
        public virtual Room GetRootForNextSchema()
        {
            switch(spawnChildSchemaAt)
            {
                case RootChildSchemaPosition.NorthMost:
                    return Schemaf.GetNorthMostRoom(levelGenerator.GetBoundsOfLevel());
                case RootChildSchemaPosition.EastMost:
                    return Schemaf.GetEastMostRoom(levelGenerator.GetBoundsOfLevel());
                case RootChildSchemaPosition.SouthMost:
                    return Schemaf.GetSouthMostRoom(levelGenerator.GetBoundsOfLevel());
                case RootChildSchemaPosition.WestMost:
                    return Schemaf.GetWestMostRoom(levelGenerator.GetBoundsOfLevel());
                case RootChildSchemaPosition.FarthestRoom:
                default:
                    return this.GetFurthestRoomFromRoot();
            }
        }

        public Room GetRootRoom()
        {
            return rootRoom;
        }

        public void SetRootRoom(Room room)
        {
            rootRoom = room;
            parentSchema = rootRoom.parentSchema;
        }

        public bool IsDoneGenerating()
        {
            return done;
        }

        public Bounds GetBoundsOfRoomCollection()
        {
            var bounds = new Bounds(this.transform.position, new Vector3());
            foreach(Room room in schemaRooms)
            {
                if (room == null) continue;
                var collidor = room.GetComponent<Collider>();
                if(collidor != null)
                bounds.Encapsulate(collidor.bounds);
            }
            return bounds;
        }

        public void SetParentSchema(LevelSchema schema)
        {
            parentSchema = schema;
        }

        public void SetLevelGenerator(LevelGenerator generator)
        {
            levelGenerator = generator;
        }

        //void OnDrawGizmos()
        //{
        //    // A sphere that fully encloses the bounding box.
        //    Vector3 center = testBounds.center;
        //    float radius = testBounds.extents.magnitude;

        //    Gizmos.color = Color.white;
        //    Gizmos.DrawWireSphere(center, radius);
        //}

        #endregion
    }

    public enum RootChildSchemaPosition
    {
        FarthestRoom,
        NorthMost,
        EastMost,
        SouthMost,
        WestMost
    }
}
