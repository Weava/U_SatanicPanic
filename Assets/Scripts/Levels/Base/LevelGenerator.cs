using Assets.Scripts.Generation.Painter.Cells.Base;
using Assets.Scripts.Generation.Painter.Rooms.Base;
using Assets.Scripts.Generation.RoomBuilding.Base;
using Assets.Scripts.Generation.RoomBuilding.Scaffolds;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Levels.Base
{
    public abstract class LevelGenerator : MonoBehaviour
    {
        #region Debug Properties
        public bool Debug_ShowRoomTemplates;
        public bool Debug_ShowCells;
        public bool Debug_ShowDoors;

        public RoomDebugPackage debugPackage;
        #endregion

        #region Generation Properties

        public bool allow_Room_1_2 = true;
        public bool allow_Room_2_2 = true;
        public bool allow_Room_2_3 = true;
        public bool allow_Room_3_3 = true;
        public bool allow_Room_4_4 = true;

        protected List<RoomSize> excludedRoomSizes { get { return GetExcludedRooms(); } }

        #endregion

        #region Scaffold Properties
        public Room_1_1_Scaffold Room_1_1_Scaffold;
        public Room_1_2_Scaffold Room_1_2_Scaffold;
        public Room_2_2_Scaffold Room_2_2_Scaffold;
        public Room_2_3_Scaffold Room_2_3_Scaffold;
        public Room_3_3_Scaffold Room_3_3_Scaffold;
        public Room_4_4_Scaffold Room_4_4_Scaffold;
        #endregion

        public virtual void BuildLevel()
        {
            if(Debug_ShowRoomTemplates)
            {
                debugPackage.RenderRoomTemplates();
            }

            if(Debug_ShowCells)
            {
                debugPackage.RenderMarkers();
            }

            if(Debug_ShowDoors)
            {
                debugPackage.RenderDoors();
            }
        }

        public void CleanUnclaimedCells()
        {
            var cellsToClean = CellCollection.collection.Where(x => !x.Value.claimed).Select(s => s.Key);
            foreach(var cell in cellsToClean.ToList())
            {
                CellCollection.collection.Remove(cell);
            }
        }

        private List<RoomSize> GetExcludedRooms()
        {
            var result = new List<RoomSize>();

            if (!allow_Room_1_2) result.Add(RoomSize.Room_1_2);
            if (!allow_Room_2_2) result.Add(RoomSize.Room_2_2);
            if (!allow_Room_2_3) result.Add(RoomSize.Room_2_3);
            if (!allow_Room_3_3) result.Add(RoomSize.Room_3_3);
            if (!allow_Room_4_4) result.Add(RoomSize.Room_4_4);

            return result;
        }
    }
}
