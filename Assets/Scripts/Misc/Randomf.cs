using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Misc
{
    public static class Randomf
    {
        /// <summary>
        /// Shuffle method for lists that uses the Unity Random Method to captitalize on the game's seed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> Shuffle<T>(this List<T> list)
        {
            var result = new List<T>();
            var temp = list;

            while (temp.Any())
            {
                var next = temp[UnityEngine.Random.Range(0, temp.Count)];
                result.Add(next);
                temp.Remove(next);
            }

            return result;
        }
    }
}