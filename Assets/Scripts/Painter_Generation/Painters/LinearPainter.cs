using Assets.Scripts.Painter_Generation.Painters.Base;

namespace Assets.Scripts.Painter_Generation.Painters
{
    public class LinearPainter : Painter
    {
        public bool mainPath;

        public override void PaintRegion()
        {
            BuildSpawnCell();
            BuildMainPath();
            BuildEndCell();
        }
    }
}
