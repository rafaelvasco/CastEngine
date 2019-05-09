using System;

namespace CastFramework
{
    public static class RandomExtensions
    {
        public static float NextFloat(this Random rnd)
        {
            return (float)rnd.NextDouble();
        }

        public static float Choose(this Random rnd, float start, float end)
        {
            return start + (end - start) * (float)rnd.NextDouble();
        }
    }
}
