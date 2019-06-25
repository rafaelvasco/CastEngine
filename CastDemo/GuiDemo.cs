using CastFramework;

namespace CastDemo
{
    public class GuiDemo : Scene
    {

        private Gui gui;

        public override void Load()
        {
            gui = new Gui();

            var panel = gui.Main.AddPanel();
            panel.Docking = GuiDocking.Center;

            var container = panel.AddVerticalContainer();

            container.Docking = GuiDocking.Center;

            container.AlignVertical = VAlignment.Stretch;
            container.AlignHorizontal = HAlignment.Stretch;

            var btn = container.AddButton("Click Me");
            var check = container.AddCheckbox();
            var slider = container.AddSlider(0, 100, 1, Orientation.Horizontal);


        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(Canvas canvas, GameTime gameTime)
        {
        }
     

      
    }
}
