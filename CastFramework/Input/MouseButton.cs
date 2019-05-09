using System;

namespace CastFramework
{
    [Flags]
    public enum MouseButton : byte
    {
        None = 0,
        Left = 1 << 1,
        Right = 1 << 2,
        Middle = 1 << 3
    }
}
