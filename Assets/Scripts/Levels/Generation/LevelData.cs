using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Levels.Generation.Parts;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation
{
    public class LevelData : MonoBehaviour
    {
        public static Dictionary<string, Room> rooms = new Dictionary<string, Room>();

        // Room id pair -> room connection
        public static Dictionary<Tuple<string, string>, RoomConnection> connections = new Dictionary<Tuple<string, string>, RoomConnection>();

        #region Helper Methods

        public Room GetRoomById(string id)
        {
            if (rooms.ContainsKey(id))
            {
                return rooms[id];
            }

            return null;
        }

        #endregion
    }
}
