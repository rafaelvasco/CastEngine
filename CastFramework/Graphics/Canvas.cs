using System.Numerics;

namespace CastFramework
{
    public enum CanvasStretchMode : byte
    {
        PixelPerfect = 0,
        LetterBox,
        Stretch,
        Resize
    }

    public unsafe class Canvas
    {
        public Font DefaultFont => default_font;

        public int Width { get; private set; }

        public int Height { get; private set; }

        public Point MousePos => new Point(
           (int)(Input.mouse_position.X / render_scale_x - render_area.X1 / render_scale_x),
           (int)(Input.mouse_position.Y / render_scale_y - render_area.Y1 / render_scale_y));

        public CanvasStretchMode StretchMode
        {
            get => stretch_mode;
            set => stretch_mode = value;
        }

        public Point RenderAreaTopLeft => render_area.TopLeft;

        public Font DefaultFont2 => default_font2;

        public int MaxDrawCalls { get; private set; }

        internal Canvas(GraphicsContext graphics_context, int width, int height, int max_vertex_count)
        {
            gfx = graphics_context;

            vertex_max_count = max_vertex_count;

            Width = width;

            Height = height;

            render_area = Rect.FromBox(0, 0, width, height);

            default_shader = Game.Instance.ContentManager.Get<ShaderProgram>("base_2d");

            InitPipeline();

            default_font = Game.Instance.ContentManager.Get<Font>("default_font");
            default_font2 = Game.Instance.ContentManager.Get<Font>("default_font2");

            OnScreenResized(Game.Instance.ScreenSize.W, Game.Instance.ScreenSize.H);

            SetBlendMode(BlendMode.AlphaBlend);

            var prim_pixmap = new Pixmap(10, 10);

            prim_pixmap.Fill(Color.White);

            prim_texture = Game.Instance.ContentManager.CreateTexture(prim_pixmap, false, false);

            prim_pixmap.Dispose();
        }

        private void SetBlendMode(BlendMode blend_mode)
        {
            pipeline.SetBlendMode(blend_mode);
        }

        public void SetShader(ShaderProgram shader)
        {
            pipeline.SetShaderProgram(shader ?? default_shader);
        }

        internal void BeginRendering()
        {
            pipeline.Reset();

            pipeline.SetShaderProgram(default_shader);

            SetSurface();
        }

        internal void EndRendering()
        {
            pipeline.Submit();

            pipeline.DrawSurfaces();
        }

        public void SetSurface(RenderSurface surface = null)
        {
            pipeline.SetRenderSurface(surface);
        }

        public void BeginClip(int x, int y, int w, int h)
        {
            pipeline.SetScissor(x, y, w, h);
        }

        public void EndClip()
        {
            pipeline.SetScissor(0, 0, 0, 0);
        }

        public void DrawQuad(Texture2D texture, ref Quad quad)
        {
            pipeline.PushQuad(texture, ref quad);
        }

        public void DrawRect(float x, float y, float w, float h, Color color, float line_size = 1)
        {
          
            ref Quad q = ref primitives_buffer[0];

            uint col = color.ABGR;

            q.V0 = new Vertex2D(x, y, 0, 0, col);
            q.V1 = new Vertex2D(x + w, y, 0, 0, col);
            q.V2 = new Vertex2D(x + w, y + line_size, 0, 0, col);
            q.V3 = new Vertex2D(x, y + line_size, 0, 0, col);

            q = ref primitives_buffer[1];

            q.V0 = new Vertex2D(x, y + h - line_size, 0, 0, col);
            q.V1 = new Vertex2D(x + w, y + h - line_size, 0, 0, col);
            q.V2 = new Vertex2D(x + w, y + h, 0, 0, col);
            q.V3 = new Vertex2D(x, y + h, 0, 0, col);

            q = ref primitives_buffer[2];

            q.V0 = new Vertex2D(x, y, 0, 0, col);
            q.V1 = new Vertex2D(x + line_size, y, 0, 0, col);
            q.V2 = new Vertex2D(x + line_size, y + h, 0, 0, col);
            q.V3 = new Vertex2D(x, y + h, 0, 0, col);

            q = ref primitives_buffer[3];

            q.V0 = new Vertex2D(x + w - line_size, y, 0, 0, col);
            q.V1 = new Vertex2D(x + w, y, 0, 0, col);
            q.V2 = new Vertex2D(x + w, y + h, 0, 0, col);
            q.V3 = new Vertex2D(x + w - line_size, y + h, 0, 0, col);
           
            pipeline.PushQuads(prim_texture, primitives_buffer, 4);
        }

        public void FillRect(float x, float y, float w, float h, Color color)
        {
            ref Quad q = ref primitives_buffer[0];

            uint col = color.ABGR;

            q.V0 = new Vertex2D(x, y, 0, 0, col);
            q.V1 = new Vertex2D(x + w, y, 0, 0, col);
            q.V2 = new Vertex2D(x + w, y + h, 0, 0, col);
            q.V3 = new Vertex2D(x, y + h, 0, 0, col);

            pipeline.PushQuads(prim_texture, primitives_buffer, 1);
        }

