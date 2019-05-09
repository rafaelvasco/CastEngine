using System;
using System.Numerics;

namespace CastFramework
{
    public class Sprite
    {
        public bool FlipH => flip_x;
        public bool FlipV => flip_y;

        public Sprite(Texture2D texture) : this(texture, 0, 0, texture.Width, texture.Height)
        {
        }

        public Sprite(Texture2D texture, Quad quad)
        {
            this.texture = texture;
            Width = quad.Width;
            Height = quad.Height;

            origin_x = 0;
            origin_y = 0;
            flip_x = false;
            flip_y = false;

            this.quad = quad;

        }

        public Sprite(Texture2D texture, float src_x, float src_y, float src_w, float src_h)
        {
            Width = src_w;
            Height = src_h;

            origin_x = 0;
            origin_y = 0;
            flip_x = false;
            flip_y = false;
            this.texture = texture;

            var tex_w = texture.Width;
            var tex_h = texture.Height;

            var u = src_x / tex_w;
            var v = src_y / tex_h;
            var u2 = (src_x + src_w) / tex_w;
            var v2 = (src_y + src_h) / tex_h;

            quad.V0.Tx = u;
            quad.V0.Ty = v;
            quad.V0.Col = 0xFFFFFFFF;
            quad.V1.Tx = u2;
            quad.V1.Ty = v;
            quad.V1.Col = 0xFFFFFFFF;
            quad.V2.Tx = u2;
            quad.V2.Ty = v2;
            quad.V2.Col = 0xFFFFFFFF;
            quad.V3.Tx = u;
            quad.V3.Ty = v2;
            quad.V3.Col = 0xFFFFFFFF;

        }

        public Vector2 Origin => new Vector2(origin_x, origin_y);

        public float Width { get; protected set; }

        public float Height { get; protected set; }

        public void Draw(Canvas canvas, float x, float y)
        {
            var tx1 = x - origin_x;
            var ty1 = y - origin_y;
            var tx2 = x + Width - origin_x;
            var ty2 = y + Height - origin_y;

            quad.V0.X = tx1;
            quad.V0.Y = ty1;
            quad.V1.X = tx2;
            quad.V1.Y = ty1;
            quad.V2.X = tx2;
            quad.V2.Y = ty2;
            quad.V3.X = tx1;
            quad.V3.Y = ty2;

            canvas.DrawQuad(texture, ref quad);
        }

        public void DrawEx(Canvas canvas, float x, float y, float rot, float hscale = 1, float vscale = -1)
        {

            if (vscale < 0)
            {
                vscale = hscale;
            }

            var tx1 = -origin_x * hscale;
            var ty1 = -origin_y * vscale;
            var tx2 = (Width - origin_x) * hscale;
            var ty2 = (Height - origin_y) * vscale;

            if (rot != 0.0f)
            {
                var cost = Calc.Cos(rot);
                var sint = Calc.Sin(rot);

                quad.V0.X = tx1 * cost - ty1 * sint + x;
                quad.V0.Y = tx1 * sint + ty1 * cost + y;
                quad.V1.X = tx2 * cost - ty1 * sint + x;
                quad.V1.Y = tx2 * sint + ty1 * cost + y;
                quad.V2.X = tx2 * cost - ty2 * sint + x;
                quad.V2.Y = tx2 * sint + ty2 * cost + y;
                quad.V3.X = tx1 * cost - ty2 * sint + x;
                quad.V3.Y = tx1 * sint + ty2 * cost + y;
            }
            else
            {
                quad.V0.X = tx1 + x;
                quad.V0.Y = ty1 + y;
                quad.V1.X = tx2 + x;
                quad.V1.Y = ty1 + y;
                quad.V2.X = tx2 + x;
                quad.V2.Y = ty2 + y;
                quad.V3.X = tx1 + x;
                quad.V3.Y = ty2 + y;
            }


            canvas.DrawQuad(texture, ref quad);
        }

        public void SetFlipH(bool flip, bool flip_origin = true)
        {
            if (flip_x == flip)
            {
                return;
            }

            SetFlip(flip, flip_y, flip_origin);
        }

        public void SetFlipV(bool flip, bool flip_origin = true)
        {
            if (flip_y == flip)
            {
                return;
            }

            SetFlip(flip_x, flip, flip_origin);
        }

        public void SetFlip(bool flip_h, bool flip_v, bool flip_origin = true)
        {
            if (flip_x == flip_h && flip_y == flip_v)
            {
                return;
            }

            float tx, ty;

            if (flip_origin && flip_x) origin_x = Width - origin_x;
            if (flip_origin && flip_y) origin_y = Height - origin_y;

            orig_flip = flip_origin;

            if (flip_origin && flip_x) origin_x = Width - origin_x;
            if (flip_origin && flip_y) origin_y = Height - origin_y;


            if (flip_h != flip_x)
            {
                tx = quad.V0.Tx;
                quad.V0.Tx = quad.V1.Tx;
                quad.V1.Tx = tx;

                ty = quad.V0.Ty;
                quad.V0.Ty = quad.V1.Ty;
                quad.V1.Ty = ty;

                tx = quad.V3.Tx;
                quad.V3.Tx = quad.V2.Tx;
                quad.V2.Tx = tx;

                ty = quad.V3.Ty;
                quad.V3.Ty = quad.V2.Ty;
                quad.V2.Ty = ty;

                flip_x = !flip_x;
            }

            if (flip_v != flip_y)
            {
                tx = quad.V0.Tx;
                quad.V0.Tx = quad.V3.Tx;
                quad.V3.Tx = tx;

                ty = quad.V0.Ty;
                quad.V0.Ty = quad.V3.Ty;
                quad.V3.Ty = ty;

                tx = quad.V1.Tx;
                quad.V1.Tx = quad.V2.Tx;
                quad.V2.Tx = tx;

                ty = quad.V1.Ty;
                quad.V1.Ty = quad.V2.Ty;
                quad.V2.Ty = ty;

                flip_y = !flip_y;
            }


        }

