using System.Collections.Generic;

namespace CastFramework { 

    public class StyleProps
    {
        public uint FontSize;

        public Dictionary<byte, int> BorderWidth;
        public Dictionary<byte, uint> BorderColor;
        public Dictionary<byte, uint> BackColor;
        public Dictionary<byte, uint> Color;

    }


    public class GuiStyle
    {
        public Dictionary<string, StyleProps> BaseStyles;
        public Dictionary<string, StyleProps> CustomStyles;
    }
}
