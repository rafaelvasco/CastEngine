using CastFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace CastBuilder
{
    public static class ContentReloader
    {
        private static ContentManifest manifest;
        private static string content_path;
        private static Dictionary<string, ResourcePak> reloadCachePaks;
        private static Dictionary<string, string> resourcePakMap;
        private static Dictionary<string, ResourceManifest> resourceManifestMap;

        public static void Watch(string project_root_path)
        {
            reloadCachePaks = new Dictionary<string, ResourcePak>();
            resourcePakMap = new Dictionary<string, string>();
            resourceManifestMap = new Dictionary<string, ResourceManifest>();

            content_path = PathUtils.GetLocalPath(project_root_path, Constants.CONTENT_FOLDER);

            if (!Directory.Exists(content_path))
            {
                throw new Exception("Invalid Project : Missing Content Folder");
            }

            using (var stream = File.OpenRead(PathUtils.GetLocalPath(content_path, "content.json")))
            {
                manifest = Utf8Json.JsonSerializer.Deserialize<ContentManifest>(stream);
            }

            BuildLookupMaps();

            using (var watcher = new FileSystemWatcher())
            {
                watcher.Path = content_path;

                watcher.IncludeSubdirectories = true;

                watcher.NotifyFilter = NotifyFilters.LastWrite;

                watcher.Filter = "*.png";

                watcher.Changed += Watcher_Changed;

                watcher.EnableRaisingEvents = true;

                Console.WriteLine($"Now Watching Project on {project_root_path} for changes. Press 'q' to quit.");

                while (Console.Read() != 'q')
                {
                    Thread.Sleep(1);
                }
            }
        }

        private static void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Changed:
                    var timer = Stopwatch.StartNew();
                    ReloadResource(e.FullPath);
                    Console.WriteLine($"Reload Resource Took: {timer.Elapsed.TotalSeconds} sec.");
                    break;
            }
        }

        private static void ReloadResource(string full_path)
        {
            var res_id = Path.GetFileNameWithoutExtension(full_path);


            if(resourceManifestMap.TryGetValue(res_id, out _))
            {
                UpdateResourceOnPack(res_id);
            }
            else
            {

            }
          
        }

        private static void UpdateResourceOnPack(string res_id)
        {
            var res_manifest = resourceManifestMap[res_id];
            var res_pak_name = resourcePakMap[res_id];

            Console.WriteLine($"Reloading Resource: {res_id} on Pak : {res_pak_name}");

            if (!reloadCachePaks.TryGetValue(res_pak_name, out _))
            {
                var newPak = ContentLoader.LoadPak(content_path, res_pak_name);

                reloadCachePaks.Add(res_pak_name, newPak);
            }

            var pak = reloadCachePaks[res_pak_name];

            switch(res_manifest)
            {
                case ImageManifest r:

                    var path = PathUtils.GetLocalPath(content_path, res_pak_name, r.Path);

                    pak.Resources[r.Id] = ContentLoader.LoadPixmapData(path);

                    break;

                case ShaderManifest r:

                    var vs_path = PathUtils.GetLocalPath(content_path, res_pak_name, r.VertexSrcPath);
                    var fs_path = PathUtils.GetLocalPath(content_path, res_pak_name, r.FragmentSrcPath);

                    pak.Resources[r.Id] = ContentLoader.LoadShaderProgramData(vs_path, fs_path);

                    break;

                case FontManifest r:

                    var dsc_path = PathUtils.GetLocalPath(content_path, res_pak_name, r.Path);
                    var img_path = PathUtils.GetLocalPath(content_path, res_pak_name, r.ImagePath);
                    pak.Resources[r.Id] = ContentLoader.LoadFontData(dsc_path, img_path);

                    break;

                case EffectManifest r:

                    break; //TODO:

                case SongManifest r:

                    break; //TODO:

                case TextFileManifest r:

                    var txt_path = PathUtils.GetLocalPath(content_path, res_pak_name, r.Path);
                    pak.Resources[r.Id] = ContentLoader.LoadTextFileData(txt_path);

                    break;
            }

            pak.SaveToDisk(content_path);

            Console.WriteLine("Reloaded");
        }

        private static void BuildLookupMaps()
        {

            foreach(var group in manifest.Content)
            {
                foreach (var image in group.Value.Images)
                {
                    resourcePakMap.Add(image.Key, group.Key);
                    resourceManifestMap.Add(image.Key, image.Value);
                }

                foreach (var shader in group.Value.Shaders)
                {
                    resourcePakMap.Add(shader.Key, group.Key);
                    resourceManifestMap.Add(shader.Key, shader.Value);
                }

                foreach (var font in group.Value.Fonts)
                {
                    resourcePakMap.Add(font.Key, group.Key);
                    resourceManifestMap.Add(font.Key, font.Value);
                }

                foreach (var sfx in group.Value.Effects)
                {
                    resourcePakMap.Add(sfx.Key, group.Key);
                    resourceManifestMap.Add(sfx.Key, sfx.Value);
                }

                foreach (var song in group.Value.Songs)
                {
                    resourcePakMap.Add(song.Key, group.Key);
                    resourceManifestMap.Add(song.Key, song.Value);
                }

                foreach (var txt in group.Value.TextFiles)
                {
                    resourcePakMap.Add(txt.Key, group.Key);
                    resourceManifestMap.Add(txt.Key, txt.Value);
                }
            }

         

        }

      
    }
}
