using CastFramework;

namespace CastDemo
{
    public class HelloWorld : Scene
    {
        private Texture2D texture;
        private Sprite sprite;

        public override void Init()
        {
        }

        public override void Load()
        {
            texture = Content.Get<Texture2D>("logo");
            sprite = new Sprite(texture);
            sprite.SetOrigin(0.5f, 0.5f);

        }

        public override void End()
        {
        }

        public override void Update(GameTime gameTime)
        {
            if(Input.KeyPressed(Key.Escape))
            {
                Game.Quit();
            }
        }

        public override void Draw(Canvas canvas, GameTime gameTime)
        {
            sprite.Draw(canvas, Canvas.Width/2, Canvas.Height/2);
        }
    }
}
