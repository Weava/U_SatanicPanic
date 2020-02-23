using Assets.Scripts.Generation.Blueprinting;
using Assets.Scripts.Generation.Painter.Rooms.Base;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Generation.RoomBuilding.Base
{
    public abstract class RoomScaffold : MonoBehaviour
    {
        public List<RoomNode> Nodes;

        public RoomNode rootNode { get { return Nodes.First(x => x.type == NodeType.Root); } }

        public List<RoomNodeObject> nodeObjects = new List<RoomNodeObject>();

        public Room room;

        public RoomNodeObject InstantiateNodeObject(RoomNode node, RoomNodeObject nodeObject)
        {
            var instance = Instantiate(nodeObject, node.transform.position, node.transform.rotation);
            instance.transform.parent = transform;
            nodeObjects.Add(instance);
            return instance;
        }

        public void SetRoom()
        {
            foreach (var doorNode in Nodes.Where(x => x.type == NodeType.Door).ToArray())
            {
                if (room.blueprint.doors.Hits(MaskF.MaskValue(doorNode.index, doorNode.offset)))
                { doorNode.options.isDoor = true; }
            }
        }

        public void RemoveScaffolding()
        {
            foreach(var node in Nodes.ToArray())
            {
                Nodes.Remove(node);
                Destroy(node.transform.gameObject);
            }
        }
    }
}
