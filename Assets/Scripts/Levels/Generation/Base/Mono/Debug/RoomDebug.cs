using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation.Base.Mono.Debug
{
    public class RoomDebug : MonoBehaviour
    {
        public GameObject floor;

        public void RenderRoomDebug(Room room)
        {
            var container = new GameObject("Room - " + room.cells.First().region);
            foreach(var cell in room.cells)
            {
                var floorInstance = Instantiate(floor, container.transform);
                floorInstance.transform.position = cell.position;
            }
        }
    }
}
