using System;
using System.Collections.Generic;
using System.IO;

namespace CastFramework
{
    [Serializable]
    public class ResourcePak
    {
        public readonly string Name;

        public readonly Dictionary<string, ResourceData> Resources;

        public ResourcePak(string name)
        {
            Name = name;
            Resources = new Dictionary<string, ResourceData>();
        }

        public void SaveToDisk(string content_path)
        {
            var bytes = BinarySerializer.Serialize(this);
            File.WriteAllBytes(Path.Combine(content_path, this.Name + ".pak"), bytes);
        }
    }
}
