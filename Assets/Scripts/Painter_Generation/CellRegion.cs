using Assets.Scripts.Misc;
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
        public int mainPathLength;

        //TODO: Region Theme

        public CellCollection cells = new CellCollection();
        public List<Room> requiredRooms = new List<Room>();

        protected Cell lastCellGenerated;

        public CellRegion()
        {
            if (requiredRooms.Any()) mainPathLength += requiredRooms.Sum(x => x.cellSize);
        }

        public bool CellHere(Vector3 location)
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
