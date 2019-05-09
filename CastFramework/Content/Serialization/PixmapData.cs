using System;

namespace CastFramework
{
    [Serializable]
    public class PixmapData : ResourceData
    {
        public byte[] Data;
        public int Width;
        public int Height;

        public override ResourceDataType Type => ResourceDataType.Image;
    }
}
