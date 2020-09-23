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
        public GameObject wall_connector;

        public GameObject ceiling_main;
        public GameObject ceiling_connector;
        public GameObject ceiling_column;

        public GameObject door;

        public GameObject stairwell;

        public void RenderRoomDebug(Room room)
        {
            var cells = room.GetCells();
            var container = new GameObject("Room - " + cells.First().GetRegion().regionName);
            container.name += cells.Any(x => x.important) ? " - Pathway" : "";
            container.name += cells.Any(x => x.type == CellType.Elevation) ? " - Elevation" : "";
            container.name += " [" + room.Data().parsing.roomType.ToString() + "]";
            foreach (var cell in cells)
            {
                if (cell.type != CellType.Cell)
                {
                    var floorInstance = Instantiate(floor_important, container.transform);
                    floorInstance.transform.position = cell.position;
                }
                else
                {
                    var floorInstance = Instantiate(floor, container.transform);
                    floorInstance.transform.position = cell.position;
                }
            }
        }

        public void RenderRoomScaffoldingDebug(Room room)
        {
            var scaffolding = Level.roomScaffolds[room.id];
            var roomContainer = new GameObject("Room");

            roomContainer.name += " [" + room.Data().parsing.roomType.ToString() + "]";

            #region Floor

            foreach (var main in scaffolding.floor.main)
            {
                if (main.root.elevationOverride_Upper) continue;
                if (main.claimed) continue;
                var instance = Instantiate(floor_main, roomContainer.transform);
                instance.transform.position = main.position;
            }

            foreach (var connector in scaffolding.floor.connectors)
            {
                if (connector.claimed) continue;
                var instance = Instantiate(floor_connector, roomContainer.transform);
                instance.transform.position = connector.position;
                instance.transform.LookAt(connector.rootCells.First().position);
            }

            foreach (var column in scaffolding.floor.columns)
            {
                if (column.claimed) continue;
                var instance = Instantiate(floor_column, roomContainer.transform);
                instance.transform.position = column.position;
            }

            #endregion Floor

            #region Wall

            foreach (var main in scaffolding.wall.main)
            {
                if (main.claimed) continue;
                var instance = Instantiate(wall_main, roomContainer.transform);
                instance.transform.position = main.position;
                instance.transform.LookAt(main.root.position);
            }

            foreach (var connector in scaffolding.wall.connectors)
            {
                if (connector.claimed) continue;
                var instance = Instantiate(wall_connector, roomContainer.transform);
                instance.transform.position = connector.position;
                instance.transform.LookAt(connector.root.position);
            }

            #endregion Wall

            #region Ceiling

            foreach (var main in scaffolding.ceiling.main)
            {
                if (main.claimed) continue;
                if (main.root.root.elevationOverride_Lower) continue;
                var instance = Instantiate(ceiling_main, roomContainer.transform);
                instance.transform.position = main.position;
            }

            foreach (var connector in scaffolding.ceiling.connectors)
            {
                if (connector.claimed) continue;
                var instance = Instantiate(ceiling_connector, roomContainer.transform);
                instance.transform.position = connector.root.position;
                instance.transform.LookAt(connector.root.rootCells.First().position);
                instance.transform.position = connector.position;
            }

            foreach (var column in scaffolding.ceiling.columns)
            {
                if (column.claimed) continue;
                var instance = Instantiate(ceiling_column, roomContainer.transform);
                instance.transform.position = column.position;
            }

            #endregion Ceiling

            var test = Level.roomScaffolds.Select(s => s.Value.elevation).ToList();

            foreach (var elevation in scaffolding.elevation)
            {
                var instance = Instantiate(stairwell, roomContainer.transform);
                instance.transform.position = elevation.position;
            }
        }

        public void RenderRoomScaffoldingDoorDebug()
        {
            foreach (var doorNode in Level.doors)
            {
                var instance = Instantiate(door);
                instance.transform.position = doorNode.position;
                instance.transform.LookAt(doorNode.cell_1.position);

                instance = Instantiate(door);
                instance.transform.position = doorNode.position;
                instance.transform.LookAt(doorNode.cell_2.position);
            }
        }
    }
}