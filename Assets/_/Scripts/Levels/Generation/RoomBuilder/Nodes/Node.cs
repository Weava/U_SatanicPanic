using System;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation.RoomBuilder.Nodes
{
    public class Node
    {
        public string id;

        public bool claimed;

        public Node()
        {
            id = Guid.NewGuid().ToString();
        }

        public Node(Vector3 position)
        {
            id = Guid.NewGuid().ToString();
            this.position = position;
        }

        public Vector3 position;
    }
}