        public void DrawText(float x, float y, string text, Color color, float scale = 1.0f)
        {
            DrawText(default_font, x, y, text, color, scale);
        }

        public void DrawText(Font font, float x, float y, string text, Color color, float scale = 1.0f)
        {
            var str_len = text.Length;

            if (str_len == 0)
            {
                return;
            }

            var dx = x;

            var letters = font.letters;
            var pre_spacings = font.pre_spacings;
            var post_spacings = font.post_spacings;

            for (var i = 0; i < str_len; ++i)
            {
                int ch_idx = text[i];

                if (letters[ch_idx] == null) ch_idx = '?';

                if (letters[ch_idx] != null)
                {
                    dx += pre_spacings[ch_idx] * scale;
                    letters[ch_idx].SetColor(color);

                    letters[ch_idx].DrawEx(this, dx, y, 0.0f, scale, scale);
                    dx += (letters[ch_idx].Width + post_spacings[ch_idx]) * scale;
                }
            }
        }

        public void SaveScreenShot(string path)
        {
            gfx.TakeScreenShot(path);
        }

        public RenderSurface AddSurface(Rect area, string name = "Surface")
        {
           return pipeline.AddSurface(area, name);
        }

        internal void OnScreenResized(int width, int height)
        {
            gfx.ResizeBackBuffer(width, height);

            var canvas_w = this.Width;
            var canvas_h = this.Height;

            switch (stretch_mode)
            {
                case CanvasStretchMode.PixelPerfect:

                    if (width > canvas_w || height > canvas_h)
                    {
                        var ar_origin = (float)canvas_w / canvas_h;
                        var ar_new = (float)width / height;

                        var scale_w = Calc.FloorToInt((float)width / canvas_w);
                        var scale_h = Calc.FloorToInt((float)height / canvas_h);

                        if (ar_new > ar_origin)
                            scale_w = scale_h;
                        else
                            scale_h = scale_w;

                        var margin_x = (width - canvas_w * scale_w) / 2;
                        var margin_y = (height - canvas_h * scale_h) / 2;

                        render_scale_x = scale_w;
                        render_scale_y = scale_h;

                        render_area = Rect.FromBox(margin_x, margin_y, canvas_w * scale_w, canvas_h * scale_h);
                    }
                    else
                    {
                        render_area = Rect.FromBox(0, 0, canvas_w, canvas_h);
                    }

                    break;
                case CanvasStretchMode.LetterBox:

                    if (width > canvas_w || height > canvas_h)
                    {
                        var ar_origin = (float)canvas_w / canvas_h;
                        var ar_new = (float)width / height;

                        var scale_w = (float)width / canvas_w;
                        var scale_h = (float)height / canvas_h;

                        if (ar_new > ar_origin)
                            scale_w = scale_h;
                        else
                            scale_h = scale_w;

                        var margin_x = (int)((width - canvas_w * scale_w) / 2);
                        var margin_y = (int)((height - canvas_h * scale_h) / 2);

                        render_scale_x = scale_w;
                        render_scale_y = scale_h;

                        render_area = Rect.FromBox(margin_x, margin_y, (int)(canvas_w * scale_w),
                            (int)(canvas_h * scale_h));
                    }
                    else
                    {
                        render_scale_x = 1.0f;
                        render_scale_y = 1.0f;
                        render_area = Rect.FromBox(0, 0, canvas_w, canvas_h);
                    }

                    break;
                case CanvasStretchMode.Stretch:

                    render_scale_x = (float)width / canvas_w;
                    render_scale_y = (float)height / canvas_h; ;
                    render_area = Rect.FromBox(0, 0, width, height);

                    break;
                case CanvasStretchMode.Resize:

                    if (width == canvas_w && height == canvas_h)
                    {
                        break;
                    }

                    render_scale_x = 1.0f;
                    render_scale_y = 1.0f;

                    render_area = Rect.FromBox(0, 0, width, height);

                    pipeline.ResizeSurfaces(width, height);

                    this.Width = width;
                    this.Height = height;

                    break;
            }

            var screen_proj_matrix = Matrix4x4.CreateOrthographicOffCenter(
                0,
                width,
                height,
                0,
                0.0f,
                1.0f
            );

            pipeline.SetScreenProjection(screen_proj_matrix);

            pipeline.SetSurfaceAreas(render_area);
        }

        private void InitPipeline()
        {
            pipeline = gfx.CreatePipeline(vertex_max_count, render_area);
        }

        private readonly GraphicsContext gfx;

        private RenderPipeline pipeline; 

        private readonly ShaderProgram default_shader;

        private readonly Font default_font;

        private readonly Font default_font2;

        private readonly Texture2D prim_texture;

        private Rect render_area;

        private CanvasStretchMode stretch_mode = CanvasStretchMode.PixelPerfect;

        private readonly int vertex_max_count;

        private float render_scale_x = 1.0f;

        private float render_scale_y = 1.0f;

        private Quad[] primitives_buffer = new Quad[16];

    }
}
