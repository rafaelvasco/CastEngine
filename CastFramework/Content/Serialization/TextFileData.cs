using System;

namespace CastFramework
{
    [Serializable]
    public class TextFileData : ResourceData
    {
        public string[] TextData;

        public override ResourceDataType Type => ResourceDataType.Text;
    }
}
