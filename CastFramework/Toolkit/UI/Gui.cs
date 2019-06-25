using System;
using System.Collections.Generic;

namespace CastFramework
{
    internal class GuiMouseState
    {
        public int MouseX;
        public int MouseY;
        public int LastMouseX;
        public int LastMouseY;
        public bool MouseLeftDown;
        public bool MouseRightDown;
        public bool MouseMiddleDown;

        public void UpdatePosition(int x, int y)
        {
            LastMouseX = MouseX;
            LastMouseY = MouseY;

            MouseX = x;
            MouseY = y;
        }

        public bool Moved => MouseX != LastMouseX || MouseY != LastMouseY;
    }

    public enum Orientation : byte
    {
        Horizontal = 0,
        Vertical
    }

    public class Gui : GameComponent
    {
        public GuiContainer Main => root;
        
        public Gui()
        {
            var width = Game.Instance.Canvas.Width;
            var height = Game.Instance.Canvas.Height;
            
            Game.Instance.Platform.OnWinResized += OnWinResized;

            mouse_state = new GuiMouseState();

            theme = new DefaultTheme(Game.Instance.Canvas.DefaultFont);
            
            root = new GuiContainer(this, width, height);
            
            surface = Game.Instance.Canvas.AddSurface(Rect.FromBox(0, 0, width, height), "GuiSurface");

            Game.Instance.AddComponent(this);
        }

        private void OnWinResized(int arg1, int arg2)
        {
            layout_invalidated = true;
            visual_invalidated = true;

        }

        public void Resize(int width, int height)
        {
            root.Resize(width, height);
        }

        public override void Update(GameTime time)
        {
            mouse_state.UpdatePosition(Game.Instance.Canvas.MousePos.X, Game.Instance.Canvas.MousePos.Y);

            var leftDown = Input.MouseDown(MouseButton.Left);
            var middleDown = Input.MouseDown(MouseButton.Middle);
            var rightDown = Input.MouseDown(MouseButton.Right);

            mouse_state.MouseLeftDown = leftDown;
            mouse_state.MouseMiddleDown = middleDown;
            mouse_state.MouseRightDown = rightDown;

            
            foreach(var ctrl in this.control_list)
            {
                if (ctrl.ContainsPoint(mouse_state.MouseX, mouse_state.MouseY))
                {

                    if(this.hovered_ctrl == null)
                    {
                        ctrl.Hovered = true;
                        ctrl.State = GuiControlState.Hovered;

                        this.hovered_ctrl = ctrl;
                        InvalidateVisual();
                        return;
                    }
                    else
                    {
                        if(ctrl.ZIndex > this.hovered_ctrl.ZIndex)
                        {
                            this.hovered_ctrl.Hovered = false;
                            this.hovered_ctrl.State = GuiControlState.Normal;

                            ctrl.Hovered = true;
                            ctrl.State = GuiControlState.Hovered;
                            this.hovered_ctrl = ctrl;
                            InvalidateVisual();
                            return;
                        }
                    }


                }
              
            }




            if (layout_invalidated)
            {
                RecalculateSize(root);
                
                RecalculateLayout(root);

                layout_invalidated = false;
            }


        }

        public override void Draw(Canvas canvas)
        {
            if (visual_invalidated)
            {
                Console.WriteLine("Gui Redraw");
            
                canvas.SetSurface(surface);

                root.Draw(canvas, theme);

                canvas.SetSurface();

                visual_invalidated = false;
            }
            
        }

        internal void InvalidateVisual()
        {
            visual_invalidated = true;
        }

        internal void InvalidateLayout()
        {
            layout_invalidated = true;
        }

        internal void AddControl(GuiControl control)
        {
            control.ZIndex = this.control_list.Count;

            this.control_list.Add(control);

        }

        private void RecalculateSize(GuiContainer container)
        {
            for (int i = 0; i < container.children.Count; i++)
            {
                var control = container.children[i];

                if (control is GuiContainer containerChild)
                {
                    RecalculateSize(containerChild);
                }
            }
            
            container.DoAutoSize();
            
        }
        
        private void RecalculateLayout(GuiContainer container)
        {
            
            container.DoLayout();
            
            for (int i = 0; i < container.children.Count; i++)
            {
                var control = container.children[i];

                if (control is GuiContainer containerChild)
                {
                    RecalculateLayout(containerChild);
                }
            }
            
            
        }
        
        private readonly GuiMouseState mouse_state;
        private readonly GuiTheme theme;
        private readonly GuiContainer root;

        private GuiControl hovered_ctrl;

        private List<GuiControl> control_list = new List<GuiControl>(50);


        private bool layout_invalidated = true;
        private bool visual_invalidated = true;
        
        private readonly RenderSurface surface;


    }
}