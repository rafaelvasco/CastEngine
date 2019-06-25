namespace CastFramework
{
    public class VerticalContainer : DirectionalContainer
    {
        internal VerticalContainer(Gui gui, GuiContainer parent) : base(gui, parent)
        {
        }

        protected override void ProcessDocking()
        {
        }

        internal override void DoAutoSize()
        {
            if (children.Count == 0)
            {
                return;
            }
            
            int maxChildW = 0;
            int totalHeight = 0;

            for (int i = 0; i < children.Count; i++)
            {
                var control = children[i];
                
                totalHeight += control.H;
                
                if (control.W > maxChildW)
                {
                    maxChildW = control.W;
                }
            }

            this.w = Calc.Max(this.w, maxChildW + 2 * Padding);
            this.h = Calc.Max(this.h, totalHeight + 2 * Padding + (children.Count - 1) * ItemSpacing);
            
        }

        internal override void DoLayout()
        {
            
            int length = children.Count;

            if (length == 0)
            {
                return;
            }

            int total_height = 0;
            int max_width = 0;

            for (int i = 0; i < length; i++)
            {
                total_height += children[i].H;

                if (children[i].W > max_width)
                {
                    max_width = children[i].W;
                }
            }

            total_height += (length - 1) * ItemSpacing;
            
            if (total_height > this.H - 2 * Padding)
            {
                total_height = this.H - 2 * Padding;
            }

            if (max_width > this.W - 2 * Padding)
            {
                max_width = this.W - 2 * Padding;
            }

            int mediam_height = (total_height - (length - 1) * ItemSpacing) / length;

            for (int i = 0; i < length; i++)
            {
                var widget = children[i];
                
                int newX = widget.X, newY = widget.Y, newW = widget.W, newH = widget.H;

                switch (AlignHorizontal)
                {
                    case HAlignment.Left:

                        newW = Calc.Min(widget.W, max_width);
                        
                        newX = this.Padding;
                        
                        break;
                    case HAlignment.Center:
                        
                        newW = Calc.Min(widget.W, max_width);

                        newX = this.W / 2 - newW / 2; 
                            
                        
                        break;
                    case HAlignment.Right:
                        
                        newW = Calc.Min(widget.W, max_width);

                        newX = this.W - newW - this.Padding;
                        
                        break;
                    
                    case HAlignment.Stretch:

                        newX = this.Padding;

                        if (!widget.FixedSize)
                        {
                            newW = this.W - 2 * Padding;
                        }
                        else
                        {
                            newW = Calc.Min(widget.W, max_width);
                        }
                        
                        break;
                }

                switch (AlignVertical)
                {
                    case VAlignment.Top:

                        newY = this.Padding;

                        newH = Calc.Min(widget.H, mediam_height);
                        
                        if (i > 0)
                        {
                            newY = children[i - 1].Y + children[i - 1].H + ItemSpacing;
                        }
                        
                        break;
                    case VAlignment.Center:

                        newY = this.H / 2 - (total_height)/2;
                        
                        newH = Calc.Min(widget.H, mediam_height);

                        if (i > 0)
                        {
                            newY = children[i - 1].Y + children[i - 1].H + ItemSpacing;
                        }
                        
                        break;
                    case VAlignment.Bottom:

                        newY = this.H - total_height - this.Padding;
                        
                        newH = Calc.Min(widget.H, mediam_height);

                        if (i > 0)
                        {
                            newY = children[i - 1].Y + children[i - 1].H + ItemSpacing;
                        }
                        
                        break;
                    
                    case VAlignment.Stretch:

                        widget.LayoutH = (this.H - 2 * Padding - (children.Count - 1) * ItemSpacing) / children.Count;
                        
                        newH = !widget.FixedSize ? widget.LayoutH : Calc.Min(widget.H, mediam_height);

                        if (i == 0)
                        {
                            newY = this.Padding;
                        }
                        else
                        {
                            newY = children[i - 1].Y + children[i - 1].LayoutH + ItemSpacing;
                        }

                        break;
                }
                
                widget.SetPosition(newX, newY);
                widget.Resize(newW, newH);
            }
        }

    }
}