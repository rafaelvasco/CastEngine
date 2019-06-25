using System;
using System.IO;
using System.Linq;
using System.Text;

namespace CastFramework
{
    public static class ContentLoader
    {
        private const string FNT_HEADER_TAG = "[BTFONT]";
        private const string FNT_CHAR_TAG = "Char=";

        private static readonly ImageReader image_reader = new ImageReader();

        private static T DeserializeObject<T>(string path)
        {
            var bytes = File.ReadAllBytes(path);

            var obj = BinaryIO.Deserialize<T>(bytes);

            return obj;
        }

        public static ResourcePak LoadPak(string content_path, string pak_name)
        {
            var path = Path.Combine(content_path,
                !pak_name.Contains(".pak") ? pak_name + ".pak" : pak_name);

            ResourcePak pak = DeserializeObject<ResourcePak>(path);

            return pak;
        }

        /* Content Loaders */
        /* ==================================================== */
       
        internal static Texture2D LoadTexture(PixmapData pixmap_data)
        {
            var pixmap = new Pixmap(pixmap_data.Data, pixmap_data.Width, pixmap_data.Height);

            var texture = new Texture2D(pixmap, false, false);

            texture.Id = pixmap_data.Id;

            return texture;
        }

        internal static Texture2D LoadTexture(string path)
        {
            var pixmap_data = LoadPixmapData(path);
            return LoadTexture(pixmap_data);
        }

        internal static Font LoadFont(FontData font_data)
        {
            var texture = LoadTexture(font_data.FontSheet);
            var glyphs = new Sprite[font_data.GlyphRects.Length];

            for (int i = 0; i < font_data.GlyphRects.Length; ++i)
            {
                var glyph_rect = font_data.GlyphRects[i];

                glyphs[i] = new Sprite(texture, glyph_rect.X1, glyph_rect.Y1, glyph_rect.Width, glyph_rect.Height);
            }

            var font = new Font(texture, glyphs, font_data.PreSpacings, font_data.PostSpacings) { Id = font_data.Id };

            return font;
        }

        internal static Font LoadFont(string path)
        {
            var font_image_path = Path.Combine(
                Path.GetDirectoryName(path),
                Path.GetFileNameWithoutExtension(path) + ".png"
            );

            var font_data = LoadFontData(path, font_image_path);

            return LoadFont(font_data);
        }

        internal static ShaderProgram LoadShader(ShaderProgramData shader_data)
        {
            var shader_program = new ShaderProgram(shader_data.VertexShader, shader_data.FragmentShader, shader_data.Samplers, shader_data.Params);

            shader_program.Id = shader_data.Id;

            return shader_program;
        }

        internal static ShaderProgram LoadShader(string vs_path, string fs_path)
        {
            var shader_prog_data = LoadShaderProgramData(vs_path, fs_path);

            return LoadShader(shader_prog_data);
        }

        //TODO:
        /*public unsafe Effect LoadEffect(SfxData sfx_data)
        {
            var wav = new Wav();

            fixed (byte* p = sfx_data.Data)
            {
                var ptr = (IntPtr)p;

                wav.loadMem(ptr, (uint)sfx_data.Data.Length, aCopy: true);
            }

            var effect = new Effect(wav)
            {
                Id = sfx_data.Id
            };

            return effect;
        }*/

        /*public Effect LoadEffect(string path)
        {
            var sfx_data = LoadSfxData(path);

            return LoadEffect(sfx_data);
        }*/

        /*public unsafe Song LoadSong(SongData song_data)
        {
            var wav_stream = new WavStream();

            fixed (byte* p = song_data.Data)
            {
                var ptr = (IntPtr)p;

                wav_stream.loadMem(ptr, (uint)song_data.Data.Length, aCopy: true);
            }

            var song = new Song(wav_stream)
            {
                Id = song_data.Id
            };

            return song;
        }*/

        /*public Song LoadSong(string path)
        {
            var song_data = LoadSongData(path);

            return LoadSong(song_data);
        }*/

        internal static TextFile LoadTextFile(TextFileData txt_data)
        {
            var txt_file = new TextFile(txt_data.TextData.ToList())
            {
                Id = txt_data.Id
            };

            return txt_file;
        }

