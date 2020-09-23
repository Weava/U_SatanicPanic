using Assets.Scripts.Levels.Generation.Extensions;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation.Base.Mono.Debug
{
    public class NodeDebug : MonoBehaviour
    {
        public GameObject doorNode;
        public GameObject poiNode;
        public GameObject poiCompoundNode;

        public void RenderDoorNodes()
        {
            var collection = new GameObject("Doors");

            foreach (var door in Level.doors.ToArray())
            {
                var position = new Vector3(
                        (door.cell_1.position.x + door.cell_2.position.x) / 2,
                        (door.cell_1.position.y + door.cell_2.position.y) / 2,
                        (door.cell_1.position.z + door.cell_2.position.z) / 2
                    );

                var instance = Instantiate(doorNode, collection.transform);
                instance.transform.position = position;
                instance.name = "Door";
            }
        }

        public void RenderPOI()
        {
            var POI = Level.roomParsings.SelectMany(s => s.Value.pointsOfInterest).ToList();

            var collection = new GameObject("Points of Interest");

            var singleCollection = new GameObject("Simple");
            singleCollection.transform.parent = collection.transform;

            var compoundCollection = new GameObject("Compound");
            compoundCollection.transform.parent = collection.transform;

            foreach(var poi_node in POI.Where(x => x.cells.Count == 1 && ! x.hasDoors))
            {
                var position = poi_node.cells.PositionBetween();

                var instance = Instantiate(poiNode, singleCollection.transform);
                instance.transform.position = position;
                instance.name = poi_node.interestType.ToString();
            }
        }
    }
}
