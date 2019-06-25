using System;

namespace CastFramework
{
    public class GuiButton : GuiControl
    {

        public string Label { get; set; } = "Click Me";

        public override string Class => "BTN";

        public override Size DefaultSize => new Size(100, 30);

        internal override void Draw(Canvas canvas, GuiStyle style)
        {
            var x = this.GlobalX;
            var y = this.GlobalY;
            var w = this.W;
            var h = this.H;

            DrawFrame(canvas, x, y, w, h, style);
        }

        internal GuiButton(Gui gui, GuiContainer parent) : base(gui, parent)
        {
        }
    }
}