//using System;
//using Assets.Scripts.Levels.Generation.Base;
//using Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding;
//using Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding.Base;
//using Assets.Scripts.Misc.Extensions;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//namespace Assets.Scripts.Levels.Generation.Rendering.Suites.Types
//{
//    /// <summary>
//    /// Takes a scaffold in a room and fills it with the smallest units possible. Usually used to fill in spaces that larger suites do not fill.
//    /// </summary>
//    public class Suite_Fill : Suite
//    {
//        [Header("Entities")]
//        #region Suite Entities
//        //List of possible entity types that can be randomly picked from to fill in a room

//        //Floor
//        public List<SuiteEntity> Floor_Mains = new List<SuiteEntity>();
//        public List<SuiteEntity> Floor_Connectors = new List<SuiteEntity>();
//        public List<SuiteEntity> Floor_Columns = new List<SuiteEntity>();

//        //Wall
//        public List<SuiteEntity> Wall_Mains = new List<SuiteEntity>();
//        public List<SuiteEntity> Wall_Connectors = new List<SuiteEntity>();

//        //Ceiling
//        public List<SuiteEntity> Ceiling_Mains = new List<SuiteEntity>();
//        public List<SuiteEntity> Ceiling_Connectors = new List<SuiteEntity>();
//        public List<SuiteEntity> Ceiling_Columns = new List<SuiteEntity>();

//        #endregion

//        void Awake()
//        {
//            type = SuiteType.Fill;
//        }

//        public override bool IsValid()
//        {
//            return true; //Suite Fill is the minimum a room can use to fill in its scaffold, there is no way it can be 'wrong'
//        }

//        public override void Render()
//        {
//            var scaffoldInstance = Level.roomScaffolds[targetRoom.id];
//            var scaffoldNodes = scaffoldInstance.GetFlattenedNodes().Where(x => !x.claimed).ToList();

//            if (scaffoldNodes.Count == 0)
//            {
//                return;

//            }

//            FetchRoomRenderContainer();

//            #region Floor pass
//            if (scaffoldNodes.Any(x => x.type == ScaffoldNodeType.Floor_Main))
//            {

//                var floorMain = Floor_Mains.Random();

//                foreach (var node in scaffoldNodes.Where(x => x.type == ScaffoldNodeType.Floor_Main))
//                {
//                    var instance = Instantiate(floorMain, node.position, new Quaternion());
//                    instance.transform.parent = roomInstanceContainer.transform;
//                    scaffoldInstance.SetNodeClaimed(node.id);
//                }
//            }

//            if (scaffoldNodes.Any(x => x.type == ScaffoldNodeType.Floor_Connector))
//            {
//                var floorConnector = Floor_Connectors.Random();

//                foreach (var node in scaffoldNodes.Where(x => x.type == ScaffoldNodeType.Floor_Connector))
//                {
//                    var instance = Instantiate(floorConnector, node.position, new Quaternion());
//                    instance.transform.LookAt(node.rootCells.First().position);
//                    instance.transform.parent = roomInstanceContainer.transform;
//                    scaffoldInstance.SetNodeClaimed(node.id);
//                }
//            }

//            if (scaffoldNodes.Any(x => x.type == ScaffoldNodeType.Floor_Column))
//            {
//                var floorColumn = Floor_Columns.Random();

//                foreach (var node in scaffoldNodes.Where(x => x.type == ScaffoldNodeType.Floor_Column))
//                {
//                    var instance = Instantiate(floorColumn, node.position, new Quaternion());
//                    instance.transform.parent = roomInstanceContainer.transform;
//                    scaffoldInstance.SetNodeClaimed(node.id);
//                }
//            }
//            #endregion

//            #region Wall pass
//            if (scaffoldNodes.Any(x => x.type == ScaffoldNodeType.Wall_Main))
//            {
//                var wallMain = Wall_Mains.Random();

//                foreach (var node in scaffoldNodes.Where(x => x.type == ScaffoldNodeType.Wall_Main))
//                {
//                    var instance = Instantiate(wallMain, node.position, new Quaternion());
//                    instance.transform.LookAt(node.rootCells.First().position);
//                    instance.transform.parent = roomInstanceContainer.transform;
//                    scaffoldInstance.SetNodeClaimed(node.id);
//                }
//            }

//            if (scaffoldNodes.Any(x => x.type == ScaffoldNodeType.Wall_Connector))
//            {
//                var wallConnector = Wall_Connectors.Random();

//                foreach (var node in scaffoldNodes.Where(x => x.type == ScaffoldNodeType.Wall_Connector))
//                {
//                    var instance = Instantiate(wallConnector, node.position, new Quaternion());
//                    instance.transform.LookAt(node.offsetRoot);
//                    instance.transform.parent = roomInstanceContainer.transform;
//                    scaffoldInstance.SetNodeClaimed(node.id);
//                }
//            }
//            #endregion

//            #region Ceiling pass
//            if (scaffoldNodes.Any(x => x.type == ScaffoldNodeType.Ceiling_Main))
//            {
//                var ceilingMain = Ceiling_Mains.Random();

//                foreach (var node in scaffoldNodes.Where(x => x.type == ScaffoldNodeType.Ceiling_Main))
//                {
//                    var instance = Instantiate(ceilingMain, node.position, new Quaternion());
//                    instance.transform.parent = roomInstanceContainer.transform;
//                    scaffoldInstance.SetNodeClaimed(node.id);
//                }
//            }

//            if (scaffoldNodes.Any(x => x.type == ScaffoldNodeType.Ceiling_Connector))
//            {
//                var ceilingConnector = Ceiling_Connectors.Random();

//                foreach (Node_CeilingConnector node in scaffoldNodes.Where(x => x.type == ScaffoldNodeType.Ceiling_Connector))
//                {
//                    var instance = Instantiate(ceilingConnector, node.root.position, new Quaternion());
//                    instance.transform.parent = roomInstanceContainer.transform;
//                    instance.transform.LookAt(node.rootCells.First().position);
//                    instance.transform.position = node.position;
//                    scaffoldInstance.SetNodeClaimed(node.id);
//                }
//            }

//            if (scaffoldNodes.Any(x => x.type == ScaffoldNodeType.Ceiling_Column))
//            {
//                var ceilingColumn = Ceiling_Columns.Random();

//                foreach (var node in scaffoldNodes.Where(x => x.type == ScaffoldNodeType.Ceiling_Column))
//                {
//                    var instance = Instantiate(ceilingColumn, node.position, new Quaternion());
//                    instance.transform.parent = roomInstanceContainer.transform;
//                    scaffoldInstance.SetNodeClaimed(node.id);
//                }
//            }
//            #endregion
//        }

//        public override void Init()
//        {
//            type = SuiteType.Fill;
//            id = Guid.NewGuid().ToString();
//        }
//    }
//}
