using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Levels.Generation.Extensions;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

namespace Assets.Scripts.Levels.Generation.Rendering.Suites.Base
{
    public class SuiteEntity : MonoBehaviour
    {
        #region Rendering Rules

        public float chanceToRenderAgain;

        public bool partial;

        //Treated as a cell space if not a partial, else becomes a floor partition
        public List<Vector3> openSpaces = new List<Vector3>();

        //Treated as a cell space
        public List<Vector3> blockedSpaces = new List<Vector3>();

        public List<Vector4> walls = new List<Vector4>();

        public List<Vector4> doors = new List<Vector4>();

        //Only applies if a partial entity
        public List<Vector3> ceilings = new List<Vector3>();

        public bool renderDebug = false;

        #endregion

        #region Meta Properties

        [HideInInspector] public int entitySize
        {
            get
            {
                if (partial) return 0;
                return openSpaces.Count + blockedSpaces.Count;
            }
        }

        #endregion

        void Start()
        {
            renderDebug = false;
        }

        #region Helper Methods

        /// <summary>
        /// Renders a normalized projection of this entity in space at root 0 with a normal of North
        /// </summary>
        /// <param name="normal"></param>
        /// <returns></returns>
        public SuiteProjection BuildProjection(Vector3 position = new Vector3(), Direction normal = Direction.North)
        {
            var result = new SuiteProjection();
            result.spaces[EntitySpaceType.open] = new List<Vector3>();
            result.spaces[EntitySpaceType.blocked] = new List<Vector3>();

            foreach (var openSpace in openSpaces)
            {
                result.spaces[EntitySpaceType.open].Add(new Vector3(
                    position.x + openSpace.x * Cellf.CELL_STEP_OFFSET,
                    position.y + openSpace.y * Cellf.CELL_STEP_OFFSET,
                    position.z + openSpace.z * Cellf.CELL_STEP_OFFSET).ProjectOffsetToNormal(normal));
            }

            foreach (var blockedSpace in blockedSpaces)
            {
                result.spaces[EntitySpaceType.blocked].Add(new Vector3(
                    position.x + blockedSpace.x * Cellf.CELL_STEP_OFFSET,
                    position.y + blockedSpace.y * Cellf.CELL_STEP_OFFSET,
                    position.z + blockedSpace.z * Cellf.CELL_STEP_OFFSET).ProjectOffsetToNormal(normal));
            }

            return result;
        }

        #endregion

        #region DrawGizmos

        void OnDrawGizmos()
        {
            RenderDebug();
        }

        private void RenderDebug()
        {
            if (renderDebug)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawCube(new Vector3(), new Vector3(1, 1, 1)); //Root

                foreach (var space in openSpaces)
                {
                    RenderCellSpace(space);
                }

                foreach (var space in blockedSpaces)
                {
                    RenderCellSpace(space, true);
                }

                foreach (var space in walls)
                {
                    RenderWallSpace(space);
                }

                foreach (var space in doors)
                {
                    RenderWallSpace(space, true);
                }
            }
        }

        private void RenderCellSpace(Vector3 position, bool blocked = false)
        {
            if (blocked)
            { Gizmos.color = Color.red; }
            else
            { Gizmos.color = Color.cyan; }

            position = ProjectToCellSpace(position);

            var floorOffset = Cellf.CELL_MAIN_OFFSET / 2.0f;
            var heightOffset = Cellf.CELL_ELEVATION_OFFSET;

            var wireFrame_floor = new List<Vector3>()
            {
                new Vector3(position.x + floorOffset, position.y, position.z + floorOffset),
                new Vector3(position.x + floorOffset, position.y, position.z - floorOffset),
                new Vector3(position.x - floorOffset, position.y, position.z - floorOffset),
                new Vector3(position.x - floorOffset, position.y, position.z + floorOffset),
            };

            var wireFrame_elevated = new List<Vector3>()
            {
                new Vector3(position.x + floorOffset, position.y + heightOffset, position.z + floorOffset),
                new Vector3(position.x + floorOffset, position.y + heightOffset, position.z - floorOffset),
                new Vector3(position.x - floorOffset, position.y + heightOffset, position.z - floorOffset),
                new Vector3(position.x - floorOffset, position.y + heightOffset, position.z + floorOffset),
            };

            var fullLineList = wireFrame_elevated;
            fullLineList.AddRange(wireFrame_floor);

            for (int i = 0; i < 3; i++)
            {
                Gizmos.DrawLine(wireFrame_floor[i], wireFrame_elevated[i]);
                Gizmos.DrawLine(wireFrame_floor[i], wireFrame_floor[i + 1]);
                Gizmos.DrawLine(wireFrame_elevated[i], wireFrame_elevated[i + 1]);
            }

            Gizmos.DrawLine(wireFrame_floor[3], wireFrame_floor[0]);
            Gizmos.DrawLine(wireFrame_elevated[3], wireFrame_elevated[0]);
            Gizmos.DrawLine(wireFrame_floor[3], wireFrame_elevated[3]);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(wireFrame_floor[0], wireFrame_floor[2]);
        }

