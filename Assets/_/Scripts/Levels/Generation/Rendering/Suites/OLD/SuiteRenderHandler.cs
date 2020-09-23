//using System;
//using System.Collections.Generic;
//using Assets.Scripts.Levels.Generation.Base;
//using Assets.Scripts.Levels.Generation.Base.Mono;
//using Assets.Scripts.Levels.Generation.Rendering.Suites.Types;
//using Assets.Scripts.Misc.Extensions;
//using System.Linq;
//using UnityEngine;
//using Random = UnityEngine.Random;

//namespace Assets.Scripts.Levels.Generation.Rendering.Suites
//{
//    public static class SuiteRenderHandler
//    {
//        //Pass for assigning globally available suites that are required to render
//        public static void RenderGlobalRequired()
//        {
//            var globalRequired = Level.suiteCollection.Select(s => s.Value)
//                .Where(x => x.regionsAllowed.Count == RegionCollection.regions.Count && x.required && x.timesUsed == 0);

//            var roomsAcrossRegions = Level.roomData.Select(s => s.room).ToList().Shuffle();

//            foreach (var suite in globalRequired)
//            {
//                foreach (var room in roomsAcrossRegions)
//                {
//                    if (suite.type == SuiteType.Fill)
//                    {
//                        if (AttemptFillRenderPass(suite as Suite_Fill, room))
//                        {
//                            break;
//                        }
//                    } else if (suite.type == SuiteType.Static)
//                    {
//                        if (AttemptStaticRenderPass(suite as Suite_Static, room))
//                        {
//                            break;
//                        }
//                    }
//                    else
//                    {
//                        return;
//                    }
//                }
//            }

//            if (Level.suiteCollection.Any(a => a.Value.required && a.Value.timesUsed == 0))
//            {
//                throw new Exception("Some globally required suites did not render. Make sure the level has the requirements to render all of the suites.");
//            }
//        }

//        public static void RenderSuitesForRegions()
//        {
//            foreach (var region in RegionCollection.regions.Select(s => s.Value).ToList())
//            {
//                var suites = Level.suiteCollection.Select(s => s.Value).Where(x => x.regionsAllowed.Contains(region));
//                var regionRooms = Level.roomData.Where(x => x.room.regionId == region.id).Select(s => s.room);

//                //Required pass
//                var required = suites.Where(x => x.required && x.timesUsed == 0).ToList();
//                foreach (var suite in required)
//                {
//                    foreach (var room in regionRooms)
//                    {
//                        if (suite.type == SuiteType.Fill)
//                        {
//                            if (AttemptFillRenderPass(suite as Suite_Fill, room))
//                            {
//                                break;
//                            }
//                        }
//                        else if (suite.type == SuiteType.Static)
//                        {
//                            if (AttemptStaticRenderPass(suite as Suite_Static, room))
//                            {
//                                break;
//                            }
//                        }
//                        else
//                        {
//                            break;
//                        }
//                    }
//                }

//                if (Level.suiteCollection.Any(a => a.Value.required && a.Value.timesUsed == 0))
//                {
//                    throw new Exception("Some region required suites did not render. Make sure the level has the requirements to render all of the suites.");
//                }

//                //Priority pass
//                var remainingSuites = suites.Where(x => !x.required).ToList();
//                var suitePool = new Dictionary<int, List<string>>();
//                foreach (var remainingSuitePriority in remainingSuites) //Create Suite pool
//                {
//                    if(!suitePool.ContainsKey(remainingSuitePriority.priority))
//                    { suitePool.Add(remainingSuitePriority.priority, new List<string>());}

//                    var amount = remainingSuitePriority.SuiteChanceRollSuccess();
//                    for (int i = 0; i < amount; i++)
//                    {
//                        suitePool[remainingSuitePriority.priority].Add(remainingSuitePriority.id);
//                    }
//                }

//                if (suitePool.Keys.Any(x => x > 0))
//                {
//                    var priorityGroupings = suitePool.Keys.Where(x => x > 0).OrderByDescending(o => o);
//                    foreach (var priority in priorityGroupings)
//                    {
//                        var poolSubset = suitePool[priority];
//                        while (poolSubset.Any())
//                        {
//                            var suite = poolSubset.Random();
//                            foreach (var regionRoom in regionRooms)
//                            {
//                                if (AttemptRender(Level.suiteCollection[suite], regionRoom))
//                                {
//                                    break;
//                                }
//                            }

//                            poolSubset.Remove(suite);
//                        }
//                    }
//                }

//                //Priority 0 Pass
//                if (suitePool.Keys.Any(x => x == 0))
//                {
//                    var suitPool = suitePool[0].ToList().Shuffle();
//                    foreach (var suite in suitPool)
//                    {
//                        foreach (var regionRoom in regionRooms)
//                        {
//                            AttemptRender(Level.suiteCollection[suite], regionRoom);
//                        }
//                    }
//                }
//            }
//        }

//        public static bool AttemptRender(Suite suite, Room room)
//        {
//            if (!Level.Rooms.ContainsKey(room.id))
//            {
//                Level.Rooms[room.id] = new LevelRoom()
//                {
//                    roomId = room.id,
//                    renderContainer = new GameObject("Render Container")
//                };

//                Level.Rooms[room.id].renderContainer.transform.parent =
//                    LevelGeneratorBase.roomInstances.FirstOrDefault(x => x.name == room.id).transform;
//            }

//            if (suite.type == SuiteType.Fill)
//            {
//                if (AttemptFillRenderPass(suite as Suite_Fill, room))
//                {
//                    return true;
//                }
//            }
//            else if (suite.type == SuiteType.Static)
//            {
//                if (AttemptStaticRenderPass(suite as Suite_Static, room))
//                {
//                    return true;
//                }
//            }

//            return false;
//        }

//        public static bool AttemptStaticRenderPass(Suite_Static suite, Room room)
//        {
//            var cells = room.GetCells();

//            suite.targetRoom = room;

//            foreach (var root in cells)
//            {
//                suite.rootCell = root;
//                if (suite.IsValid())
//                {
//                    suite.Render();
//                    suite.SaveChanges();
//                    return true;
//                }
//            }

//            return false;
//        }

//        public static bool AttemptFillRenderPass(Suite_Fill suite, Room room)
//        {
//            suite.targetRoom = room;

//            if (suite.IsValid())
//            {
//                suite.Render();
//                suite.SaveChanges();
//            }
//            else
//            { return false; }

//            return true;
//        }

//        /// <summary>
//        /// Rolls for the chance that a suite renders successfully, out putting a total amount of successes which will be the population of a suite
//        /// </summary>
//        /// <param name="suite"></param>
//        /// <param name="repeats"></param>
//        /// <returns></returns>
//        public static int SuiteChanceRollSuccess(this Suite suite)
//        {
//            var attempts = 0;
//            var repeats = 0;
//            while (attempts < suite.frequencyCap)
//            {
//                var result = Random.Range(0, 100);
//                if (suite.chanceToRender >= result)
//                {
//                    repeats++;
//                    attempts++;
//                }
//                else //Terminate on fail
//                {
//                    return repeats;
//                }
//            }

//            return repeats;
//        }
//    }
//}
