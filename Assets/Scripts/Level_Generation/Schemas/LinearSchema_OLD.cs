using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Level_Generation.Schema
{
    class LinearSchema_OLD : LevelSchema
    {
        public Direction primaryDirection;

        public int numberOfRooms;
        int numberOfRoomsLeft;

        private List<Direction> directionMask;

        bool done = false;

        bool resolvingConflict = false;

        private void Awake()
        {
            directionMask = new List<Direction>();
            directionMask.Add(primaryDirection);
            directionMask.AddRange(Directionf.GetNeighborDirections(primaryDirection));
            numberOfRoomsLeft = numberOfRooms;
        }

        private void FixedUpdate()
        {
            if(resolvingConflict)
            {
                if (lastRoomGenerated != null)
                {
                    if (lastRoomGenerated.ready && !lastRoomGenerated.inConflict)
                    {
                        resolvingConflict = false;
                        initResolution = true;
                        ResetConflictProperties(true);
                        return;
                    } else if(lastRoomGenerated.inConflict)
                    {
                        if (!AttemptResolveConflictedRoom(true))
                        {
                            //The resolution failed
                            throw new Exception("Level could not be resolved.");
                        }
                    }
                } else if (!AttemptResolveConflictedRoom(true))
                {
                    //The resolution failed
                    throw new Exception("Level could not be resolved.");
                }
            } else if(!done)
            {
                if(lastRoomGenerated != null)
                {   
                    if(lastRoomGenerated.inConflict)
                    {
                        resolvingConflict = true;
                        return;
                    } else
                    {
                        if (!lastRoomGenerated.ready) return;

                        lastRoomPrefabUsed = GetRandomRoomFromPool();
                        BuildNextRoom(null, lastRoomPrefabUsed, -1, directionMask);
                    }
                } else
                {
                    //InitSpawnRoom();
                }

                if (--numberOfRoomsLeft <= 0) done = true;
            } else //Testing for existing bugs
            {
                SceneManager.LoadScene("LinearTest");
            }
        }
    }
}
