using Assets.Scripts.Misc;
using Assets.Scripts.Painter_Generation.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Painter_Generation
{
    public abstract class CellRegion : MonoBehaviour
    {
        public bool infiniteSize;

        public int mainPathLength;

        //TODO: Region Theme

        public CellCollection cells = new CellCollection();

        public Vector3 regionBounds; //0,0,0 is the -x -z corner of the cell region

        public Cell lastCellGenerated;


        public CellRegion()
        {
            //if (requiredRooms.Any()) mainPathLength += requiredRooms.Sum(x => x.cellSize);
        }

        public bool InBounds(Vector3 position)
        {
            if (infiniteSize) return true;

            return true;
        }

        public bool CellIsHere(Vector3 location)
        {
            return cells.collection.Any(x => x.Key == location);
        }

        public Cell GetStartCell()
        {
            return cells.collection.First(x => x.Value.tags.Contains(Tags.CELL_START)).Value;
        }

        public Cell GetEndCell()
        {
            return cells.collection.First(x => x.Value.tags.Contains(Tags.CELL_END)).Value;
        }
    }
}
