namespace CastFramework
{
    public class HorizontalContainer : DirectionalContainer
    {
   
        
        internal HorizontalContainer(Gui gui, GuiContainer parent) : base(gui, parent) {}

        protected override void ProcessDocking()
        {
        }

        internal override void DoAutoSize()
        {
            if (children.Count == 0)
            {
                return;
            }
            
            int maxChildH = 0;
            int totalWidth = 0;

            for (int i = 0; i < children.Count; i++)
            {
                var control = children[i];
                
                totalWidth += control.W;
                
                if (control.H > maxChildH)
                {
                    maxChildH = control.H;
                }
            }

            this.w = Calc.Max(this.w, totalWidth + 2 * Padding + (children.Count - 1) * ItemSpacing);
            this.h = Calc.Max(this.h, maxChildH + 2 * Padding);

        }

        internal override void DoLayout()
        {
            int length = children.Count;

            if (length == 0)
            {
                return;
            }

            int total_width = 0;
            int max_height = 0;

            for (int i = 0; i < length; i++)
            {
                total_width += children[i].W;

                if (children[i].H > max_height)
                {
                    max_height = children[i].H;
                } 
            }

            total_width += (length - 1) * ItemSpacing;
            
            if (total_width > this.W - 2 * Padding)
            {
                total_width = this.W - 2 * Padding;
            }

            if (max_height > this.H - 2 * Padding)
            {
                max_height = this.H - 2 * Padding;
            }

            int mediam_width = (total_width - (length - 1) * ItemSpacing) / length;

            for (int i = 0; i < length; i++)
            {
                var widget = children[i];
                
                int newX = widget.X, newY = widget.Y, newW = widget.W, newH = widget.H;

                switch (AlignHorizontal)
                {
                    case HAlignment.Left:

                        newX = this.Padding;
                        
                        newW = Calc.Min(widget.W, mediam_width);
                        
                        if (i > 0)
                        {
                            newX = children[i - 1].X + children[i - 1].W + ItemSpacing;
                        }
                        
                        break;
                    case HAlignment.Center:

                        newX = this.W / 2 - total_width/2;
                        
                        newW = Calc.Min(widget.W, mediam_width);

                        if (i > 0)
                        {
                            newX = children[i - 1].X + children[i - 1].W + ItemSpacing;
                        }
                        
                        break;
                    
                    case HAlignment.Right:

                        newX = this.W - total_width - this.Padding;
                        
                        newW = Calc.Min(widget.W, mediam_width);

                        if (i > 0)
                        {
                            newX = children[i - 1].X + children[i - 1].W + ItemSpacing;
                        }
                        
                        break;
                    
                    case HAlignment.Stretch:

                        widget.LayoutW = (this.W - 2 * Padding - (children.Count - 1) * ItemSpacing) / children.Count;
                        
                        newW = !widget.FixedSize ? widget.LayoutW : Calc.Min(widget.W, mediam_width);

                        if (i == 0)
                        {
                            newX = this.Padding;
                        }
                        else
                        {
                            newX = children[i - 1].X + children[i - 1].LayoutW + ItemSpacing;
                        }
                        
                        break;
                }

                switch (AlignVertical)
                {
                    case VAlignment.Top:

                        newH = Calc.Min(widget.H, max_height);
                        
                        newY = this.Padding;
                        
                        
                        break;
                    case VAlignment.Center:

                        newH = Calc.Min(widget.H, max_height);
                        
                        newY = this.H / 2 - newH / 2; 
                        
                        break;
                    case VAlignment.Bottom:
                        
                        newH = Calc.Min(widget.H, max_height);
                        
                        newY = this.H - newH - this.Padding;
                        
                        break;

                      
                    case VAlignment.Stretch:

                        newY = this.Padding;

                        if (!widget.FixedSize)
                        {
                            newH = this.H - 2 * Padding;
                        }
                        else
                        {
                            newH = Calc.Min(newH, max_height);
                        }

                        break;
                }

                widget.SetPosition(newX, newY);
                widget.Resize(newW, newH);
            }
            
        }
    }
}