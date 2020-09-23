using Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Scaffolding.Base;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Levels.Generation.RoomBuilder.Nodes.Parsing.Base
{
    public class Parsing_Node : Node
    {
        public RoomType roomType;
        public List<PointOfInterest> pointsOfInterest = new List<PointOfInterest>();
    }

    public enum RoomType
    {
        EndRoom,    //Small room with only 1 entrance
        Connector,  //Small room with multiple entrances across multiple cells
        SideRoom,   //Medium room with a section of the room with no doors
        Arena,      //Large room with a small amount of doors
        LargeRoom,  //Generic Large Room
        Courtyard,  //Large room with multiple doors
        Unknown,
    }

    public class PointOfInterest
    {
        public string id;
        public List<Vector3> cells = new List<Vector3>();
        public List<Scaffold_Node> scaffoldNodes = new List<Scaffold_Node>();
        public bool hasDoors = false;
        public PointOfInterestType interestType;

        public PointOfInterest()
        {
            id = Guid.NewGuid().ToString();
        }
    }

    public enum PointOfInterestType
    {
        Open_Area,  //0 walls
        Side,       //1 wall
        Corner,     //2 walls, adjacent
        Hall,       //2 walls, sides
        Cap,        //3 walls

        //Compound points of interest - N is any number
        Hallway,                        //2 Caps with at least 1 door in each cell

        Long_Hallway,                   //2 Caps with at least 1 door in each, and 1 Hall
        T_Hallway,                      //3 Caps with at least 1 door in each, and 1 Side
        Cross_Hallway,                  //1 open area + 4 Caps with 1 door in each
        Side_Partition,                 //2 Sides/Corners, 0 doors
        Side_Partition_Long,            //2-3 Sides or 2 corners + 1 Side
        Side_Partition_Elbow,           //1 corner + 2 sides, 0 doors
        Side_Partition_Elbow_Enclosed,  //3 corners, 0 doors
        Small_Center_Piece,             //2 corners + 2 sides, N doors
        Medium_Center_Piece,            //1 open area + 7 other cells in room surrounding it, N doors
        Large_Center_Piece,             //1 open area + 9 other cells in room surrounding it, N doors

        //Override Point of Interest, for the most part, these configurations can't get any more compounded
        Endroom, //3 walls, 1 door,
    }
}