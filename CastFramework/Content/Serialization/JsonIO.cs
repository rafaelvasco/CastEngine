using System.IO;
using Utf8Json;

namespace CastFramework
{
    public static class JsonIO
    {
        public static T Load<T>(string path)
        {
            using(var stream = File.OpenRead(path))
            {
                T obj = JsonSerializer.Deserialize<T>(stream);

                return obj;
            }
        }

        public static void Save<T>(T obj, string path)
        {
            File.WriteAllBytes(path,
                    JsonSerializer.PrettyPrintByteArray(JsonSerializer.Serialize(obj)));
        }
    }
}
