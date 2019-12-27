using Assets.Scripts.Painter_Generation.CellRegions;
using Assets.Scripts.Painter_Generation.Cells.CellGenerators.Base;
using Assets.Scripts.Painter_Generation.Painters;
using Assets.Scripts.Painter_Generation.Rooms;
using Assets.Scripts.Painter_Generation.Rooms.Mappers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Painter_Generation.Cells.CellGenerators
{
    public class SimpleLinearCellGenerator : CellGenerator
    {
        public int paddingAmount;

        public InfiniteSizeRegion cellRegion;

        public LinearPainter mainPathPainter;

        /// <summary>
        /// Floof up the borders  of the main path with decorative cells
        /// </summary>
        public ExpandingPainter cellExpandingPainter;

        public RoomMapper roomMapper;

        private void Start()
        {
            Generate();
            Map();
        }

        public override void Generate()
        {
            mainPathPainter.region = cellRegion;
            mainPathPainter.PaintRegion();

            cellExpandingPainter.region = cellRegion;
            cellExpandingPainter.paddingAmount = paddingAmount;
            cellExpandingPainter.PaintRegion();

            foreach(var cell in cellRegion.cells.collection.Select(s => s.Value))
            {
                BuildMarker(cell.position, cell.cellType);
            }
        }

        public override void Map()
        {
            levelMap = new LevelMap();
            levelMap.regions.Add(new LevelRegion() { region = cellRegion });
            roomMapper.map = levelMap;
            roomMapper.Map();
            foreach(var regionRoomSet in levelMap.regions.Select(s => s.rooms).ToList())
            {
                foreach(var room in regionRoomSet)
                {
                    BuildRoomMarker(room.rootCell.position, room.roomDimensions, room.roomDirection);
                }
            }
        }
    }
}
