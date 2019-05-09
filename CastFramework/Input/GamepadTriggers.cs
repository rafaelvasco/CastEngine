namespace CastFramework
{
    public struct GamePadTriggers
    {
        public float Left { get; }
        public float Right { get; }

        public GamePadTriggers(float left, float right)
        {
            Left = Calc.Clamp(left, 0f, 1f);
            Right = Calc.Clamp(right, 0f, 1f);
        }
    }
}
