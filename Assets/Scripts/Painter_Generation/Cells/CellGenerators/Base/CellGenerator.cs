using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Painter_Generation.Cells.CellGenerators.Base
{
    public abstract class CellGenerator : MonoBehaviour
    {
        public GameObject Marker;
        public GameObject SpawnMarker;
        public GameObject EndMarker;
        public GameObject PathMarker;

        public virtual void Generate()
        {
            throw new NotImplementedException("Level generator needs a generation definition.");
        }

        public void BuildMarker(Vector3 position, CellType markerType)
        {
            switch(markerType)
            {
                case CellType.End_Cell:
                    Instantiate(EndMarker, position, new Quaternion());
                    return;
                case CellType.Spawn_Cell:
                    Instantiate(SpawnMarker, position, new Quaternion());
                    return;
                case CellType.Main_Path_Cell:
                    Instantiate(PathMarker, position, new Quaternion());
                    return;
                case CellType.Cell:
                default:
                    Instantiate(Marker, position, new Quaternion());
                    return;
            }
        }
    }
}
