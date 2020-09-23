using Assets.Scripts.Levels.Generation.Parts.ScaffoldNodes;
using System;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts.Levels.Generation.Parts
{
    public class RoomScaffold : MonoBehaviour
    {
        public string roomId;

        public List<Vector3> space = new List<Vector3>();

        [HideInInspector] public List<Vector3> worldSpace = new List<Vector3>();

        public List<DoorNode> doorNodes = new List<DoorNode>();

        public bool renderDebug = true;

        #region Spatial Methods

        public List<Vector3> GetWorldSpace()
        {
            var forwardVector = gameObject.transform.forward;
            var snappedRoot = SnapToGrid(gameObject.transform.position);

            var worldSpace = new List<Vector3>();
            foreach (var vector3 in space)
            {
                worldSpace.Add(ProjectToWorldSpace(snappedRoot, vector3, forwardVector));
            }

            return worldSpace;
        }

        public Vector3 ProjectToWorldSpace(Vector3 root, Vector3 position, Vector3 rootForward)
        {
            if (Mathf.Abs(rootForward.z) >= Mathf.Abs(rootForward.x)) //North / South
            {
                if (rootForward.z > 0) //North
                {
                    return new Vector3(root.x + position.x * CellF.CellOffset,
                        root.y + position.y * CellF.CellHeightOffset,
                        root.z + position.z * CellF.CellOffset);
                }
                else //South
                {
                    return new Vector3(root.x - position.x * CellF.CellOffset,
                        root.y + position.y * CellF.CellHeightOffset,
                        root.z - position.z * CellF.CellOffset);
                }
            }
            else // East / West
            {
                if (rootForward.x > 0) //East
                {
                    return new Vector3(root.x + position.z * CellF.CellOffset,
                        root.y + position.y * CellF.CellHeightOffset,
                        root.z - position.x * CellF.CellOffset);
                }
                else //West
                {
                    return new Vector3(root.x - position.z * CellF.CellOffset,
                        root.y + position.y * CellF.CellHeightOffset,
                        root.z + position.x * CellF.CellOffset);
                }
            }
        }

        public Vector3 SnapToGrid(Vector3 position)
        {
            var result = new Vector3();

            var xFloat = position.x - Math.Truncate(position.x);
            var yFloat = position.y - Math.Truncate(position.y);
            var zFloat = position.z - Math.Truncate(position.z);

            if (xFloat >= 0.5)
                result.x = (float)Math.Ceiling(position.x);
            else
                result.x = (float)Math.Floor(position.x);

            if (yFloat >= 0.5)
                result.y = (float)Math.Ceiling(position.y);
            else
                result.y = (float)Math.Floor(position.y);

            if (zFloat >= 0.5)
                result.z = (float)Math.Ceiling(position.z);
            else
                result.z = (float)Math.Floor(position.z);

            return result;
        }

        #endregion Spatial Methods

        #region Gizmo Render

        private void OnDrawGizmos()
        {
            if (renderDebug)
            {
                RenderSpace();
                RenderDoors();
            }
        }

        private void RenderSpace()
        {
            var worldSpace = GetWorldSpace();
            foreach (var space in worldSpace)
            {
                DrawCell(space);
            }
        }

        private void RenderDoors()
        {
            foreach (var doorNode in doorNodes)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(doorNode.transform.position, doorNode.transform.forward);
            }
        }

        private void DrawCell(Vector3 root)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(root, new Vector3(CellF.CellOffset, CellF.CellHeightOffset, CellF.CellOffset));
        }

        #endregion Gizmo Render
    }
}