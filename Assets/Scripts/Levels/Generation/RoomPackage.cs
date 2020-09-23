using Assets.Scripts.Levels.Generation.Parts;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation
{
    /// <summary>
    /// Container that holds variations of a single room type. Helps organize rooms into specific needs or pools
    /// </summary>
    public class RoomPackage : MonoBehaviour
    {
        public string packageName;

        //Removes this package from the generators pool when a room is fetched
        public bool useOnlyOne = false;

        //At least one of these rooms must exist in level
        public bool required = false;

        public List<Room> rooms = new List<Room>();
    }
}