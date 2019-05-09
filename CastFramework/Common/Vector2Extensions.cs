using System.Numerics;

namespace CastFramework
{
    public static class Vector2Extensions
    {
        public static Vector2 Normalized(this Vector2 vec)
        {
            if(vec.X == 0.0f && vec.Y == 0.0f)
            {
                return Vector2.Zero;
            }

            float val = 1.0f / vec.Length();

            return new Vector2(vec.X * val, vec.Y * val);
        }
    }
}
