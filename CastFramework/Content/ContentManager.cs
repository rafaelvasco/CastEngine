using System;
using System.Collections.Generic;
using System.IO;

namespace CastFramework
{
    public class ContentManager
    {
        private readonly Dictionary<string, Resource> loaded_resources;

        private readonly List<Resource> runtime_resources;

        public ContentManager()
        {
            loaded_resources = new Dictionary<string, Resource>();
            runtime_resources = new List<Resource>();
            LoadContentPack("Base");
        }

        public T Get<T>(string resource_id) where T : Resource
        {
            if (loaded_resources.TryGetValue(resource_id, out Resource resource))
            {
                return (T)resource;
            }

            Game.Instance.ThrowError("Can't find resource with ID: {0}", resource_id);

            return null;
        }

        public Texture2D LoadTexture(string texture_path)
        {
            var full_path = Path.Combine("Content", texture_path);

            var id = Path.GetFileNameWithoutExtension(texture_path);

            if (loaded_resources.TryGetValue(id, out Resource res))
            {
                return (Texture2D)res;
            }

            Texture2D texture = ContentLoader.LoadTexture(full_path);

            loaded_resources.Add(texture.Id, texture);

            return texture;
        }

        public ShaderProgram LoadShader(string vs_path, string fs_path)
        {
            var vs_full_path = Path.Combine("Content", vs_path);
            var fs_full_path = Path.Combine("Content", fs_path);

            var id = Path.GetFileNameWithoutExtension(vs_path);

            if (loaded_resources.TryGetValue(id, out Resource res))
            {
                return (ShaderProgram)res;
            }

            ShaderProgram shader = ContentLoader.LoadShader(vs_full_path, fs_full_path);

            loaded_resources.Add(shader.Id, shader);

            return shader;
        }

        public Font LoadFont(string font_path)
        {
            var full_path = Path.Combine("Content", font_path);

            var id = Path.GetFileNameWithoutExtension(font_path);

            if (loaded_resources.TryGetValue(id, out Resource res))
            {
                return (Font)res;
            }

            Font font = ContentLoader.LoadFont(full_path);

            loaded_resources.Add(font.Id, font);

            return font;
        }

        //TODO:
        /*public Effect LoadSfx(string sfx_path)
        {
            var full_path = Path.Combine("Content", sfx_path);

            var id = Path.GetFileNameWithoutExtension(sfx_path);

            if (loaded_resources.TryGetValue(id, out Resource res))
            {
                return (Effect)res;
            }

            Effect sfx = loader.LoadEffect(full_path);

            loaded_resources.Add(sfx.Id, sfx);

            return sfx;
        }*/

        //TODO:
        /*public Song LoadSong(string song_path)
        {
            var full_path = Path.Combine("Content", song_path);

            var id = Path.GetFileNameWithoutExtension(song_path);

            if (loaded_resources.TryGetValue(id, out Resource res))
            {
                return (Song)res;
            }

            Song song = loader.LoadSong(full_path);

            loaded_resources.Add(song.Id, song);

            return song;
        }*/

        public void LoadContentPack(string pak_name)
        {
            ResourcePak pak = ContentLoader.LoadPak("Content", pak_name);

            // Extract Resources

            foreach(var resource in pak.Resources)
            {
                switch(resource.Value.Type)
                {
                    case ResourceDataType.Image:

                        Texture2D texture = ContentLoader.LoadTexture((PixmapData)resource.Value);
                        loaded_resources.Add(texture.Id, texture);
                        break;

                    case ResourceDataType.Font:

                        Font font = ContentLoader.LoadFont((FontData)resource.Value);
                        loaded_resources.Add(font.Id, font);
                        break;

                    case ResourceDataType.Shader:

                        ShaderProgram shader = ContentLoader.LoadShader((ShaderProgramData)resource.Value);
                        loaded_resources.Add(shader.Id, shader);
                        break;

                    case ResourceDataType.Sfx:
                        //TODO:
                        break;

                    case ResourceDataType.Song:
                        //TODO:
                        break;
                    case ResourceDataType.Text:

                        TextFile text_file = ContentLoader.LoadTextFile((TextFileData)resource.Value);
                        loaded_resources.Add(text_file.Id, text_file);

                        break;
                }
            }
        }

        public Pixmap CreatePixmap(byte[] data, int width, int height)
        {
            var pixmap = new Pixmap(data, width, height)
            {
                Id = $"Pixmap [{width},{height}]"
            };

            RegisterRuntimeLoaded(pixmap);

            return pixmap;
        }

        public Pixmap CreatePixmap(int width, int height)
        {
            var pixmap = new Pixmap(width, height) { Id = $"Pixmap [{width},{height}]" };

            RegisterRuntimeLoaded(pixmap);

            return pixmap;
        }

        public Texture2D CreateTexture(Pixmap pixmap, bool tiled, bool filtered)
        {
            var texture = new Texture2D(pixmap, filtered, tiled);

            texture.Id = $"Texture [{texture.Width},{texture.Height}]";

            RegisterRuntimeLoaded(texture);

            return texture;
        }

        public Texture2D CreateTexture(Texture2D texture, int srcX, int srcY, int srcW, int srcH, bool tiled, bool filtered)
        {
            var pixmapRegion = texture.GetData(srcX, srcY, srcW, srcH);

            return CreateTexture(pixmapRegion, tiled, filtered);
        }

        public Texture2D CreateTexture(int width, int height, bool tiled, bool filtered, Color color)
        {
            var pixmap = new Pixmap(width, height);

            pixmap.Fill(color);

            return CreateTexture(pixmap, tiled, filtered);
        }

        public RenderTarget CreateRenderTarget(int width, int height)
        {
            var render_target = new RenderTarget(width, height);

            render_target.Id = $"Render Target: [{width.ToString()}, {height.ToString()}]";

            RegisterRuntimeLoaded(render_target);

            return render_target;
        }

        internal void RegisterRuntimeLoaded(Resource resource)
        {
            runtime_resources.Add(resource);
        }

        internal void DisposeRuntimeLoaded(Resource resource)
        {
            runtime_resources.Remove(resource);

            resource.Dispose();
        }

        internal void FreeEverything()
        {
            Console.WriteLine($" > Diposing {loaded_resources.Count.ToString()} loaded resources.");

            foreach (var resource in loaded_resources)
            {
                Console.WriteLine($" > Diposing {resource.Key}.");
                resource.Value.Dispose();
            }

            Console.WriteLine($" > Disposing {runtime_resources.Count.ToString()} runtime resources.");

            foreach (var resource in runtime_resources)
            {
                Console.WriteLine($" > Diposing {resource.Id}.");
                resource.Dispose();
            }

            loaded_resources.Clear();
            runtime_resources.Clear();
        }
    }
}
