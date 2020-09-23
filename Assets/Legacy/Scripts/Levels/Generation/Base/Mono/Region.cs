using Assets.Scripts.Levels.Generation.Rendering.Suites.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation.Base.Mono
{
    public class Region : MonoBehaviour
    {
        [HideInInspector]
        public string id;

        public string regionName;

        public bool cellExpansionConstant = true;
        public int cellExpansionAmount = 0;
        public int cellExpansionStart = 0;
        public int cellExpansionMiddle = 0;
        public int cellExpansionEnd = 0;

        public int proliferationAmount = 0;

        public float cellDecayAmount;

        public RoomClaimingStrategy roomClaimingStrategy;
        public int maximumRoomSize = 1;
        public float claimChance = 1;
        public bool greedyClaiming = false;

        public PathMarker startNode;
        public PathMarker endNode;

        public List<Suite> suites = new List<Suite>();

        [HideInInspector]
        public Vector3 startPosition { get { return startNode.transform.position; } }

        [HideInInspector]
        public Vector3 endPosition { get { return endNode.transform.position; } }

        //[HideInInspector]
        //public List<Cell> cells = new List<Cell>();

        //[HideInInspector]
        //public List<Room> rooms = new List<Room>();

        private void Awake()
        {
            id = Guid.NewGuid().ToString();
        }

        private void Start()
        {
            //foreach (var suite in suites)
            //{
            //    suite.Init();

            //    if (!Level.suiteCollection.ContainsKey(suite.id))
            //    {
            //        Level.suiteCollection.Add(suite.id, suite);
            //        suite.regionsAllowed.Add(this);
            //    }
            //}
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            if (startNode != null && endNode != null)
                Gizmos.DrawLine(startNode.transform.position, endNode.transform.position);
        }
    }

    public static class RegionCollection
    {
        public static Dictionary<string, Region> regions = new Dictionary<string, Region>();

        public static List<Cell> GetCells(this Region region)
        {
            return CellCollection.cells.Where(x => x.Value.regionId == region.id).Select(s => s.Value).ToList();
        }

        public static List<Room> GetRooms(this Region region)
        {
            return RoomCollection.rooms.Where(x => x.Value.regionId == region.id).Select(s => s.Value).ToList();
        }
    }

    public enum RoomClaimingStrategy
    {
        Bloom, //Fan out from a root cell in each direction
        PartialBloom, //Randomly decide not to claim cells within a fan step
        LimitedStep, //Controlled procedure for more regular shaped rooms
        Deterministic
    }
}