using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation.Base.Mono.Debug
{
    public class RoomDebug : MonoBehaviour
    {
        public GameObject floor;
        public GameObject floor_important;

        public void RenderRoomDebug(Room room)
        {
            var container = new GameObject("Room - " + room.cells.First().region);
            foreach(var cell in room.cells)
            {
                if(cell.type != CellType.Cell)
                {
                    var floorInstance = Instantiate(floor_important, container.transform);
                    floorInstance.transform.position = cell.position;
                } else
                {
                    var floorInstance = Instantiate(floor, container.transform);
                    floorInstance.transform.position = cell.position;
                }
                
            }
        }
    }
}
