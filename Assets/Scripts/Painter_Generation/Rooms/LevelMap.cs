using Assets.Scripts.Generation.Painter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Painter_Generation.Rooms
{
    /// <summary>
    /// Represents the relationship between regions/cells and rooms
    /// </summary>
    public class LevelMap
    {
        public List<LevelRegion> regions = new List<LevelRegion>();
    }

    public class LevelRegion
    {
        public Region region;
        public List<Room> rooms = new List<Room>();
    }
}
