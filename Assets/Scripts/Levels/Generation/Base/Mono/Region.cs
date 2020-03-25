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

        public float cellDecayAmount;

        public PathMarker startNode;
        public PathMarker endNode;

        public Vector3 startPosition { get { return startNode.transform.position; } }
        public Vector3 endPosition { get { return endNode.transform.position; } }

        public List<Cell> cells = new List<Cell>();

        void OnDrawGizmos()
        {
             Gizmos.color = Color.yellow;
             if (startNode != null && endNode != null)
                 Gizmos.DrawLine(startNode.transform.position, endNode.transform.position);
        }
    }
}
