using Assets.Scripts.Generation.RoomBuilding.Base;
using Assets.Scripts.Generation.RoomBuilding.Suite.Base;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Generation.RoomBuilding
{
    public static class SimpleRoomScaffolding
    {
        public static RoomSuite roomSuite;

        public static void Scaffold(RoomScaffold scaffold)
        {
            var floor = roomSuite.floors[Random.Range(0, roomSuite.floors.Count)];
            var wall = roomSuite.walls[Random.Range(0, roomSuite.walls.Count)];
            var door = roomSuite.doors[Random.Range(0, roomSuite.doors.Count)];
            var connector = roomSuite.connectors[Random.Range(0, roomSuite.connectors.Count)];
            var spacer = roomSuite.spacers[Random.Range(0, roomSuite.spacers.Count)];

            foreach (var floorNode in scaffold.Nodes.Where(x => x.type == NodeType.Root).ToArray())
            {
                var floorInstance = scaffold.InstantiateNodeObject(floorNode, floor);
            }  

            foreach (var doorNode in scaffold.Nodes.Where(x => x.type == NodeType.Door).ToArray())
            {
                if(doorNode.options.isDoor)
                {
                    var doorInstance = scaffold.InstantiateNodeObject(doorNode, door);
                }
                else
                {
                    var wallInstance = scaffold.InstantiateNodeObject(doorNode, wall);
                }
            }

            foreach(var connectorNode in scaffold.Nodes.Where(x => x.type == NodeType.Connector).ToArray())
            {
                var connectorInstance = scaffold.InstantiateNodeObject(connectorNode, connector);
            }

            foreach (var spacerNode in scaffold.Nodes.Where(x => x.type == NodeType.Spacer).ToArray())
            {
                var spacerInstance = scaffold.InstantiateNodeObject(spacerNode, spacer);
            }
        }
    }
}
