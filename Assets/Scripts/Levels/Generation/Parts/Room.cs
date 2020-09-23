using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation.Parts
{
    public class Room : MonoBehaviour
    {
        [HideInInspector] public string id = Guid.NewGuid().ToString();

        public string roomName;

        public RoomType roomType;

        public RoomScaffold scaffold;

        void Awake()
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
