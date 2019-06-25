namespace CastFramework
{
    public abstract class GameComponent
    {
        public abstract void Update(GameTime time);
        public abstract void Draw(Canvas canvas);
    }
}
