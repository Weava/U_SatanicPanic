using Assets.Scripts.Generation.Painter.Cells.Base;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Generation.Painter.Cells.Base
{
    public class CellCollection
    {
        public Dictionary<Vector3, Cell> collection = new Dictionary<Vector3, Cell>();

        public bool Add(Cell cell)
        {
            if (collection.Any(x => x.Key == cell.position)) return false;

            collection.Add(cell.position, cell);

            return true;
        }

        public bool HasCellAt(Vector3 location)
        {
            return collection.Any(x => x.Key == location);
        }
    }
}
