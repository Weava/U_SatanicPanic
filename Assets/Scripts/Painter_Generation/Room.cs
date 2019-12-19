using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Painter_Generation
{
    public abstract class Room : MonoBehaviour
    {
        //Cells that are encompassed by this room block
        public List<Cell> cells;
        public int cellSize; //Number of cells contained in a room
    }
}
