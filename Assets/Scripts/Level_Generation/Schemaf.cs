using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Level_Generation
{
    /// <summary>
    /// Schema meta helper
    /// </summary>
    public static class Schemaf
    {
        public static Room GetFurthestRoomFromRoot(this LevelSchema schema, bool mustBeAvailable = true)
        {
            Room farthestRoom = null;
            int stackCount = 0;
            int farthestStackCount = 0;
            Room currentRoom = null;

            var endRooms = schema.GetSchemaRooms().Where(x => x.IsEndRoom()).ToList();

            if (endRooms.Count() == 1) return endRooms.First();

            endRooms.ForEach(x => {
                currentRoom = x;
                while (currentRoom != null && currentRoom != schema.GetRootRoom())
                {
                    currentRoom = currentRoom.parentRoom;
                    stackCount++;
                }

                if(stackCount > farthestStackCount)
                {
                    farthestStackCount = stackCount;
                    stackCount = 0;
                    farthestRoom = x;
                }
            });

            return farthestRoom;
        }

        public static Bounds GetBoundsOfLevel(this List<LevelSchema> schemas)
        {
            Bounds result = new Bounds(new Vector3(), new Vector3());

            foreach(var schema in schemas)
            {
                result.Encapsulate(schema.GetBoundsOfRoomCollection());
            }

            return result;
        }

        #region Cardinal Room Search

        public static Room GetFurthestRoomInDirection(Direction direction, Bounds levelBounds)
        {
            switch(direction)
            {
                case Direction.East:
                    return GetEastMostRoom(levelBounds);
                case Direction.South:
                    return GetSouthMostRoom(levelBounds);
                case Direction.West:
                    return GetWestMostRoom(levelBounds);
                case Direction.North:
                default:
                    return GetNorthMostRoom(levelBounds);
            }
        }

        public static Room GetNorthMostRoom(Bounds levelBounds)
        {
            var center = levelBounds.center;
            var extent = levelBounds.extents;

            Room farthestRoom = null;

            var scanPosition = center + new Vector3(-extent.x, 0, extent.z);
            var intervalAmount = 8;
            var intervals = extent.x * 2 * 8; //Aproximately how many rooms are visible from the North projection

            RaycastHit hit;
            for (int i = 0; i < intervals; i++)
            {
                var hasHit = Physics.Raycast(scanPosition, new Vector3(0, 0, -1), out hit, extent.z * 2);
                if (hasHit && hit.collider.gameObject.tag == "Room")
                {
                    Debug.DrawLine(scanPosition, hit.point, Color.red);

                    if(farthestRoom == null 
                        || farthestRoom.transform.position.z < hit.collider.transform.position.z)
                    {
                        farthestRoom = hit.collider.gameObject.GetComponent<Room>();
                    }
                }

                scanPosition += new Vector3(intervalAmount, 0, 0);
            }

            return farthestRoom;
        }

        public static Room GetEastMostRoom(Bounds levelBounds)
        {
            var center = levelBounds.center;
            var extent = levelBounds.extents;

            Room farthestRoom = null;

            var intervalAmount = 8f;

            //Direction dependent
            var scanPosition = center + new Vector3(extent.x, 0, extent.z);
            var intervals = extent.z * 2 * 8f; //Aproximately how many rooms are visible from the East projection

            RaycastHit hit;
            for (int i = 0; i < intervals; i++)
            {
                var hasHit = Physics.Raycast(scanPosition, new Vector3(-1, 0, 0), out hit, extent.x * 2);
                if (hasHit && hit.collider.gameObject.tag == "Room")
                {
                    Debug.DrawLine(scanPosition, hit.point, Color.red);

                    if (farthestRoom == null
                        || farthestRoom.transform.position.x < hit.collider.transform.position.x)
                    {
                        farthestRoom = hit.collider.gameObject.GetComponent<Room>();
                    }
                }

                scanPosition += new Vector3(0, 0, -intervalAmount);
            }

            return farthestRoom;
        }

        public static Room GetSouthMostRoom(Bounds levelBounds)
        {
            var center = levelBounds.center;
            var extent = levelBounds.extents;

            Room farthestRoom = null;

            var intervalAmount = 8;

            //Direction dependent
            var scanPosition = center + new Vector3(extent.x, 0, -extent.z);
            var intervals = extent.x * 2 * 8; //Aproximately how many rooms are visible from the South projection

            RaycastHit hit;
            for (int i = 0; i < intervals; i++)
            {
                var hasHit = Physics.Raycast(scanPosition, new Vector3(0, 0, 1), out hit, extent.z * 2);
                if (hasHit && hit.collider.gameObject.tag == "Room")
                {
                    Debug.DrawLine(scanPosition, hit.point, Color.red);

                    if (farthestRoom == null
                        || farthestRoom.transform.position.z > hit.collider.transform.position.z)
                    {
                        farthestRoom = hit.collider.gameObject.GetComponent<Room>();
                    }
                }

                scanPosition += new Vector3(-intervalAmount, 0, 0);
            }

            return farthestRoom;
        }

        public static Room GetWestMostRoom(Bounds levelBounds)
        {
            var center = levelBounds.center;
            var extent = levelBounds.extents;

            Room farthestRoom = null;

            var intervalAmount = 8;

            //Direction dependent
            var scanPosition = center + new Vector3(-extent.x, 0, -extent.z);
            var intervals = extent.z * 2 * 8; //Aproximately how many rooms are visible from the West projection

            RaycastHit hit;
            for (int i = 0; i < intervals; i++)
            {
                var hasHit = Physics.Raycast(scanPosition, new Vector3(1, 0, 0), out hit, extent.x * 2);
                if (hasHit && hit.collider.gameObject.tag == "Room")
                {
                    Debug.DrawLine(scanPosition, hit.point, Color.red);

                    if (farthestRoom == null
                        || farthestRoom.transform.position.x < hit.collider.transform.position.x)
                    {
                        farthestRoom = hit.collider.gameObject.GetComponent<Room>();
                    }
                }

                scanPosition += new Vector3(0, 0, intervalAmount);
            }

            return farthestRoom;
        }

        #endregion
    }
}
