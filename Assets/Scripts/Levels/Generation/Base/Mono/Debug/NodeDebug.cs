using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation.Base.Mono.Debug
{
    public class NodeDebug : MonoBehaviour
    {
        public GameObject doorNode;

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
    }
}
