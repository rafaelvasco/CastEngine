using System;
using System.Collections.Generic;
using System.IO;
using CastFramework;

namespace CastBuilder
{
    public static class ContentBuilder
    {
        public static void Build(string project_root_path)
        {
            var content_path = PathUtils.GetLocalPath(project_root_path, Constants.CONTENT_FOLDER);

            if (!Directory.Exists(content_path))
            {
                throw new Exception("Invalid Project : Missing Content Folder");
            }

            ContentManifest manifest;

            var manifest_path = PathUtils.GetLocalPath(content_path, "content.json");

            if (!File.Exists(manifest_path))
            {
                throw new Exception("Invalid Project : Missing content.json");
            }

            manifest = JsonIO.Load<ContentManifest>(manifest_path);

            var root_path = Path.GetDirectoryName(manifest_path);

            var paks = BuildPaks(root_path, manifest);

            foreach(var pak in paks)
            {
                pak.SaveToDisk(root_path);
            }
        }


        private static List<ResourcePak> BuildPaks(string root_path, ContentManifest manifest)
        {
            var paks = new List<ResourcePak>();

            foreach(var group in manifest.Content)
            {
                var pak = new ResourcePak(group.Key);

                foreach(var image in group.Value.Images)
                {
                    var path = new Uri(Path.Combine(root_path, group.Key, image.Value.Path)).LocalPath;

                    var pixmap_data = ContentLoader.LoadPixmapData(path);

                    pak.Resources.Add(image.Value.Id, pixmap_data);
                }
                foreach(var font in group.Value.Fonts)
                {
                    var descr_path = new Uri(Path.Combine(root_path, group.Key, font.Value.Path)).LocalPath;
                    var image_path = new Uri(Path.Combine(root_path, group.Key, font.Value.ImagePath)).LocalPath;

                    var font_data = ContentLoader.LoadFontData(descr_path, image_path);

                    pak.Resources.Add(font.Value.Id, font_data);
                }
                foreach(var shader in group.Value.Shaders)
                {
                    var vs_path = new Uri(Path.Combine(root_path, group.Key, shader.Value.VertexSrcPath)).LocalPath;
                    var fs_path = new Uri(Path.Combine(root_path, group.Key, shader.Value.FragmentSrcPath)).LocalPath;

                    var shader_data = ContentLoader.LoadShaderProgramData(vs_path, fs_path);

                    pak.Resources.Add(shader.Value.Id, shader_data);
                }
                foreach(var sfx in group.Value.Effects)
                {
                    //TODO:
                }
                foreach(var song in group.Value.Songs)
                {
                    //TODO:
                }
                foreach(var txt in group.Value.TextFiles)
                {
                    var txt_path = new Uri(Path.Combine(root_path, group.Key, txt.Value.Path)).LocalPath;

                    var txt_data = ContentLoader.LoadTextFileData(txt_path);

                    pak.Resources.Add(txt.Value.Id, txt_data);
                }
              
                paks.Add(pak);

            }

            return paks;
        }
    }
}
