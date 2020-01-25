using Assets.Scripts.Generation.Blueprinting;
using Assets.Scripts.Generation.Painter.Cells.Base;
using Assets.Scripts.Misc;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Generation.Painter.Rooms.Base
{
    public class Room
    {
        #region Properties
        public string region = "";
        public string subregion = "";

        public Direction orientation;

        public RoomSize roomSize;

        public Dictionary<Vector3, Cell> cells = new Dictionary<Vector3, Cell>();

        public Vector3 rootPosition;

        public TagCollection tags;

        public Blueprint blueprint;

        #region Context

        public bool pathRoom = false;

        public List<Cell> DoorCells = new List<Cell>();
        public List<Cell> DeadCells = new List<Cell>();

        #endregion

        #endregion
    }

    public enum RoomSize
    {
        Room_1_1,
        Room_1_2,
        Room_2_2,
        Room_2_3,
        Room_3_3,
        Room_4_4,
        Room_5_5
    }
}
