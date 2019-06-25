namespace CastFramework
{
    public abstract class Scene
    {
        public static Game Game { get; internal set; }
        public static ContentManager Content { get; internal set; }
        public static Canvas Canvas { get; internal set; }

        public bool KeepContentOnMemory { get;set; } = false;

        public abstract void Load();
        public virtual void Unload() { }
        public virtual void Init() { }
        public virtual void End() { }
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(Canvas canvas, GameTime gameTime);
        public virtual void OnCanvasResize(int width, int height) { }
    }

    public class EmptyScene : Scene
    {
        public override void Load()
        {
        }

        public override void Init()
        {
        }

        public override void End()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(Canvas canvas, GameTime gameTime)
        {
        }
    }
}
