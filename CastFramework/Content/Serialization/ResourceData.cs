using System;

namespace CastFramework
{
    [Serializable]
    public abstract class ResourceData
    {
        public string Id;

        public abstract ResourceDataType Type { get;}
    }
}
