using Assets.Scripts.Generation.RoomBuilding.Base;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Generation.RoomBuilding.Suite.Base
{
    public class RoomSuite : MonoBehaviour
    {
        public List<RoomNodeObject> floors;
        public List<RoomNodeObject> walls;
        public List<RoomNodeObject> doors;
        public List<RoomNodeObject> connectors;
        public List<RoomNodeObject> spacers;
        public List<RoomNodeObject> columns;
    }
}
