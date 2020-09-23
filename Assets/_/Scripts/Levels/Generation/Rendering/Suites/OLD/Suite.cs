//using Assets.Scripts.Levels.Generation.Base;
//using Assets.Scripts.Levels.Generation.Extensions;
//using System;
//using System.Collections.Generic;
//using Assets.Scripts.Levels.Generation.Base.Mono;
//using UnityEngine;

//namespace Assets.Scripts.Levels.Generation.Rendering.Suites
//{
//    /// <summary>
//    /// Container for level to use for rendering room partials and specific room features
//    /// </summary>
//    public class Suite : MonoBehaviour
//    {
//        [HideInInspector] public string id;
//        [HideInInspector] public Room targetRoom;
//        [HideInInspector] public GameObject roomInstanceContainer;
//        [HideInInspector] public List<Region> regionsAllowed = new List<Region>();


//        [HideInInspector] public SuiteType type;

//        #region Options
//        [Header("Suite Options")]
//        [Range(0, 100)]
//        public int priority = 0;

//        /// <summary>
//        /// If required, only one instance will be used in a level
//        /// </summary>
//        public bool required = false;

//        [Range(0, 100)]
//        public int chanceToRender = 100;

//        //Number of times that a suite is allowed to render
//        public int frequencyCap = 1;

//        [HideInInspector] public int timesUsed = 0;

//        #endregion

//        public virtual bool IsValid()
//        { throw new NotImplementedException("Suite must implement an IsValid validation method."); }

//        public virtual void Render()
//        { throw new NotImplementedException("Suite must implement a Render method."); }

//        #region Helper Methods

//        /// <summary>
//        /// Based on a local root at 0,0,0
//        /// </summary>
//        /// <param name="offset"></param>
//        /// <returns></returns>
//        protected static Vector3 ProjectToCellSpace(Vector3 root, Vector3 offset)
//        {
//            return new Vector3(
//                root.x + (offset.x * Cellf.CELL_STEP_OFFSET),
//                root.y + (offset.y * Cellf.CELL_ELEVATION_OFFSET),
//                root.z + (offset.z * Cellf.CELL_STEP_OFFSET)
//            );
//        }

//        protected void FetchRoomRenderContainer()
//        {
//            if (!Level.Rooms.ContainsKey(targetRoom.id))
//            {
//                Level.Rooms[targetRoom.id] = new LevelRoom()
//                {
//                    roomId = targetRoom.id,
//                    renderContainer = new GameObject("Render Container")
//                };
//            }

//            roomInstanceContainer = Level.Rooms[targetRoom.id].renderContainer;
//        }

//        public virtual void Init()
//        {
//            id = Guid.NewGuid().ToString();
//        }

//        #endregion
//    }

//    public enum SuiteType
//    {
//        Not_Assigned,
//        Fill,
//        Static
//    }
//}
