using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Levels.Generation.Base
{
    public static class Level
    {
        public static List<DoorNode> doors = new List<DoorNode>();
    }

    public class DoorNode
    {
        public Cell cell_1;
        public Cell cell_2;
    }
}
