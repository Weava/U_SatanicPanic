using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Levels.Generation.Base;
using Assets.Scripts.Levels.Generation.Base.Mono;
using Assets.Scripts.Levels.Generation.Rendering.Suites.Base;
using UnityEngine;
using Assets.Scripts.Misc.Extensions;

namespace Assets.Scripts.Levels.Generation.Rendering.Suites
{
    /// <summary>
    /// Handles Suite rendering per region
    /// </summary>
    public static class SuiteRenderHandler
    {
        #region Rendering Metadata Properties

        public static List<Suite> GlobalSuites = new List<Suite>();
        public static Dictionary<string, List<Suite>> RegionSuites = new Dictionary<string, List<Suite>>();

        /// <summary>
        /// Actual collection of suites that will be attempted to render
        /// </summary>
        public static Dictionary<string, List<Suite>> RenderPool = new Dictionary<string, List<Suite>>();

        #endregion

        public static bool RenderLevelSuites()
        {
            Init(); //Setups render pool
            return RenderRegions();
        }

        #region Rendering Process

        private static void Init()
        {
            foreach (var region in RegionCollection.regions.Select(s => s.Value))
            {
                RegionSuites.Add(region.id, region.suites);
            }

            var globalPool = new List<Suite>();
            var regions = RegionCollection.regions.Select(s => s.Value).ToList();

            //Populate global suites into regions for render pool
            foreach (var globalSuite in GlobalSuites)
            {
                globalSuite.isGlobalSuite = true;

                if (globalSuite.noLimitToRenderCap)
                {
                    foreach (var region in regions)
                    {
                        if (RenderPool.ContainsKey(region.id))
                        {
                            RenderPool[region.id].Add(globalSuite);
                        }
                        else
                        {
                            RenderPool.Add(region.id, new List<Suite> { globalSuite });
                        }
                    }
                }
                else
                {

                    var timesToRender = Random.Range(globalSuite.required ? 1 : 0, globalSuite.renderAmountCap + 1);
                    for (int i = 0; i < timesToRender; i++)
                    {
                        var randomRegion = regions.Random();
                        if (RenderPool.ContainsKey(randomRegion.id))
                        {
                            RenderPool[randomRegion.id].Add(globalSuite);
                        }
                        else
                        {
                            RenderPool.Add(randomRegion.id, new List<Suite> {globalSuite});
                        }
                    }
                }
            }

            //Populate region suites into respective regions for render pool
            foreach (var region in RegionSuites.Select(s => s.Key))
            {
                foreach (var suite in RegionSuites[region])
                {
                    if (suite.noLimitToRenderCap)
                    {
                        if (RenderPool.ContainsKey(region))
                        {
                            RenderPool[region].Add(suite);
                        }
                        else
                        {
                            RenderPool.Add(region, new List<Suite> { suite });
                        }
                    }
                    else
                    {
                        var timesToRender = Random.Range(suite.required ? 1 : 0, suite.renderAmountCap + 1);
                        for (int i = 0; i < timesToRender; i++)
                        {
                            if (RenderPool.ContainsKey(region))
                            {
                                RenderPool[region].Add(suite);
                            }
                            else
                            {
                                RenderPool.Add(region, new List<Suite> {suite});
                            }
                        }
                    }

                    foreach (var overridingSuite in RenderPool[region].Where(x => x.overrideGlobalTag != "").ToList())
                    {
                        var globalSuiteToOverride = RenderPool[region].FirstOrDefault(x =>
                            x.isGlobalSuite && x.overrideGlobalTag == overridingSuite.overrideGlobalTag);
                        if (globalSuiteToOverride != null)
                        {
                            RenderPool[region].Remove(globalSuiteToOverride);
                        }
                    }
                }
            }
        }

        private static bool RenderRegions()
        {
            foreach (var regionId in RenderPool.Select(s => s.Key))
            {
                var regionSuites = RenderPool[regionId];
                foreach (var room in Level.Rooms.Select(s => s.Value).Where(x => x.regionId == regionId).ToList())
                {
                    //TODO: Order suites by bias preference per room
                    regionSuites = regionSuites.Shuffle();
                    foreach (var suite in regionSuites)
                    {
                        if (RenderRoom(room, suite)) //Successful suite render, remove from pools
                        {
                            if (!suite.noLimitToRenderCap)
                            {
                                RenderPool[regionId].Remove(suite);
                            }
                            suite.renderContainer = null;
                            break;
                        }
                    }
                }

                if (RenderPool[regionId].Any(x => x.required)) //Generation failed to render all required suites
                { return false; }
            }

            return true;
        }

        private static bool RenderRoom(LevelRoom room, Suite suite)
        {
            if (!suite.ValidateRoom(room)) return false; //Check if room even has a chance with this suite

            var success = suite.Build(room);
            suite.renderContainer.roomId = room.roomId;

            if (success)
            {
                room.renderContainer.name = suite.suiteName;
                suite.Render(ref room);
                suite.renderContainer.claimedScaffolds.ForEach(x => Level.roomScaffolds[room.roomId].SetNodeClaimed(x.id));
                room.SaveChanges();
            }

            return success;
        }

        #endregion
    }
}
