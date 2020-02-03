using Assets.Scripts.Generation.Painter.Cells.Base;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Levels.Base
{
    public abstract class LevelGenerator : MonoBehaviour
    {
        public bool Debug_ShowRoomTemplates;
        public bool Debug_ShowCells;
        public bool Debug_ShowDoors;

        public RoomDebugPackage debugPackage;

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
    }
}
