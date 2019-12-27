namespace Assets.Scripts.Painter_Generation.Painters
{
    public class LinearPainter : Painter.Painters.Base.Painter
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
