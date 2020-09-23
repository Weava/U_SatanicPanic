using UnityEngine;

namespace Assets.Scripts.Levels.Generation.Base.Mono
{
    public class PathMarker : MonoBehaviour
    {
        private const int PATH_GRID_SNAP_AMOUNT_HORIZONTAL = 8;
        private const int PATH_GRID_SNAP_AMOUNT_VERTICAL = 4;

        private void Awake()
        {
            transform.position = new Vector3(
                Mathf.FloorToInt(transform.position.x / PATH_GRID_SNAP_AMOUNT_HORIZONTAL) * PATH_GRID_SNAP_AMOUNT_HORIZONTAL,
                Mathf.FloorToInt(transform.position.y / PATH_GRID_SNAP_AMOUNT_VERTICAL) * PATH_GRID_SNAP_AMOUNT_VERTICAL,
                Mathf.FloorToInt(transform.position.z / PATH_GRID_SNAP_AMOUNT_HORIZONTAL) * PATH_GRID_SNAP_AMOUNT_HORIZONTAL
                );
        }
    }
}