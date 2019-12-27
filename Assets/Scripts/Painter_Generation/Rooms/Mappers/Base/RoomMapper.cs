using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Painter_Generation.Rooms.Mappers.Base
{
    public class RoomMapper : MonoBehaviour
    {
        public LevelMap map;

        #region Progress Flags

        public bool room_1_2_Complete;
        public bool room_2_2_Complete;
        public bool room_2_3_Complete;
        public bool room_3_3_Complete;

        #endregion

        public virtual void Map()
        {

        }

        protected void ClaimRoom(RoomClaim roomClaim, LevelRegion region, ref bool doneFlag)
        {
            if (roomClaim != null)
            {
                var room = new Room(RoomType.Room, roomClaim.dimensions, roomClaim);
                region.rooms.Add(room);
            }
            else
            {
                doneFlag = true;
            }
        }

        protected void Reset()
        {
            room_1_2_Complete = false;
            room_2_2_Complete = false;
            room_2_3_Complete = false;
            room_3_3_Complete = false;
        }
    }
}
