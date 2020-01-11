using Assets.Scripts.Generation.Painter.Cells.Base;
using Assets.Scripts.Misc;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Generation.Painter.Cells.Base
{
    /// <summary>
    /// The level's cell collection, available for any metadata needs
    /// </summary>
    public static class CellCollection
    {
        public static Dictionary<Vector3, Cell> collection = new Dictionary<Vector3, Cell>();

        public static Cell Get(Vector3 position)
        {
            return collection[position];
        }

        public static void Add(Cell cell)
        {
            if (!collection.Any(x => x.Key == cell.position))
            {
                collection.Add(cell.position, cell);
            }
        }

        public static void AddRange(List<Cell> cells)
        {
            foreach(var cell in cells)
            {
                Add(cell);
            }
        }

        public static bool HasCellAt(Vector3 location)
        {
            return collection.Any(x => x.Key == location);
        }

        public static void ClearCellCollection()
        {
            collection = new Dictionary<Vector3, Cell>();
        }

        //public static Cell GetPathCellInSequence(string identifierTag, PathSequence sequence)
        //{
        //    var list = new List<string>() { Tags.CELL_PATH, identifierTag };

        //    switch (sequence)
        //    {
        //        case PathSequence.First:
        //            return collection.Where(x => list.All(y => x.Value.tags.Contains(y))).Select(s => s.Value)
        //                .OrderBy(o => (o as PathCell).pathSequence).ToList()
        //                .First();
        //        case PathSequence.Last:
        //            return collection.Where(x => list.All(y => x.Value.tags.Contains(y))).Select(s => s.Value)
        //                .OrderBy(o => (o as PathCell).pathSequence).ToList()
        //                .Last();
        //        default:
        //            return null;
        //    }
        //}

        //public static List<Cell> GetPathCellsByTag(string identifierTag)
        //{
        //    var list = new List<string>()     { Tags.CELL_PATH, identifierTag };
        //    return collection.Where(x => list.All(y => x.Value.tags.Contains(y))).Select(s => s.Value).ToList();
        //}

        //public static List<Cell> GetCellsByTag(string tag)
        //{
        //    return collection.Where(x => x.Value.tags.Contains(tag)).Select(s => s.Value).ToList();
        //}

        //public static List<Cell> GetCellsByTag(List<string> tags)
        //{
        //    return collection.Where(x => tags.All(y => x.Value.tags.Contains(y))).Select(s => s.Value).ToList();
        //}
    }

    public enum PathSequence
    {
        First,
        Last
    }
}
