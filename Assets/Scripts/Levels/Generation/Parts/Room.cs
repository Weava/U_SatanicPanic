using System;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation.Parts
{
    public class Room : MonoBehaviour
    {
        [HideInInspector] public string id = Guid.NewGuid().ToString();

        public string roomName;

        public RoomType roomType;

        public RoomScaffold scaffold;

        private void Awake()
        {
            scaffold.roomId = id;
            scaffold.doorNodes.ForEach(x => x.roomId = id);
        }
    }

    public enum RoomType
    {
        Room,
        Connector
    }
}