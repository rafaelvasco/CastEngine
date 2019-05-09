using System.Collections.Generic;

namespace CastFramework
{
    public class TextFile : Resource
    {
        internal TextFile(List<string> text)
        {
            Text = text;
        }
        public List<string> Text { get; }

        internal override void Dispose()
        {
            Text.Clear();
        }
    }
}
