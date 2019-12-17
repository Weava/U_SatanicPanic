using UnityEngine;

namespace Assets.Scripts.Level_Generation
{
    public class Door : MonoBehaviour
    {
        public int index;
        public Direction direction;
        public bool occupied { get { return connectedRoom != null; } }
        public Room connectedRoom;
        public Room rootRoom;
    }
}
