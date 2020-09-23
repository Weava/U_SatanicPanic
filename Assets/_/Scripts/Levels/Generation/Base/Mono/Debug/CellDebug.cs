using UnityEngine;

namespace Assets.Scripts.Levels.Generation.Base.Mono.Debug
{
    public class CellDebug : MonoBehaviour
    {
        public GameObject PathwayCellMarker;
        public GameObject ElevationCellMarker;
        public GameObject CellMarker;
        public GameObject SpawnCellMarker;

        public void RenderCellDebug(Vector3 position, CellType type = CellType.Cell)
        {
            GameObject instance = null;

            switch (type)
            {
                case CellType.Spawn:
                    instance = Instantiate(SpawnCellMarker, position, new Quaternion());
                    break;
                case CellType.Pathway:
                    instance = Instantiate(PathwayCellMarker, position, new Quaternion());
                    break;
                case CellType.Elevation:
                    instance = Instantiate(ElevationCellMarker, position, new Quaternion());
                    break;
                default:
                    instance = Instantiate(CellMarker, position, new Quaternion());
                    break;
            }

            var cell = CellCollection.cells[position];
            instance.gameObject.name = RegionCollection.regions[cell.regionId].regionName + " - " + cell.type;
            if(cell.type != CellType.Cell)
            {
                instance.name += " [" + cell.sequence.ToString() + "]";
            }
        }
    }
}
