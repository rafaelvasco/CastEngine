using System;
using System.Collections.Generic;
using System.IO;
using Utf8Json;

namespace CastFramework
{
    public class ResourceManifest
    {
        public string Id;
    }

    [Serializable]
    public class ImageManifest : ResourceManifest
    {
        public string Path;
    }

    [Serializable]
    public class FontManifest : ResourceManifest
    {
        public string Path;
        public string ImagePath;
    }

    [Serializable]
    public class EffectManifest : ResourceManifest
    {
        public string Path;
    }

    [Serializable]
    public class SongManifest : ResourceManifest
    {
        public string Path;
    }

    [Serializable]
    public class ShaderManifest : ResourceManifest
    {
        public string VertexSrcPath;
        public string FragmentSrcPath;
    }

    [Serializable]
    public class TextFileManifest : ResourceManifest
    {
        public string Path;
    }

    [Serializable]
    public struct ContentGroup
    {
        public string Name;

        public Dictionary<string, ImageManifest> Images;
        public Dictionary<string, ShaderManifest> Shaders;
        public Dictionary<string, FontManifest> Fonts;
        public Dictionary<string, EffectManifest> Effects;
        public Dictionary<string, SongManifest> Songs;
        public Dictionary<string, TextFileManifest> TextFiles;
    }

    [Serializable]
    public class ContentManifest
    {
        public Dictionary<string, ContentGroup> Content;
    }

    
}
