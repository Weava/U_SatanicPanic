using UnityEngine;

namespace Assets.Scripts.Generation.RoomBuilding.Base
{
    public class RoomNode : MonoBehaviour
    {
        public Direction normal;
        public NodeType type;

        public int index;
        public int offset;

        public RoomNodeOptions options = new RoomNodeOptions();

        public bool claimed = false;
        public RoomNodeObject nodeObject;
    }

    public class RoomNodeOptions
    {
        public bool isDoor;
    }

    public enum NodeType
    {
        Door,
        Root,
        Connector,
        Spacer,
        Column
    }
}
