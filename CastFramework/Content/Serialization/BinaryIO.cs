using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace CastFramework
{
    public static class BinaryIO
    {
        public static byte[] Serialize(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            using (var memoryStream = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();

                binaryFormatter.Serialize(memoryStream, obj);

                var compressed = CompressBytes(memoryStream.ToArray());

                return compressed;
            }
        }

        public static T Deserialize<T>(string file_path)
        {
            var bytes = File.ReadAllBytes(file_path);

            return Deserialize<T>(bytes);
        }

        public static T Deserialize<T>(byte[] data)
        {
            using (var memoryStream = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();

                var decompressed = DecompressBytes(data);

                memoryStream.Write(decompressed, 0, decompressed.Length);

                memoryStream.Seek(0, SeekOrigin.Begin);

                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }

        private static byte[] CompressBytes(byte[] input)
        {
            byte[] compressedData;

            using (var outputStream = new MemoryStream())
            {
                using (var zip = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    zip.Write(input, 0, input.Length);
                }

                compressedData = outputStream.ToArray();
            }

            return compressedData;
        }

        private static byte[] DecompressBytes(byte[] input)
        {
            byte[] decompressedData;

            using (var outputStream = new MemoryStream())
            {
                using (var inputStream = new MemoryStream(input))
                {
                    using (var zip = new GZipStream(inputStream, CompressionMode.Decompress))
                    {
                        zip.CopyTo(outputStream);
                    }
                }

                decompressedData = outputStream.ToArray();
            }

            return decompressedData;
        }
    }
}
