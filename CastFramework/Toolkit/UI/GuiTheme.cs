using System;

namespace CastFramework
{
    public abstract class GuiTheme
    {
        public abstract void DrawButton(Canvas canvas, GuiButton button);
        public abstract void DrawPanel(Canvas canvas, GuiPanel panel);
        public abstract void DrawSlider(Canvas canvas, GuiSlider slider);
        public abstract void DrawCheckBox(Canvas canvas, GuiCheckbox checkbox);
    }

    public class DefaultTheme : GuiTheme
    {

        public Color AccentColor = Color.DodgerBlue;
        public Color PanelFill = new Color(37,37,37);
        public Color PanelBorder = Color.Cyan;
        public Color ControlFill = new Color(77, 77, 77);
        public Color ControlActiveFill = Color.Cyan;
        public Color ControlOverFill = Color.DodgerBlue;
        public Color ControlBorder = Color.White;
        public Color ControlOverBorder = Color.Cyan;


        public DefaultTheme(Font font)
        {
            this.font = font;
        }
        
        private void DrawFrame(Canvas canvas, int x, int y, int w, int h, Color borderColor, Color fillColor)
        {
            canvas.FillRect(x , y, w, h, fillColor);
            canvas.DrawRect(x-1 , y-1 , w+1 , h+1, borderColor);
        }

        public override void DrawButton(Canvas canvas, GuiButton button)
        {
            var x = button.GlobalX;
            var y = button.GlobalY;
            var w = button.W;
            var h = button.H;

            var textSize = new Size(

                button.Label.Length * 8,
                8
            );
            
            var labelPosX = x + (w / 2 - textSize.W / 2);
            var labelPosY = y + (h / 2 - textSize.H / 2);

            if (!button.Active)
            {
                DrawFrame(
                    canvas, 
                    x, y, 
                    w, h, 
                    button.Hovered ? ControlOverBorder : ControlBorder, 
                    button.Hovered ? ControlOverFill : ControlFill);

                canvas.DrawText(font, labelPosX, labelPosY, button.Label, Color.White, 0.25f);
            }
            else
            {
                DrawFrame(
                    canvas, 
                    x, y+1, 
                    w, h, 
                    button.Hovered ? ControlOverBorder : ControlBorder, 
                    button.Hovered ? ControlOverFill : ControlFill);

                
                canvas.DrawText(font, labelPosX, labelPosY+1, button.Label, Color.White, 0.25f);
                
            }
            
        }

        public override void DrawPanel(Canvas gfx, GuiPanel panel)
        {
            var x = panel.GlobalX;
            var y = panel.GlobalY;
            var w = panel.W;
            var h = panel.H;

            DrawFrame(
                gfx,
                x, y,
                w, h,
                PanelBorder,
                PanelFill);

        }

        public override void DrawSlider(Canvas canvas, GuiSlider slider)
        {
            int x = slider.GlobalX;
            int y = slider.GlobalY;
            int w = slider.W;
            int h = slider.H;
           


            float valueFactor = (float)slider.Value / (slider.MaxValue - slider.MinValue);

            if (slider.Orientation == Orientation.Horizontal)
            {
                DrawFrame(
                    canvas,
                    x, y,
                    w, h,
                    ControlBorder,
                    ControlFill);


                int indicatorSize = (int)(valueFactor * w);

                indicatorSize = Calc.Clamp(indicatorSize, 0, w - 4);

                canvas.FillRect(x+2,y+2, indicatorSize, h-4, AccentColor);
                
                var label_x = x + w / 2 - 4;
                var label_y = y + h / 2 - 4;

                canvas.DrawText(font, label_x, label_y, slider.Value.ToString(), Color.White, 0.25f);

            }
            else
            {
                DrawFrame(
                    canvas,
                    x, y,
                    w, h,
                    ControlBorder,
                    ControlFill);

                int indicatorSize = (int)((float)slider.Value / (slider.MaxValue - slider.MinValue) * h);

                indicatorSize = Calc.Clamp(indicatorSize, 0, h - 4);

                canvas.FillRect(x+2, y+2, w - 3, indicatorSize, AccentColor);
                
                var label_x = x + w / 2 - 4;
                var label_y = y + h / 2 - 4;
                
                canvas.DrawText(font, label_x, label_y, slider.Value.ToString(), Color.White, 0.25f);
            }
        }

        public override void DrawCheckBox(Canvas canvas, GuiCheckbox checkbox)
        {
            var x = checkbox.GlobalX;
            var y = checkbox.GlobalY;
            var w = checkbox.W;
            var h = checkbox.H;
            var checkW = checkbox.CheckboxSize.W;
            var checkH = checkbox.CheckboxSize.H;
            var padding = checkbox.Padding;

            Color fillColor = !checkbox.Checked ? ControlFill : ControlActiveFill;
            
            canvas.DrawRect(x, y, w, h, Color.Cyan);
            
            if (!checkbox.Active)
            {
                DrawFrame(
                    canvas,
                    x + padding, y + padding,
                    checkW, checkH,
                    ControlBorder,
                    fillColor);
            }
            else
            {
                DrawFrame(
                    canvas,
                    x + padding, y + padding,
                    checkW, checkH,
                    ControlBorder,
                    fillColor);
            }
            
            canvas.DrawText(x + padding + checkW + padding, y + h/2 - 4, checkbox.Label, Color.White, 0.25f);
        }
        
        private readonly Font font;
    }
}