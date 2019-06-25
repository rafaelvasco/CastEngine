using CastFramework;

namespace CastEditor
{
    public class Editor : Scene
    {

        private Gui gui;

        public override void Init()
        {
        }

        public override void Load()
        {
            gui = new Gui();
            gui.Main.Padding = 0;

            var topbar = gui.Main.AddPanel();
            topbar.Padding = 0;
            topbar.Resize(Canvas.Width, 30);
            
            var topbar_layout = topbar.AddHorizontalContainer();
            topbar_layout.Docking = GuiDocking.Center;
            topbar_layout.Padding = 0;
            topbar_layout.ItemSpacing = 0;

            topbar_layout.AlignVertical = VAlignment.Stretch;

            var file_button = topbar_layout.AddButton("File");
            var help_button = topbar_layout.AddButton("Help");

        }

        public override void End()
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (Input.KeyPressed(Key.Escape))
            {
                Game.Quit();
            }

            if(Input.KeyPressed(Key.F11))
            {
                Game.ToggleFullscreen();
            }
        }

        public override void Draw(Canvas canvas, GameTime gameTime)
        {
        }
    }
}
