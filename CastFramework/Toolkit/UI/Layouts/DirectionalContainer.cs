namespace CastFramework
{
    public abstract class DirectionalContainer : GuiContainer
    {
        protected DirectionalContainer(Gui gui, int width, int height) : base(gui, width, height)
        {
        }

        protected DirectionalContainer(Gui gui, GuiContainer parent) : base(gui, parent)
        {
        }
        
        public int ItemSpacing
        {
            get => item_spacing;
            set
            {
                if (item_spacing != value)
                {
                    item_spacing = value;
                    Gui.InvalidateVisual();
                    Gui.InvalidateLayout();
                }
            }
        }

        public VAlignment AlignVertical
        {
            get => v_alignment;
            set
            {
                if (v_alignment != value)
                {
                    v_alignment = value;
                    
                    Gui.InvalidateVisual();
                    Gui.InvalidateLayout();
                }
            }
        }

        public HAlignment AlignHorizontal
        {
            get => h_alignment;
            set
            {
                if (h_alignment != value)
                {
                    h_alignment = value;
                    
                    Gui.InvalidateVisual();
                    Gui.InvalidateLayout();
                }
            }
        }
        
        protected int item_spacing = 10;
        protected VAlignment v_alignment = VAlignment.Top;
        protected HAlignment h_alignment = HAlignment.Left;

        
    }
}