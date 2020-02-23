using Assets.Scripts.Generation.Extensions;
using Assets.Scripts.Generation.Painter.Cells.Base;
using Assets.Scripts.Generation.Painter.Cells.Factory;
using Assets.Scripts.Generation.Painter.Rooms;
using Assets.Scripts.Generation.Painter.Rooms.Base;
using Assets.Scripts.Generation.RoomBuilding;
using Assets.Scripts.Generation.RoomBuilding.Base;
using Assets.Scripts.Generation.RoomBuilding.Suite.Base;
using Assets.Scripts.Levels.Base;
using Assets.Scripts.Misc;
using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Levels.Demo_0
{
    public class Demo_0_Generator : LevelGenerator
    {
        public RoomSuite basementRoomSuite;

        public int basementLength;
        private string basementTag = "Region_Basement";

        //public int floor1Length;

        //public int floor2Length;

        //public int outsideLength;

        //public int arenaLength;

        private Cell nextRootCell;

        #region Monobehaviour

        private void Start()
        {
            BuildLevel();
        }

        #endregion

        public override void BuildLevel()
        {
            try
            {
                BuildBasementPath();
                BuildBasementCells();
                DecayBasementCells();
                CellBuilder.CleanUpIsolatedCells();
                BuildBasementRooms();

                //Floor 1

                //Floor 2

                //Outside

                //Arena

                CleanUnclaimedCells();
                RoomBuilder.BuildBlueprints();

                //TODO: delete/refactor this later
                SimpleRoomScaffolding.roomSuite = basementRoomSuite;           
                foreach (var room in RoomCollection.collection)
                {
                    RoomScaffold scaffold;
                    switch(room.roomSize)
                    {
                        case RoomSize.Room_1_1:
                            scaffold = Instantiate(Room_1_1_Scaffold, room.rootPosition + Room_1_1_Scaffold.rootNode.transform.position, Quaternion.Euler(new Vector3(0, room.orientation.RotationAngle(), 0)));
                            break;
                        case RoomSize.Room_1_2:
                            scaffold = Instantiate(Room_1_2_Scaffold, room.rootPosition + Room_1_2_Scaffold.rootNode.transform.position, Quaternion.Euler(new Vector3(0, room.orientation.RotationAngle(), 0)));
                            break;
                        case RoomSize.Room_2_2:
                            scaffold = Instantiate(Room_2_2_Scaffold, room.rootPosition + Room_2_2_Scaffold.rootNode.transform.position, Quaternion.Euler(new Vector3(0, room.orientation.RotationAngle(), 0)));
                            break;
                        case RoomSize.Room_2_3:
                            scaffold = Instantiate(Room_2_3_Scaffold, room.rootPosition + Room_2_3_Scaffold.rootNode.transform.position, Quaternion.Euler(new Vector3(0, room.orientation.RotationAngle(), 0)));
                            break;
                        case RoomSize.Room_3_3:
                            scaffold = Instantiate(Room_3_3_Scaffold, room.rootPosition + Room_3_3_Scaffold.rootNode.transform.position, Quaternion.Euler(new Vector3(0, room.orientation.RotationAngle(), 0)));
                            break;
                        case RoomSize.Room_4_4:
                            scaffold = Instantiate(Room_4_4_Scaffold, room.rootPosition + Room_4_4_Scaffold.rootNode.transform.position, Quaternion.Euler(new Vector3(0, room.orientation.RotationAngle(), 0)));
                            break;
                        default:
                            scaffold = null;
                            break;
                    }
                    scaffold.room = room;
                    scaffold.SetRoom();
                    SimpleRoomScaffolding.Scaffold(scaffold);
                }

                base.BuildLevel();
            } catch (Exception e)
            {
                throw new Exception("Level could not be generated.", e);
            }
        }

        private bool BuildBasementPath()
        {
             var result = PathBuilder.BuildPath(new Vector3(0, 0, 0), new PathOptions()
            {
                pathType = PathType.Straight_Line,

                primaryPathLength = basementLength,
                primaryDirection = Direction.North,
                
                Region = basementTag,
                Subregion = "Basement_1"
            });

            nextRootCell = result.Last();

            var direction = Random.Range(0, 2) == 1 ? Direction.West : Direction.East;

            result = PathBuilder.BuildPath(nextRootCell, new PathOptions()
            {
                pathType = PathType.Arched_Line,

                primaryPathLength = basementLength,
                secondaryPathLength = basementLength,

                primaryDirection = Direction.North,
                secondaryDirection = direction,

                Region = basementTag,
                Subregion = "Basement_2"
            });

            nextRootCell = result.Last();

            result = PathBuilder.BuildPath(nextRootCell, new PathOptions()
            {
                pathType = PathType.Curved_Line,
                primaryPathLength = basementLength,
                secondaryPathLength = basementLength,

                primaryDirection = Direction.South,
                secondaryDirection = direction,

                Region = basementTag,
                Subregion = "Basement_3"
            });

            //Elevation cell at this point
            nextRootCell = result.Last();

            //result = PathBuilder.BuildPath(nextRootCell, new PathOptions()
            //{
            //    pathType = PathType.Elevation,

            //    primaryDirection = Direction.South,
            //    elevationDirection = Direction.Up,

            //    tags = new Dictionary<string, string>() { [Tags.REGION] = basementTag, [Tags.SUBREGION] = "Basement_Stairs" }
            //});

            //nextRootCell = result.Last();

            return true;
        }

        private void BuildBasementCells()
        {
            var cells = CellCollection.collection.Where(x => x.Value.Region == basementTag).Select(s => s.Value).ToList();
            cells.Remove(CellCollection.collection
                .First(x => x.Value.tags.Contains(Tags.INIT_PATH) 
                && x.Value.Subregion == "Basement_1").Value);
            CellBuilder.Expand(cells, new CellOptions()
            {
                expansionAmount = 3
            });
        }

        private void DecayBasementCells()
        {
            CellBuilder.Decay(CellCollection.collection.Select(s => s.Value).ToList(), new CellOptions() { decayRate = 0.5f });
        }

        private void BuildBasementRooms()
        {
            var cells = CellCollection.collection.Where(x => x.Value.Region == basementTag).Select(s => s.Value).ToList();

            cells.ClaimRooms(ClaimType.SequencedGreedy, new RoomOptions() {
                Region = basementTag,
                excludeRoomSize = excludedRoomSizes
            });

            RoomBuilder.BuildPathContext(basementTag);
            RoomBuilder.BuildNonPathContext(basementTag, new RoomContextOptions() {
                generateAdditionalDoors = true,
                doorChance = 0.15f
            });
        }
    }
}
