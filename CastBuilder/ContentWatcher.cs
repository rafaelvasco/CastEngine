using CastFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace CastBuilder
{
    public static class ContentWatcher
    {
        private static ContentManifest manifest;
        private static string content_path;
        private static Dictionary<string, ResourcePak> reloadCachePaks;
        private static Dictionary<string, string> resourcePakMap;
        private static Dictionary<string, ResourceManifest> resourceManifestMap;
        private static List<string> newResourcesToAdd;

        public static void Watch(string project_root_path)
        {
            reloadCachePaks = new Dictionary<string, ResourcePak>();
            resourcePakMap = new Dictionary<string, string>();
            resourceManifestMap = new Dictionary<string, ResourceManifest>();
            newResourcesToAdd = new List<string>();

            content_path = PathUtils.GetLocalPath(project_root_path, Constants.CONTENT_FOLDER);

            if (!Directory.Exists(content_path))
            {
                throw new Exception("Invalid Project : Missing Content Folder");
            }

            LoadContentManifest();

            ContentBuilder.Build(project_root_path);

            using (var watcher = new FileSystemWatcher())
            {
                watcher.Path = content_path;

                watcher.IncludeSubdirectories = true;

                watcher.NotifyFilter = 
                    NotifyFilters.LastWrite | 
                    NotifyFilters.FileName | 
                    NotifyFilters.DirectoryName;

                watcher.Filter = "*.*";

                watcher.Changed += Watcher_Changed;
                watcher.Deleted += Watcher_Changed;
                watcher.Renamed += Watcher_Renamed;

                watcher.EnableRaisingEvents = true;

                ConsoleUtils.ShowInfo($"Now Watching Project on {project_root_path} for changes. Press 'q' to quit.");

                while (Console.Read() != 'q')
                {
                    Thread.Sleep(1);
                }
            }
        }

        private static void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            try
            {
                var extension = Path.GetExtension(e.FullPath);

                switch (extension)
                {
                    case ".png":
                    case ".vs":
                    case ".fs":
                    case ".wav":
                    case ".ogg":
                    case ".txt":

                        var new_res_id = Path.GetFileNameWithoutExtension(e.FullPath);
                        var old_res_id = Path.GetFileNameWithoutExtension(e.OldFullPath);

                        RemoveResourceOnPak(old_res_id);
                        UpdateResourceOnManifest(new_res_id, old_res_id, e.FullPath);
                        UpdateResourceOnPak(new_res_id);

                        break;
                }
            }
            catch(Exception ex)
            {
                ConsoleUtils.ShowError(ex.Message);
            }

            
        }

        private static void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                var extension = Path.GetExtension(e.FullPath);

                switch (extension)
                {
                    case ".png":
                    case ".vs":
                    case ".fs":
                    case ".wav":
                    case ".ogg":
                    case ".txt":

                        switch (e.ChangeType)
                        {
                            case WatcherChangeTypes.Changed:

                                var timer = Stopwatch.StartNew();

                                var res_id = FindResource(e.FullPath);

                                if (res_id == null)
                                {
                                    return;
                                }

                                UpdateResourceOnPak(res_id);

                                ConsoleUtils.ShowInfo($"Reload Resource Took: {timer.Elapsed.TotalSeconds} sec.");
                                break;

                            case WatcherChangeTypes.Deleted:

                                var timer_remove = Stopwatch.StartNew();

                                var res_id_remove = FindResource(e.FullPath);

                                if (res_id_remove == null)
                                {
                                    throw new Exception($"Tried to remove content that is not in the Content Manifest: {e.FullPath}");
                                }

                                bool removed = RemoveResourceOnPak(res_id_remove);

                                if (removed)
                                {
                                    RemoveResourceOnManifest(res_id_remove);

                                    ConsoleUtils.ShowInfo($"Remove Resource Took: {timer_remove.Elapsed.TotalSeconds} sec.");
                                }
                                else
                                {
                                    throw new Exception($"Resource {res_id_remove} was not removed from Pak. It is not present in it,");
                                }

                                break;

                        }

                        break;

                    case ".json":

                        if (e.ChangeType == WatcherChangeTypes.Changed && e.Name == "content.json")
                        {
                            LoadContentManifest();
                        }

                        break;
                }
            }
            catch(Exception ex)
            {
                ConsoleUtils.ShowError(ex.Message);
            }

            
        }

        private static void LoadContentManifest()
        {
            ContentManifest newManifest = JsonIO.Load<ContentManifest>(PathUtils.GetLocalPath(content_path, "content.json"));
            
            bool reloading_manifest = false;

            if(manifest != null)
            {
                reloading_manifest = true;
                StageNewResources(newManifest);
            }

            manifest = newManifest;

            FillContentMapsFromCurrentManifest();

            if(reloading_manifest)
            {

                foreach(var new_res_id in newResourcesToAdd)
                {
                    ConsoleUtils.ShowInfo($"Found new resource to Add: {new_res_id}");
                    UpdateResourceOnPak(new_res_id);
                }

                newResourcesToAdd.Clear();
            }
            
        }

        private static void SaveContentManifest()
        {
            JsonIO.Save(manifest, PathUtils.GetLocalPath(content_path, "content.json"));
        }

        private static void StageNewResources(ContentManifest newManifest) 
        {
            foreach(var new_group in newManifest.Content)
            {
                if(!manifest.Content.TryGetValue(new_group.Key, out _)) {

                    foreach(var img_manifest in new_group.Value.Images)
                    {
                        newResourcesToAdd.Add(img_manifest.Key);
                    }

                    foreach(var fnt_manifest in new_group.Value.Fonts)
                    {
                        newResourcesToAdd.Add(fnt_manifest.Key);
                    }

                    foreach(var sha_manifest in new_group.Value.Shaders)
                    {
                        newResourcesToAdd.Add(sha_manifest.Key);
                    }

                    foreach(var sfx_manifest in new_group.Value.Effects)
                    {
                        newResourcesToAdd.Add(sfx_manifest.Key);
                    }

                    foreach(var sng_manifest in new_group.Value.Songs)
                    {
                        newResourcesToAdd.Add(sng_manifest.Key);
                    }

                    foreach(var txt_manifest in new_group.Value.TextFiles)
                    {
                        newResourcesToAdd.Add(txt_manifest.Key);
                    }

                }
                else
                {

                    foreach(var img_manifest in new_group.Value.Images)
                    {
                        if(!manifest.Content[new_group.Key].Images.TryGetValue(img_manifest.Key, out _))
                        {
                            newResourcesToAdd.Add(img_manifest.Key);
                        }
                    }

                    foreach (var fnt_manifest in new_group.Value.Fonts)
                    {
                        if (!manifest.Content[new_group.Key].Fonts.TryGetValue(fnt_manifest.Key, out _))
                        {
                            newResourcesToAdd.Add(fnt_manifest.Key);
                        }
                    }

                    foreach (var sha_manifest in new_group.Value.Shaders)
                    {
                        if (!manifest.Content[new_group.Key].Shaders.TryGetValue(sha_manifest.Key, out _))
                        {
                            newResourcesToAdd.Add(sha_manifest.Key);
                        }
                    }

                    foreach (var sfx_manifest in new_group.Value.Effects)
                    {
                        if (!manifest.Content[new_group.Key].Effects.TryGetValue(sfx_manifest.Key, out _))
                        {
                            newResourcesToAdd.Add(sfx_manifest.Key);
                        }
                    }

                    foreach (var sng_manifest in new_group.Value.Songs)
                    {
                        if (!manifest.Content[new_group.Key].Songs.TryGetValue(sng_manifest.Key, out _))
                        {
                            newResourcesToAdd.Add(sng_manifest.Key);
                        }
                    }

                    foreach (var txt_manifest in new_group.Value.Songs)
                    {
                        if (!manifest.Content[new_group.Key].TextFiles.TryGetValue(txt_manifest.Key, out _))
                        {
                            newResourcesToAdd.Add(txt_manifest.Key);
                        }
                    }

                }
            }
        }

        private static void FillContentMapsFromCurrentManifest()
        {
            resourcePakMap.Clear();
            resourceManifestMap.Clear();

            foreach (var group in manifest.Content)
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

        private static string FindResource(string full_path)
        {
            var res_id = Path.GetFileNameWithoutExtension(full_path);

            if(resourceManifestMap.TryGetValue(res_id, out _))
            {
                return res_id;
            }
          
            return null;
          
        }

        private static void UpdateResourceOnPak(string res_id)
        {
            var res_manifest = resourceManifestMap[res_id];
            var res_pak_name = resourcePakMap[res_id];

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

            reloadCachePaks.Remove(res_pak_name);

            reloadCachePaks.Add(res_pak_name, pak);

            ConsoleUtils.ShowInfo($"Reloading Resource: {res_id} on Pak : {res_pak_name}");
        }


        private static void UpdateResourceOnManifest(string new_res_id, string old_id, string new_path)
        {
            if(resourceManifestMap.TryGetValue(old_id, out ResourceManifest res_manifest))
            {
                string pak = resourcePakMap[old_id];

                switch (res_manifest)
                {
                    case ImageManifest _:

                        var img_extension = Path.GetExtension(new_path);

                        if(img_extension != ".png")
                        {
                            throw new Exception($"[Renamed File] Unsupported Image File : {new_path}");
                        }

                        var img_manifest = new ImageManifest() 
                        { 
                            Id = new_res_id, 
                            Path = PathUtils.GetLocalNormalizedRelativePath(Path.Combine(content_path, pak), new_path)
                        };

                        manifest.Content[pak].Images.Remove(old_id);
                        manifest.Content[pak].Images.Add(new_res_id, img_manifest);

                        break;

                    case ShaderManifest _:

                        

                        var sha_manifest = new ShaderManifest() 
                        {
                            Id = new_res_id
                        };

                        var extension_sha = Path.GetExtension(new_path);
                        
                        if(extension_sha == ".vs")
                        {
                            sha_manifest.VertexSrcPath = PathUtils.GetLocalNormalizedRelativePath(Path.Combine(content_path, pak), new_path);
                        }
                        else if (extension_sha == ".fs")
                        {
                            sha_manifest.FragmentSrcPath = PathUtils.GetLocalNormalizedRelativePath(Path.Combine(content_path, pak), new_path);
                        }
                        else
                        {
                            throw new Exception($"[Renamed File] Unrecognized Shader File : {new_path}");
                        }

                        manifest.Content[pak].Shaders.Remove(old_id);
                        manifest.Content[pak].Shaders.Add(new_res_id, sha_manifest);

                        break;

                    case FontManifest _:

                        var fnt_manifest = new FontManifest
                        {
                            Id = new_res_id
                        };

                        var extension_fnt = Path.GetExtension(new_path);

                        if (extension_fnt == ".fnt")
                        {
                            fnt_manifest.Path = PathUtils.GetLocalNormalizedRelativePath(Path.Combine(content_path, pak), new_path);
                        }
                        else if (extension_fnt == ".png")
                        {
                            fnt_manifest.ImagePath = PathUtils.GetLocalNormalizedRelativePath(Path.Combine(content_path, pak), new_path);
                        }
                        else
                        {
                            throw new Exception($"[Renamed File] Unrecognized Font File : {new_path}");
                        }

                        manifest.Content[pak].Fonts.Remove(old_id);
                        manifest.Content[pak].Fonts.Add(new_res_id, fnt_manifest);

                        break;

                    case EffectManifest _:

                        var sfx_extension = Path.GetExtension(new_path);

                        if (sfx_extension != ".wav")
                        {
                            throw new Exception($"[Renamed File] Unsupported Sfx File : {new_path}");
                        }

                        var sfx_manifest = new EffectManifest
                        {
                            Id = new_res_id,
                            Path = PathUtils.GetLocalNormalizedRelativePath(Path.Combine(content_path, pak), new_path)
                        };

                        manifest.Content[pak].Effects.Remove(old_id);
                        manifest.Content[pak].Effects.Add(new_res_id, sfx_manifest);

                        break;

                    case SongManifest _:

                        var sng_extension = Path.GetExtension(new_path);

                        if (sng_extension != ".ogg")
                        {
                            throw new Exception($"[Renamed File] Unsupported Song File : {new_path}");
                        }

                        var sng_manifest = new SongManifest
                        {
                            Id = new_res_id,
                            Path = PathUtils.GetLocalNormalizedRelativePath(Path.Combine(content_path, pak), new_path)
                        };


                        manifest.Content[pak].Songs.Remove(old_id);
                        manifest.Content[pak].Songs.Add(new_res_id, sng_manifest);

                        break;

                    case TextFileManifest _:

                        var txt_extension = Path.GetExtension(new_path);

                        if (txt_extension != ".txt")
                        {
                            throw new Exception($"[Renamed File] Unsupported Txt File : {new_path}");
                        }

                        var txt_manifest = new TextFileManifest
                        {
                            Id = new_res_id,
                            Path = PathUtils.GetLocalNormalizedRelativePath(Path.Combine(content_path, pak), new_path)
                        };

                        manifest.Content[pak].TextFiles.Remove(old_id);
                        manifest.Content[pak].TextFiles.Add(new_res_id, txt_manifest);

                        break;
                }

                SaveContentManifest();

                FillContentMapsFromCurrentManifest();

                ConsoleUtils.ShowInfo($"Renamed Resource from {old_id} to {new_res_id} on Pak {pak}");
            }
        }

        private static void RemoveResourceOnManifest(string res_id)
        {
            string pak = resourcePakMap[res_id];
            ResourceManifest res_manifest = resourceManifestMap[res_id];

            switch (res_manifest)
            {
                case ImageManifest _:
                    manifest.Content[pak].Images.Remove(res_id);
                    break;

                case ShaderManifest _:
                    manifest.Content[pak].Shaders.Remove(res_id);
                    break;

                case FontManifest _:
                    manifest.Content[pak].Fonts.Remove(res_id);
                    break;

                case EffectManifest _:
                    manifest.Content[pak].Effects.Remove(res_id);
                    break;

                case SongManifest _:
                    manifest.Content[pak].Songs.Remove(res_id);
                    break; 

                case TextFileManifest _:
                    manifest.Content[pak].TextFiles.Remove(res_id);
                    break;
            }

            SaveContentManifest();

            FillContentMapsFromCurrentManifest();

        }

        private static bool RemoveResourceOnPak(string res_id)
        {
            var res_pak_name = resourcePakMap[res_id];

            if (!reloadCachePaks.TryGetValue(res_pak_name, out _))
            {
                var newPak = ContentLoader.LoadPak(content_path, res_pak_name);

                reloadCachePaks.Add(res_pak_name, newPak);
            }

            var pak = reloadCachePaks[res_pak_name];

            var removed = pak.Resources.Remove(res_id);

            if(removed)
            {
                pak.SaveToDisk(content_path);
            }

            reloadCachePaks.Remove(res_pak_name);

            reloadCachePaks.Add(res_pak_name, pak);

            ConsoleUtils.ShowInfo($"Removed Resource: {res_id} from Pak : {res_pak_name}");

            return removed;

        }
    }
}
