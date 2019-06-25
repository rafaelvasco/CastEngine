
namespace CastFramework
{
    public class GuiPanel : GuiContainer
    {
        public override string Class => "PAN";

        internal override void Draw(Canvas canvas, GuiStyle style)
        {
            var x = this.GlobalX;
            var y = this.GlobalY;
            var w = this.W;
            var h = this.H;

            DrawFrame(canvas, x, y, w, h, style);

            base.Draw(canvas, style);
            
        }

        internal GuiPanel(Gui gui, GuiContainer parent) : base(gui, parent)
        {
        }
    }
}