        private void RenderWallSpace(Vector4 root, bool door = false)
        {
            if (door)
            { Gizmos.color = Color.green; }
            else
            { Gizmos.color = Color.yellow; }

            var rootBase = ProjectToCellSpace(root);
            root.x = rootBase.x;
            root.y = rootBase.y;
            root.z = rootBase.z;

            var floorOffset = Cellf.CELL_MAIN_OFFSET / 2.0f;
            var heightOffset = Cellf.CELL_ELEVATION_OFFSET;

            List<Vector3> wireFrame;
            switch (root.w)
            {
                case 1:
                    wireFrame = new List<Vector3>()
                    {
                        new Vector3(root.x + floorOffset, root.y, root.z + floorOffset),
                        new Vector3(root.x + floorOffset, root.y, root.z - floorOffset),
                        new Vector3(root.x + floorOffset, root.y + heightOffset, root.z - floorOffset),
                        new Vector3(root.x + floorOffset, root.y + heightOffset, root.z + floorOffset)
                    };
                    break;
                case 2:
                    wireFrame = new List<Vector3>()
                    {
                        new Vector3(root.x + floorOffset, root.y, root.z - floorOffset),
                        new Vector3(root.x - floorOffset, root.y, root.z - floorOffset),
                        new Vector3(root.x - floorOffset, root.y + heightOffset, root.z - floorOffset),
                        new Vector3(root.x + floorOffset, root.y + heightOffset, root.z - floorOffset)
                    };
                    break;
                case 3:
                    wireFrame = new List<Vector3>()
                    {
                        new Vector3(root.x - floorOffset, root.y, root.z - floorOffset),
                        new Vector3(root.x - floorOffset, root.y, root.z + floorOffset),
                        new Vector3(root.x - floorOffset, root.y + heightOffset, root.z + floorOffset),
                        new Vector3(root.x - floorOffset, root.y + heightOffset, root.z - floorOffset)
                    };
                    break;
                default:
                    wireFrame = new List<Vector3>()
                    {
                        new Vector3(root.x - floorOffset, root.y, root.z + floorOffset),
                        new Vector3(root.x + floorOffset, root.y, root.z + floorOffset),
                        new Vector3(root.x + floorOffset, root.y + heightOffset, root.z + floorOffset),
                        new Vector3(root.x - floorOffset, root.y + heightOffset, root.z + floorOffset)
                    };
                    break;
            }

            for (int i = 0; i < 3; i++)
            {
                Gizmos.DrawLine(wireFrame[i], wireFrame[i + 1]);
            }

            Gizmos.DrawLine(wireFrame[3], wireFrame[0]);
            Gizmos.DrawLine(wireFrame[0], wireFrame[2]);
        }

        private static Vector3 ProjectToCellSpace(Vector3 offset)
        {
            return new Vector3(
                offset.x * Cellf.CELL_STEP_OFFSET,
                offset.y * Cellf.CELL_ELEVATION_OFFSET,
                offset.z * Cellf.CELL_STEP_OFFSET
                );
        }


        #endregion
    }

    public class SuiteProjection
    {
        public Dictionary<EntitySpaceType, List<Vector3>> spaces = new Dictionary<EntitySpaceType, List<Vector3>>();

        public SuiteProjection()
        {
            spaces[EntitySpaceType.open] = new List<Vector3>();
            spaces[EntitySpaceType.blocked] = new List<Vector3>();
        }

        public List<Vector3> FlattenedSpace()
        {
            var result = new List<Vector3>();

            result.AddRange(spaces[EntitySpaceType.open]);
            result.AddRange(spaces[EntitySpaceType.blocked]);

            return result;
        }
    }

    public enum EntitySpaceType
    {
        open,
        blocked
    }
}
