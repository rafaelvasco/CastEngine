using System;

namespace CastFramework
{
    [Serializable]
    public class FontData : ResourceData
    {
        public PixmapData FontSheet;
        public Rect[] GlyphRects;
        public float[] PreSpacings;
        public float[] PostSpacings;

        public override ResourceDataType Type => ResourceDataType.Font;
    }
}
