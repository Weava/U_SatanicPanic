using Assets.Scripts.Generation.RoomBuilding.Suite.Base;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation
{
    public class LevelRegion : MonoBehaviour
    {
        public GameObject startNode;
        public GameObject endNode;

        public string regionName;

        private void Awake()
        {
            //Snap nodes to vector whole numbers
            startNode.transform.position = new Vector3(
                    Mathf.RoundToInt(startNode.transform.position.x),
                    Mathf.RoundToInt(startNode.transform.position.y),
                    Mathf.RoundToInt(startNode.transform.position.z)
                );

            endNode.transform.position = new Vector3(
                    Mathf.RoundToInt(endNode.transform.position.x),
                    Mathf.RoundToInt(endNode.transform.position.y),
                    Mathf.RoundToInt(endNode.transform.position.z)
                );
        }

        public void BuildPath()
        {

        }
    }
}
