using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation.Base.Mono.Debug
{
    public class RoomDebug : MonoBehaviour
    {
        public GameObject floor;
        public GameObject floor_important;

        public GameObject floor_main;
        public GameObject floor_connector;
        public GameObject floor_column;

        public GameObject wall_main;

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

        public void RenderRoomScaffoldingDebug(Room room)
        {
            var scaffolding = Level.roomScaffolds[room];
            var roomContainer = new GameObject("Room");

            #region Floor
            foreach (var main in scaffolding.floor.main)
            {
                var instance = Instantiate(floor_main, roomContainer.transform);
                instance.transform.position = main.position;
            }

            foreach(var connector in scaffolding.floor.connectors)
            {
                var instance = Instantiate(floor_connector, roomContainer.transform);
                instance.transform.position = connector.position;
                instance.transform.LookAt(connector.rootCell.position);
            }

            foreach (var column in scaffolding.floor.columns)
            {
                var instance = Instantiate(floor_column, roomContainer.transform);
                instance.transform.position = column.position;
            }
            #endregion

            #region Wall
            foreach(var main in scaffolding.wall.main)
            {
                var instance = Instantiate(wall_main, roomContainer.transform);
                instance.transform.position = main.position;
                instance.transform.LookAt(main.root.position);
            }
            #endregion
        }
    }
}
