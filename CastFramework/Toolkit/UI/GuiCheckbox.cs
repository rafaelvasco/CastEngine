using System;

namespace CastFramework
{
    public class GuiCheckbox : GuiControl
    {
        public event EventHandler<bool> OnCheck;

        public string Label
        {
            get => label;
            set
            {
                label = value;
                Resize(CheckboxSize.W + label.Length * 8 + 2 * Padding, h);
                
            }
        }
        public bool Checked
        {
            get => is_checked;
            set
            {
                is_checked = value;
                Gui.InvalidateVisual();
            }
        }
        
        public Size CheckboxSize { get; set; } = new Size(10, 10);

        public int Padding { get; set; } = 5;
            
        internal GuiCheckbox(Gui gui, GuiContainer parent) : base(gui, parent)
        {
            label = "Check Me";
            
            w = CheckboxSize.W + label.Length * 8 + 2 * Padding; // TODO:
            h = CheckboxSize.H + 2 * Padding;

            FixedSize = true;
        }

        internal override void Update(GuiMouseState mouseState)
        {
            if (this.ContainsPoint(mouseState.MouseX, mouseState.MouseY))
            {
                this.Hovered = true;

                if (!is_checked)
                {
                    if (mouseState.MouseLeftDown && !this.Active)
                    {
                        this.Active = true;
                        Gui.InvalidateVisual();
                    }
                    else if (!mouseState.MouseLeftDown && this.Active)
                    {
                        this.is_checked = true;
                        OnCheck?.Invoke(this, true);
                        this.Active = false;
                        Gui.InvalidateVisual();
                    }
                }
                else
                {
                    if (mouseState.MouseLeftDown && !this.Active)
                    {
                        this.Active = true;
                        Gui.InvalidateVisual();
                    }
                    else if (!mouseState.MouseLeftDown && this.Active)
                    {
                        this.is_checked = false;
                        OnCheck?.Invoke(this, false);
                        this.Active = false;
                        Gui.InvalidateVisual();
                    }
                }

            }
            else
            {
                if (Active)
                {
                    this.is_checked = !this.is_checked;
                    this.Active = false;
                    Gui.InvalidateVisual();
                }
            }
        }

        internal override void Draw(Canvas canvas, GuiTheme theme)
        {
            theme.DrawCheckBox(canvas, this);
        }
        
        private bool is_checked;

        private string label;
    }
}