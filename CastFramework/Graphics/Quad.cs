using System.Runtime.InteropServices;

namespace CastFramework
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Quad
    {
        public Vertex2D V0;
        public Vertex2D V1;
        public Vertex2D V2;
        public Vertex2D V3;

        private Rect region;

        public BlendMode Blend;

        public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Quad));

        public float Width => Calc.Abs(V1.X - V0.X);
        public float Height => Calc.Abs(V2.Y - V1.Y);


        public Quad(Texture2D texture, Rect src_rect = default, Rect dest_rect = default)
        {
            this.V0 = new Vertex2D();
            this.V1 = new Vertex2D();
            this.V2 = new Vertex2D();
            this.V3 = new Vertex2D();

            this.Blend = BlendMode.AlphaBlend;

            this.region = new Rect();

            if(texture != null)
            {
                SetRegion(texture, src_rect);
                SetArea(dest_rect);
            }
        }

        public void SetArea(Rect dest_rect)
        {
            float dest_x1, dest_y1, dest_x2, dest_y2;

            if (dest_rect.IsEmpty)
            {
                dest_x1 = 0;
                dest_y1 = 0;
                dest_x2 = region.Width;
                dest_y2 = region.Height;
            }
            else
            {
                dest_x1 = dest_rect.X1;
                dest_y1 = dest_rect.Y1;
                dest_x2 = dest_rect.X2;
                dest_y2 = dest_rect.Y2;
            }

            this.V0.X = dest_x1;
            this.V0.Y = dest_y1;
            this.V1.X = dest_x2;
            this.V1.Y = dest_y1;
            this.V2.X = dest_x2;
            this.V2.Y = dest_y2;
            this.V3.X = dest_x1;
            this.V3.Y = dest_y2;
        }

        public void SetRegion(Texture2D texture, Rect src_rect)
        {
            float ax, ay, bx, by;

            if (src_rect.IsEmpty)
            {
                region = Rect.FromBox(0, 0, texture.Width, texture.Height);

                ax = 0.0f;
                ay = 0.0f;
                bx = 1.0f;
                by = 1.0f;
            }
            else
            {
                region = src_rect;

                float inv_tex_w = 1.0f / texture.Width;
                float inv_tex_h = 1.0f / texture.Height;

                ax = src_rect.X1 * inv_tex_w;
                ay = src_rect.Y1 * inv_tex_h;
                bx = src_rect.X2 * inv_tex_w;
                by = src_rect.Y2 * inv_tex_h;
            }

            this.V0.Tx = ax;
            this.V0.Ty = ay;
            this.V1.Tx = bx;
            this.V1.Ty = ay;
            this.V2.Tx = bx;
            this.V2.Ty = by;
            this.V3.Tx = ax;
            this.V3.Ty = by;
        }

        public void SetColor(Color color)
        {
            uint abgr = color.ABGR;

            this.V0.Col = abgr;

            this.V1.Col = abgr;

            this.V2.Col = abgr;

            this.V3.Col = abgr;
        }

        public void SetColors(Color top_left, Color top_right, Color bottom_left, Color bottom_right)
        {
            this.V0.Col = top_left.ABGR;

            this.V1.Col = top_right.ABGR;

            this.V2.Col = bottom_right.ABGR;

            this.V3.Col = bottom_left.ABGR;
        }

        public Rect GetRegionRect(Texture2D texture)
        {
            return new Rect(
                (int)(V0.Tx * texture.Width), 
                (int)(V0.Ty * texture.Height), 
                (int)(V2.X * texture.Width), 
                (int)(V2.Y * texture.Height)
            );
        }
    }
}
