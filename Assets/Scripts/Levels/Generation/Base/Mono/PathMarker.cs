
using Assets.Scripts.Levels.Generation.Extensions;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation.Base.Mono
{
    public class PathMarker : MonoBehaviour
    {
        public string regionName;

        public bool hasSpawn;

        public Vector3 startPosition { get { return transform.position; } }

        public Vector3 endPosition { get { if(endMarker != null)
                { return endMarker.transform.position; }
                else { return new Vector3(); } }
        }

        public PathMarker endMarker;
    }
}
