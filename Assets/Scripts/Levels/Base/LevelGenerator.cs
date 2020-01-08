using Assets.Scripts.Generation.Painter.Cells;
using Assets.Scripts.Generation.Painter.Cells.Base;
using Assets.Scripts.Misc;
using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Levels.Base
{
    public abstract class LevelGenerator : MonoBehaviour
    {
        public GameObject Marker;
        public GameObject InitMarker;

        public virtual bool BuildLevel()
        {
            throw new NotImplementedException("A level generator instance must be defined to build level.");
        }

        public void RenderMarkers()
        {
            foreach(var cell in CellCollection.collection.Select(s => s.Value))
            {
                var name = "";
                foreach(var tag in cell.tags)
                {
                    name += tag + " ";
                }

                if(cell.tags.Contains(Tags.CELL_PATH))
                {
                    name += (cell as PathCell).pathSequence;
                }

                if(cell.tags.Contains(Tags.INIT_PATH))
                {
                    var instance = Instantiate(InitMarker, cell.position, new Quaternion());
                    instance.name = name;
                } else
                {
                    var instance = Instantiate(Marker, cell.position, new Quaternion());
                    instance.name = name;
                }
            }
        }
    }
}
