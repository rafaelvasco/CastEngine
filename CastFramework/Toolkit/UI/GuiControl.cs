
using System;

namespace CastFramework
{
    public enum GuiControlState: byte
    {
        Normal,
        Hovered,
        Active
    }

    public abstract class GuiControl
    {

        public event EventHandler OnClick;
        public event EventHandler OnPressed;
        public event EventHandler OnReleased;

        public int X => x;

        public int Y => y;

        public int W => w;

        public int H => h;

        public int ZIndex { get;set;};

        public GuiDocking Docking
        {
            get => docking;
            set
            {
                if (docking != value)
                {
                    docking = value;
                    
                    Gui.InvalidateVisual();
                    Gui.InvalidateLayout();
                }
            }
        }

        public GuiContainer Parent { get; }

        public abstract string Class { get;}

        public abstract Size DefaultSize { get; }

        public bool DebugDraw { get;set;} = false;

        public string Id { get;set; }

        public int GlobalX => Parent?.GlobalX + X ?? X;
        public int GlobalY => Parent?.GlobalY + Y ?? Y;

        internal int LayoutW;

        internal int LayoutH;

        public GuiControlState State { get; internal set;} = GuiControlState.Normal;

        public bool Hovered { get; internal set;}

        public bool FixedSize { get; set; }

        public Rect BoundingRect => Rect.FromBox(GlobalX, GlobalY, W, H);

        protected GuiControl(Gui gui)
        {
            Gui = gui;
            Parent = null;

            w = DefaultSize.W;
            h = DefaultSize.H;
        }

        protected GuiControl(Gui gui, GuiContainer parent)
        {
            Gui = gui;
            Parent = parent;
        }

        public void SetPosition(int x, int y)
        {
            this.x = x;
            this.y = y;

            Gui.InvalidateVisual();
            Gui.InvalidateLayout();
        }

        public void Resize(int newW, int newH)
        {
            if (this.w == newW && this.h == newH) return;
            
            this.w = newW;
            this.h = newH;
            
            Gui.InvalidateVisual();
            Gui.InvalidateLayout();
        }

        public virtual bool ContainsPoint(int px, int py)
        {
            if (
                px < GlobalX ||
                py < GlobalY ||
                px > GlobalX + W ||
                py > GlobalY + H)
            {
                return false;
            }

            return true;
        }

        internal abstract void Draw(Canvas canvas, GuiStyle style);
        
        protected void DrawFrame(Canvas canvas, int x, int y, int w, int h, GuiStyle style)
        {
            StyleProps props = style.BaseStyles[Class];

            if(Id != null)
            {
                props = style.CustomStyles[Id];
            }

            canvas.FillRect(x, y, w, h, props.BackColor[(byte)State]);
            canvas.DrawRect(x - 1, y - 1, w + 1, h + 1, props.BorderColor[(byte)State]);
        }

        protected readonly Gui Gui;

        internal int x;
        internal int y;
        internal int w;
        internal int h;

        private GuiDocking docking = GuiDocking.None;
    }
}