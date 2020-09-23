using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Misc.Extensions
{
    public static class CollectionExtensions
    {
        public static T Random<T>(this List<T> collection) where T : class
        {
            return collection[UnityEngine.Random.Range(0, collection.Count)];
        }

        public static List<T> Shuffle<T>(this List<T> collection) where T : class
        {
            var result = new List<T>();

            var temp = collection;

            while (temp.Count > 0)
            {
                var element = temp.Random();
                result.Add(element);
                temp.Remove(element);
            }

            return result;
        }
    }
}
