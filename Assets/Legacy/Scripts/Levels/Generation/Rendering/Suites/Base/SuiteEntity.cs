using Assets.Scripts.Levels.Generation.Extensions;
using Assets.Scripts.Levels.Generation.RoomBuilder;
using System.Collections.Generic;
using System.Linq;
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

        public RoomTypeEnum roomType;

        public List<Vector4> spaces = new List<Vector4>();

        public List<Vector4> walls = new List<Vector4>();

        public List<Vector4> doors = new List<Vector4>();

        //Only applies if a partial entity
        public List<Vector3> ceilings = new List<Vector3>();

        public bool renderDebug = false;

        #endregion Rendering Rules

        #region Meta Properties

        [HideInInspector]
        public int entitySize
        {
            get
            {
                if (partial) return 0;
                return spaces.Count;
            }
        }

        #endregion Meta Properties

        private void Start()
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

            foreach (var openSpace in spaces)
            {
                result.spaces.Add(new Vector3(
                    position.x + openSpace.x * Cellf.CELL_STEP_OFFSET,
                    position.y + openSpace.y * Cellf.CELL_STEP_OFFSET,
                    position.z + openSpace.z * Cellf.CELL_STEP_OFFSET).ProjectOffsetToNormal(normal));
            }

            return result;
        }

        public SuiteProjection BuildWallProjection(Vector3 position = new Vector3(), Direction normal = Direction.North)
        {
            var result = new SuiteProjection();

            foreach (var wall in walls)
            {
                result.spaces.Add(new Vector4(
                    position.x + wall.x * Cellf.CELL_STEP_OFFSET,
                    position.y + wall.y * Cellf.CELL_STEP_OFFSET,
                    position.z + wall.z * Cellf.CELL_STEP_OFFSET,
                     wall.w).ProjectOffsetToNormal(normal));
            }

            return result;
        }

        #endregion Helper Methods

        #region DrawGizmos

        private void OnDrawGizmos()
        {
            RenderDebug();
        }

        private void RenderDebug()
        {
            if (renderDebug)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawCube(new Vector3(), new Vector3(1, 1, 1)); //Root

                foreach (var space in spaces)
                {
                    DrawCell(space);
                }

                //foreach (var space in blockedSpaces)
                //{
                //    RenderCellSpace(space, true);
                //}

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

        private void RenderCellSpace(Vector4 position)
        {
            //position = ProjectToCellSpace(position);

            //var floorOffset = Cellf.CELL_MAIN_OFFSET / 2.0f;
            //var heightOffset = Cellf.CELL_ELEVATION_OFFSET;

            //var wireFrame_floor = new List<Vector3>()
            //{
            //    new Vector3(position.x + floorOffset, position.y, position.z + floorOffset),
            //    new Vector3(position.x + floorOffset, position.y, position.z - floorOffset),
            //    new Vector3(position.x - floorOffset, position.y, position.z - floorOffset),
            //    new Vector3(position.x - floorOffset, position.y, position.z + floorOffset),
            //};

            //var wireFrame_elevated = new List<Vector3>()
            //{
            //    new Vector3(position.x + floorOffset, position.y + heightOffset, position.z + floorOffset),
            //    new Vector3(position.x + floorOffset, position.y + heightOffset, position.z - floorOffset),
            //    new Vector3(position.x - floorOffset, position.y + heightOffset, position.z - floorOffset),
            //    new Vector3(position.x - floorOffset, position.y + heightOffset, position.z + floorOffset),
            //};

            //var fullLineList = wireFrame_elevated;
            //fullLineList.AddRange(wireFrame_floor);

            //for (int i = 0; i < 3; i++)
            //{
            //    Gizmos.DrawLine(wireFrame_floor[i], wireFrame_elevated[i]);
            //    Gizmos.DrawLine(wireFrame_floor[i], wireFrame_floor[i + 1]);
            //    Gizmos.DrawLine(wireFrame_elevated[i], wireFrame_elevated[i + 1]);
            //}

            //Gizmos.DrawLine(wireFrame_floor[3], wireFrame_floor[0]);
            //Gizmos.DrawLine(wireFrame_elevated[3], wireFrame_elevated[0]);
            //Gizmos.DrawLine(wireFrame_floor[3], wireFrame_elevated[3]);

            //Gizmos.DrawLine(wireFrame_floor[0], wireFrame_floor[2]);
        }

        private void RenderWallSpace(Vector4 root, bool door = false)
        {
            Color color = Color.clear;
            if (door)
            { color = Color.green; }
            else
            { color = Color.yellow; }

            var rootBase = ProjectToCellSpace(root);
            root.x = rootBase.x;
            root.y = rootBase.y;
            root.z = rootBase.z;

            DrawWall(root, ((int)root.w).ToDirection(), color);
        }

        private static void DrawWall(Vector3 root, Direction normal, Color color)
        {
            var floorOffset = Cellf.CELL_MAIN_OFFSET / 2.0f;
            var heightOffset = Cellf.CELL_ELEVATION_OFFSET;

            List<Vector3> wireFrame;
            switch (normal)
            {
                case Direction.East:
                    wireFrame = new List<Vector3>()
                    {
                        new Vector3(root.x + floorOffset, root.y, root.z + floorOffset),
                        new Vector3(root.x + floorOffset, root.y, root.z - floorOffset),
                        new Vector3(root.x + floorOffset, root.y + heightOffset, root.z - floorOffset),
                        new Vector3(root.x + floorOffset, root.y + heightOffset, root.z + floorOffset)
                    };
                    break;

                case Direction.South:
                    wireFrame = new List<Vector3>()
                    {
                        new Vector3(root.x + floorOffset, root.y, root.z - floorOffset),
                        new Vector3(root.x - floorOffset, root.y, root.z - floorOffset),
                        new Vector3(root.x - floorOffset, root.y + heightOffset, root.z - floorOffset),
                        new Vector3(root.x + floorOffset, root.y + heightOffset, root.z - floorOffset)
                    };
                    break;

                case Direction.West:
                    wireFrame = new List<Vector3>()
                    {
                        new Vector3(root.x - floorOffset, root.y, root.z - floorOffset),
                        new Vector3(root.x - floorOffset, root.y, root.z + floorOffset),
                        new Vector3(root.x - floorOffset, root.y + heightOffset, root.z + floorOffset),
                        new Vector3(root.x - floorOffset, root.y + heightOffset, root.z - floorOffset)
                    };
                    break;

                default: //North
                    wireFrame = new List<Vector3>()
                    {
                        new Vector3(root.x - floorOffset, root.y, root.z + floorOffset),
                        new Vector3(root.x + floorOffset, root.y, root.z + floorOffset),
                        new Vector3(root.x + floorOffset, root.y + heightOffset, root.z + floorOffset),
                        new Vector3(root.x - floorOffset, root.y + heightOffset, root.z + floorOffset)
                    };
                    break;
            }

            Gizmos.color = color;

            for (int i = 0; i < 3; i++)
            {
                Gizmos.DrawLine(wireFrame[i], wireFrame[i + 1]);
            }

            Gizmos.DrawLine(wireFrame[3], wireFrame[0]);
            Gizmos.DrawLine(wireFrame[0], wireFrame[2]);
        }

        private static void DrawCell(Vector4 position)
        {
            position = ProjectToCellSpace(position);
            var normals = ((int)position.w).GetDirectionsFromByte();

            foreach (var direction in Directionf.Directions())
            {
                DrawWall(position, direction, normals.Contains(direction) ? Color.red : Color.cyan);
            }
        }

        private static Vector4 ProjectToCellSpace(Vector4 offset)
        {
            return new Vector4(
                offset.x * Cellf.CELL_STEP_OFFSET,
                offset.y * Cellf.CELL_ELEVATION_OFFSET,
                offset.z * Cellf.CELL_STEP_OFFSET,
                offset.w
                );
        }

        #endregion DrawGizmos
    }

    public class SuiteProjection
    {
        public List<Vector4> spaces = new List<Vector4>();

        public List<Vector3> spacesAsVec3 => spaces.Select(s => (Vector3)s).ToList();
    }
}