        public static TextFile LoadTextFile(string path)
        {
            var txt_data = LoadTextFileData(path);

            return LoadTextFile(txt_data);
        }

        /* File Data Loaders */
        /* ================================================================== */

        public static PixmapData LoadPixmapData(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                Image img = image_reader.Read(stream);

                var id = Path.GetFileNameWithoutExtension(path);

                var pixmap_data = new PixmapData()
                {
                    Id = id,
                    Data = img.Data,
                    Width = img.Width,
                    Height = img.Height
                };

                return pixmap_data;
            }
        }

        public static ShaderProgramData LoadShaderProgramData(string vs_path, string fs_path)
        {
            var result = ShaderBuilder.Build(vs_path, fs_path);

            var id = Path.GetFileNameWithoutExtension(vs_path);

            var shader_program_data = new ShaderProgramData()
            {
                Id = id,
                VertexShader = result.VsBytes,
                FragmentShader = result.FsBytes,
                Samplers = result.Samplers,
                Params = result.Params
            };

            return shader_program_data;
        }

        public static FontData LoadFontData(string descr_path, string image_path)
        {
            var sheet_data = LoadPixmapData(image_path);

            var glyphs = new Rect[255];
            var pre_spacings = new float[255];
            var post_spacings = new float[255];

            using (var descr_stream = File.OpenRead(descr_path))
            {
                using (var reader = new StreamReader(descr_stream, Encoding.UTF8))
                {
                    string line;
                    var idx = 0;

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Length == 0)
                        {
                            continue;
                        }

                        if (idx == 0 && !line.Equals(FNT_HEADER_TAG))
                        {
                            throw new Exception("Invalid Font Description File.");
                        }

                        if (line.StartsWith(FNT_CHAR_TAG))
                        {
                            string char_def_str = line.Split('=')[1];

                            string[] char_def_attrs = char_def_str.Split(',');

                            if (char_def_attrs.Length != 7)
                            {
                                throw new Exception(
                                    $"Invalid Font Description File: Invalid Char Definition at line: {line + 1}");
                            }

                            int ch_idx = int.Parse(char_def_attrs[0]);

                            if (ch_idx < 0 || ch_idx > 255)
                            {
                                throw new Exception("Invalid Font Description File: Character Id out of range");
                            }

                            int letter_reg_x = int.Parse(char_def_attrs[1]);
                            int letter_reg_y = int.Parse(char_def_attrs[2]);
                            int letter_reg_w = int.Parse(char_def_attrs[3]);
                            int letter_reg_h = int.Parse(char_def_attrs[4]);
                            int letter_pre_spac = int.Parse(char_def_attrs[5]);
                            int letter_post_spac = int.Parse(char_def_attrs[6]);

                            glyphs[ch_idx] = Rect.FromBox(letter_reg_x, letter_reg_y, letter_reg_w,
                                letter_reg_h);

                            pre_spacings[ch_idx] = letter_pre_spac;
                            post_spacings[ch_idx] = letter_post_spac;
                        }

                        idx++;
                    }
                }
            }

            var id = Path.GetFileNameWithoutExtension(descr_path);

            var font_data = new FontData()
            {
                FontSheet = sheet_data,
                GlyphRects = glyphs,
                Id = id,
                PreSpacings = pre_spacings,
                PostSpacings = post_spacings
            };

            return font_data;
        }

        //TODO:
        /*public SfxData LoadSfxData(string path)
        {
            var bytes = File.ReadAllBytes(path);

            var id = Path.GetFileNameWithoutExtension(path);

            var sfx_data = new SfxData()
            {
                Id = id,
                Data = bytes
            };

            return sfx_data;
        }*/

        /*public SongData LoadSongData(string path)
        {
            var bytes = File.ReadAllBytes(path);
            var id = Path.GetFileNameWithoutExtension(path);

            var song_data = new SongData()
            {
                Id = id,
                Data = bytes
            };

            return song_data;
        }*/

        public static TextFileData LoadTextFileData(string path)
        {
            var text = File.ReadAllLines(path);

            var id = Path.GetFileNameWithoutExtension(path);

            var text_file_data = new TextFileData()
            {
                Id = id,
                TextData = text.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray()
            };

            return text_file_data;
        }

    }
}