        public void SetTexture(Texture2D tex)
        {
            if (tex == null)
            {
                quad = new Quad();
                return;
            }

            var old_tex_w = texture.Width;
            var old_tex_h = texture.Height;

            var tex_w = tex.Width;
            var tex_h = tex.Height;


            if (tex_w != old_tex_w || tex_h != old_tex_h)
            {
                var u = quad.V0.Tx * old_tex_w;
                var v = quad.V0.Ty * old_tex_h;
                var u2 = quad.V2.Tx * old_tex_w;
                var v2 = quad.V2.Ty * old_tex_h;

                u /= tex_w;
                v /= tex_h;
                u2 /= tex_w;
                v2 /= tex_h;

                quad.V0.Tx = u;
                quad.V0.Ty = v;
                quad.V1.Tx = u2;
                quad.V1.Ty = v;
                quad.V2.Tx = u2;
                quad.V2.Ty = v2;
                quad.V3.Tx = u;
                quad.V3.Ty = v2;
            }
        }

        public void SetTextureRect(float src_x, float src_y, float src_w, float src_h, bool adj_size)
        {
            if (adj_size)
            {
                Width = src_w;
                Height = src_h;
            }

            var tex_w = texture.Width;
            var tex_h = texture.Height;

            var u = src_x / tex_w;
            var v = src_y / tex_h;
            var u2 = (src_x + src_w) / tex_w;
            var v2 = (src_y + src_h) / tex_h;

            quad.V0.Tx = u;
            quad.V0.Ty = v;
            quad.V1.Tx = u2;
            quad.V1.Ty = v;
            quad.V2.Tx = u2;
            quad.V2.Ty = v2;
            quad.V3.Tx = u;
            quad.V3.Ty = v2;

            var flipx = flip_x;
            var flipy = flip_y;

            flip_x = false;
            flip_y = false;

            SetFlip(flipx, flipy, orig_flip);
        }

        public void SetColor(Color color)
        {
            uint abgr = color.ABGR;

            quad.V0.Col = abgr;
            quad.V1.Col = abgr;
            quad.V2.Col = abgr;
            quad.V3.Col = abgr;
        }

        public void SetColor(uint abgr)
        {
            quad.V0.Col = abgr;
            quad.V1.Col = abgr;
            quad.V2.Col = abgr;
            quad.V3.Col = abgr;
        }


        public void SetColor
        (
            Color top_left_col,
            Color top_right_col,
            Color bottom_left_col,
            Color bottom_right_col
        )
        {
            uint abgr_tl = top_left_col.ABGR;
            uint abgr_tr = top_right_col.ABGR;
            uint abgr_bl = bottom_left_col.ABGR;
            uint abgr_br = bottom_right_col.ABGR;

            quad.V0.Col = abgr_tl;
            quad.V1.Col = abgr_tr;
            quad.V2.Col = abgr_br;
            quad.V3.Col = abgr_bl;
        }

        public void SetColor
        (
            uint top_left_col,
            uint top_right_col,
            uint bottom_left_col,
            uint bottom_right_col
        )
        {
            quad.V0.Col = top_left_col;
            quad.V1.Col = top_right_col;
            quad.V2.Col = bottom_right_col;
            quad.V3.Col = bottom_left_col;
        }

        public Color GetColor(int corner = 0)
        {
            switch (corner)
            {
                case 0: return new Color(quad.V0.Col);
                case 1: return new Color(quad.V1.Col);
                case 2: return new Color(quad.V2.Col);
                case 3: return new Color(quad.V3.Col);
            }

            return default;
        }

        public void SetOrigin(float ox, float oy)
        {
            origin_x = Width * ox;
            origin_y = Height * oy;
        }

        public void SetOrigin(ref Vector2 orig)
        {
            origin_x = Width * orig.X;
            origin_y = Height * orig.Y;
        }

        public void SetBlendMode(BlendMode blend)
        {
            this.quad.Blend = blend;
        }

        /*public Rect GetBoundingBox(float ref_x, float ref_y)
        {
            return RectF.FromBox(ref_x - _origin_x, ref_y - _origin_y, ref_x - _origin_x + Width,
                ref_y - _origin_y + Height);
        }*/

        protected bool flip_x;
        protected bool flip_y;
        protected bool orig_flip;
        protected float origin_x;
        protected float origin_y;

        protected readonly Texture2D texture;

        protected BlendMode blend_mode = BlendMode.AlphaBlend;

        protected Quad quad;


    }
}
