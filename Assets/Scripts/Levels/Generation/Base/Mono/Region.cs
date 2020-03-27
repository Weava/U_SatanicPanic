using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation.Base.Mono
{
    public class Region : MonoBehaviour
    {
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

        public PathMarker startNode;
        public PathMarker endNode;

        [HideInInspector]
        public Vector3 startPosition { get { return startNode.transform.position; } }
        [HideInInspector]
        public Vector3 endPosition { get { return endNode.transform.position; } }

        [HideInInspector]
        public List<Cell> cells = new List<Cell>();

        [HideInInspector]
        public List<Room> rooms = new List<Room>();

        void OnDrawGizmos()
        {
             Gizmos.color = Color.yellow;
             if (startNode != null && endNode != null)
                 Gizmos.DrawLine(startNode.transform.position, endNode.transform.position);
        }
    }

    public enum RoomClaimingStrategy
    {
        Bloom, //Fan out from a root cell in each direction
        PartialBloom, //Randomly decide not to claim cells within a fan step
    }
}
