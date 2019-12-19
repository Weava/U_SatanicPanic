using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Painter_Generation
{
    public class Cell : MonoBehaviour
    {
        public List<string> tags = new List<string>();
        public Vector3 position = new Vector3();

        public Cell(Vector3 initPosition)
        {
            position = initPosition;
        }

        public Cell(Vector3 initPosition, List<string> initTags)
        {
            position = initPosition;
            tags.AddRange(initTags);
        }
    }
}
