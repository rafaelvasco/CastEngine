using System;
using System.IO;

namespace CastBuilder
{
    public static class PathUtils
    {
        public static string GetLocalPath(string path)
        {
            return new Uri(path).LocalPath;
        }

        public static string GetLocalPath(string path1, string path2)
        {
            return new Uri(Path.Combine(path1, path2)).LocalPath;
        }

        public static string GetLocalPath(string path1, string path2, string path3)
        {
            return new Uri(Path.Combine(path1, path2, path3)).LocalPath;
        }

        public static string GetLocalPath(string path1, string path2, string path3, string path4)
        {
            return new Uri(Path.Combine(path1, path2, path3, path4)).LocalPath;
        }
    }
}
