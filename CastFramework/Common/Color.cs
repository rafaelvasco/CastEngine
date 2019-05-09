using System;
using System.Runtime.InteropServices;
using System.Text;

namespace CastFramework
{
    public readonly struct Color : IEquatable<Color>, IComparable<Color>
    {
        public static readonly Color Transparent = new Color(0x00000000);
        public static readonly Color AliceBlue = new Color(0xfffff8f0);
        public static readonly Color AntiqueWhite = new Color(0xffd7ebfa);
        public static readonly Color Aqua = new Color(0xffffff00);
        public static readonly Color Aquamarine = new Color(0xffd4ff7f);
        public static readonly Color Azure = new Color(0xfffffff0);
        public static readonly Color Beige = new Color(0xffdcf5f5);
        public static readonly Color Bisque = new Color(0xffc4e4ff);
        public static readonly Color Black = new Color(0xff000000);
        public static readonly Color BlanchedAlmond = new Color(0xffcdebff);
        public static readonly Color Blue = new Color(0xffff0000);
        public static readonly Color BlueViolet = new Color(0xffe22b8a);
        public static readonly Color Brown = new Color(0xff2a2aa5);
        public static readonly Color BurlyWood = new Color(0xff87b8de);
        public static readonly Color CadetBlue = new Color(0xffa09e5f);
        public static readonly Color Chartreuse = new Color(0xff00ff7f);
        public static readonly Color Chocolate = new Color(0xff1e69d2);
        public static readonly Color Coral = new Color(0xff507fff);
        public static readonly Color CornflowerBlue = new Color(0xffed9564);
        public static readonly Color Cornsilk = new Color(0xffdcf8ff);
        public static readonly Color Crimson = new Color(0xff3c14dc);
        public static readonly Color Cyan = new Color(0xffffff00);
        public static readonly Color DarkBlue = new Color(0xff8b0000);
        public static readonly Color DarkCyan = new Color(0xff8b8b00);
        public static readonly Color DarkGoldenrod = new Color(0xff0b86b8);
        public static readonly Color DarkGray = new Color(0xffa9a9a9);
        public static readonly Color DarkGreen = new Color(0xff006400);
        public static readonly Color DarkKhaki = new Color(0xff6bb7bd);
        public static readonly Color DarkMagenta = new Color(0xff8b008b);
        public static readonly Color DarkOliveGreen = new Color(0xff2f6b55);
        public static readonly Color DarkOrange = new Color(0xff008cff);
        public static readonly Color DarkOrchid = new Color(0xffcc3299);
        public static readonly Color DarkRed = new Color(0xff00008b);
        public static readonly Color DarkSalmon = new Color(0xff7a96e9);
        public static readonly Color DarkSeaGreen = new Color(0xff8bbc8f);
        public static readonly Color DarkSlateBlue = new Color(0xff8b3d48);
        public static readonly Color DarkSlateGray = new Color(0xff4f4f2f);
        public static readonly Color DarkTurquoise = new Color(0xffd1ce00);
        public static readonly Color DarkViolet = new Color(0xffd30094);
        public static readonly Color DeepPink = new Color(0xff9314ff);
        public static readonly Color DeepSkyBlue = new Color(0xffffbf00);
        public static readonly Color DimGray = new Color(0xff696969);
        public static readonly Color DodgerBlue = new Color(0xffff901e);
        public static readonly Color Firebrick = new Color(0xff2222b2);
        public static readonly Color FloralWhite = new Color(0xfff0faff);
        public static readonly Color ForestGreen = new Color(0xff228b22);
        public static readonly Color Fuchsia = new Color(0xffff00ff);
        public static readonly Color Gainsboro = new Color(0xffdcdcdc);
        public static readonly Color GhostWhite = new Color(0xfffff8f8);
        public static readonly Color Gold = new Color(0xff00d7ff);
        public static readonly Color Goldenrod = new Color(0xff20a5da);
        public static readonly Color Gray = new Color(0xff808080);
        public static readonly Color Green = new Color(0xff008000);
        public static readonly Color GreenYellow = new Color(0xff2fffad);
        public static readonly Color Honeydew = new Color(0xfff0fff0);
        public static readonly Color HotPink = new Color(0xffb469ff);
        public static readonly Color IndianRed = new Color(0xff5c5ccd);
        public static readonly Color Indigo = new Color(0xff82004b);
        public static readonly Color Ivory = new Color(0xfff0ffff);
        public static readonly Color Khaki = new Color(0xff8ce6f0);
        public static readonly Color Lavender = new Color(0xfffae6e6);
        public static readonly Color LavenderBlush = new Color(0xfff5f0ff);
        public static readonly Color LawnGreen = new Color(0xff00fc7c);
        public static readonly Color LemonChiffon = new Color(0xffcdfaff);
        public static readonly Color LightBlue = new Color(0xffe6d8ad);
        public static readonly Color LightCoral = new Color(0xff8080f0);
        public static readonly Color LightCyan = new Color(0xffffffe0);
        public static readonly Color LightGoldenrodYellow = new Color(0xffd2fafa);
        public static readonly Color LightGray = new Color(0xffd3d3d3);
        public static readonly Color LightGreen = new Color(0xff90ee90);
        public static readonly Color LightPink = new Color(0xffc1b6ff);
        public static readonly Color LightSalmon = new Color(0xff7aa0ff);
        public static readonly Color LightSeaGreen = new Color(0xffaab220);
        public static readonly Color LightSkyBlue = new Color(0xffface87);
        public static readonly Color LightSlateGray = new Color(0xff998877);
        public static readonly Color LightSteelBlue = new Color(0xffdec4b0);
        public static readonly Color LightYellow = new Color(0xffe0ffff);
        public static readonly Color Lime = new Color(0xff00ff00);
        public static readonly Color LimeGreen = new Color(0xff32cd32);
        public static readonly Color Linen = new Color(0xffe6f0fa);
        public static readonly Color Magenta = new Color(0xffff00ff);
        public static readonly Color Maroon = new Color(0xff000080);
        public static readonly Color MediumAquamarine = new Color(0xffaacd66);
        public static readonly Color MediumBlue = new Color(0xffcd0000);
        public static readonly Color MediumOrchid = new Color(0xffd355ba);
        public static readonly Color MediumPurple = new Color(0xffdb7093);
        public static readonly Color MediumSeaGreen = new Color(0xff71b33c);
        public static readonly Color MediumSlateBlue = new Color(0xffee687b);
        public static readonly Color MediumSpringGreen = new Color(0xff9afa00);
        public static readonly Color MediumTurquoise = new Color(0xffccd148);
        public static readonly Color MediumVioletRed = new Color(0xff8515c7);
        public static readonly Color MidnightBlue = new Color(0xff701919);
        public static readonly Color MintCream = new Color(0xfffafff5);
        public static readonly Color MistyRose = new Color(0xffe1e4ff);
        public static readonly Color Moccasin = new Color(0xffb5e4ff);
        public static readonly Color MonoGameOrange = new Color(0xff003ce7);
        public static readonly Color NavajoWhite = new Color(0xffaddeff);
        public static readonly Color Navy = new Color(0xff800000);
        public static readonly Color OldLace = new Color(0xffe6f5fd);
        public static readonly Color Olive = new Color(0xff008080);
        public static readonly Color OliveDrab = new Color(0xff238e6b);
        public static readonly Color Orange = new Color(0xff00a5ff);
        public static readonly Color OrangeRed = new Color(0xff0045ff);
        public static readonly Color Orchid = new Color(0xffd670da);
        public static readonly Color PaleGoldenrod = new Color(0xffaae8ee);
        public static readonly Color PaleGreen = new Color(0xff98fb98);
        public static readonly Color PaleTurquoise = new Color(0xffeeeeaf);
        public static readonly Color PaleVioletRed = new Color(0xff9370db);
        public static readonly Color PapayaWhip = new Color(0xffd5efff);
        public static readonly Color PeachPuff = new Color(0xffb9daff);
        public static readonly Color Peru = new Color(0xff3f85cd);
        public static readonly Color Pink = new Color(0xffcbc0ff);
        public static readonly Color Plum = new Color(0xffdda0dd);
        public static readonly Color PowderBlue = new Color(0xffe6e0b0);
        public static readonly Color Purple = new Color(0xff800080);
        public static readonly Color Red = new Color(0xff0000ff);
        public static readonly Color RosyBrown = new Color(0xff8f8fbc);
        public static readonly Color RoyalBlue = new Color(0xffe16941);
        public static readonly Color SaddleBrown = new Color(0xff13458b);
        public static readonly Color Salmon = new Color(0xff7280fa);
        public static readonly Color SandyBrown = new Color(0xff60a4f4);
        public static readonly Color SeaGreen = new Color(0xff578b2e);
        public static readonly Color SeaShell = new Color(0xffeef5ff);
        public static readonly Color Sienna = new Color(0xff2d52a0);
        public static readonly Color Silver = new Color(0xffc0c0c0);
        public static readonly Color SkyBlue = new Color(0xffebce87);
        public static readonly Color SlateBlue = new Color(0xffcd5a6a);
        public static readonly Color SlateGray = new Color(0xff908070);
        public static readonly Color Snow = new Color(0xfffafaff);
        public static readonly Color SpringGreen = new Color(0xff7fff00);
        public static readonly Color SteelBlue = new Color(0xffb48246);
        public static readonly Color Tan = new Color(0xff8cb4d2);
        public static readonly Color Teal = new Color(0xff808000);
        public static readonly Color Thistle = new Color(0xffd8bfd8);
        public static readonly Color Tomato = new Color(0xff4763ff);
        public static readonly Color Turquoise = new Color(0xffd0e040);
        public static readonly Color Violet = new Color(0xffee82ee);
        public static readonly Color Wheat = new Color(0xffb3def5);
        public static readonly Color White = new Color(0xffffffff);
        public static readonly Color WhiteSmoke = new Color(0xfff5f5f5);
        public static readonly Color Yellow = new Color(0xff00ffff);
        public static readonly Color YellowGreen = new Color(0xff32cd9a);

        private readonly uint abgr;


        public Color(uint abgr)
        {
            this.abgr = abgr;
        }

        public Color(Color color, int alpha)
        {
            if ((alpha & 0xFFFFFF00) != 0)
            {
                var clampedA = (uint)Calc.Clamp(alpha, Byte.MinValue, Byte.MaxValue);

                abgr = (color.abgr & 0x00FFFFFF) | (clampedA << 24);
            }
            else
            {
                abgr = (color.abgr & 0x00FFFFFF) | ((uint)alpha << 24);
            }
        }

        public Color(Color color, float alpha) :
            this(color, (int)(alpha * 255))
        {
        }

        public Color(float r, float g, float b)
            : this((int)(r * 255), (int)(g * 255), (int)(b * 255))
        {
        }

        public Color(float r, float g, float b, float alpha)
            : this((int)(r * 255), (int)(g * 255), (int)(b * 255), (int)(alpha * 255))
        {
        }

        public Color(int r, int g, int b)
        {
            abgr = 0xFF000000; // A = 255

            if (((r | g | b) & 0xFFFFFF00) != 0)
            {
                var clampedR = (uint)Calc.Clamp(r, Byte.MinValue, Byte.MaxValue);
                var clampedG = (uint)Calc.Clamp(g, Byte.MinValue, Byte.MaxValue);
                var clampedB = (uint)Calc.Clamp(b, Byte.MinValue, Byte.MaxValue);

                abgr |= (clampedB << 16) | (clampedG << 8) | clampedR;
            }
            else
            {
                abgr |= ((uint)b << 16) | ((uint)g << 8) | (uint)r;
            }

        }

        public Color(int r, int g, int b, int alpha)
        {
            if (((r | g | b | alpha) & 0xFFFFFF00) != 0)
            {
                var clampedR = (uint)Calc.Clamp(r, Byte.MinValue, Byte.MaxValue);
                var clampedG = (uint)Calc.Clamp(g, Byte.MinValue, Byte.MaxValue);
                var clampedB = (uint)Calc.Clamp(b, Byte.MinValue, Byte.MaxValue);
                var clampedA = (uint)Calc.Clamp(alpha, Byte.MinValue, Byte.MaxValue);

                abgr = (clampedA << 24) | (clampedB << 16) | (clampedG << 8) | (clampedR);
            }
            else
            {
                abgr = ((uint)alpha << 24) | ((uint)b << 16) | ((uint)g << 8) | (uint)r;
            }
        }

        public Color(byte r, byte g, byte b, byte alpha)
        {
            abgr = ((uint)alpha << 24) | ((uint)b << 16) | ((uint)g << 8) | (r);
        }

        public byte A
        {
            get
            {
                unchecked
                {
                    return (byte)(this.abgr >> 24);
                }
            }
        }

        public byte B
        {
            get
            {
                unchecked
                {
                    return (byte)(this.abgr >> 16);
                }
            }
        }

        public byte G
        {
            get
            {
                unchecked
                {
                    return (byte)(this.abgr >> 8);
                }
            }
        }

        public byte R
        {
            get
            {
                unchecked
                {
                    return (byte)this.abgr;
                }
            }
        }

        // ------------------------------------------------------------

        public float Af => (this.abgr >> 24) / 255f;

        public float Bf => (byte)(this.abgr >> 16) / 255f;

        public float Gf => (byte)(this.abgr >> 8) / 255f;

        public float Rf => (byte)this.abgr / 255f;


        public static bool operator ==(Color a, Color b)
        {
            return a.abgr == b.abgr;
        }

        public static bool operator !=(Color a, Color b)
        {
            return a.abgr != b.abgr;
        }

        public override int GetHashCode()
        {
            return this.abgr.GetHashCode();
        }

        public bool Equals(Color obj)
        {
            return this.abgr == obj.abgr;
        }

        public override bool Equals(object obj)
        {
            return obj is Color color && this.Equals(color);
        }

        public static Color Lerp(Color value1, Color value2, float amount)
        {
            amount = Calc.Normalize(amount);
            return new Color(
                (int)Calc.Lerp(value1.R, value2.R, amount),
                (int)Calc.Lerp(value1.G, value2.G, amount),
                (int)Calc.Lerp(value1.B, value2.B, amount),
                (int)Calc.Lerp(value1.A, value2.A, amount));
        }

        public void CopyTo(out Color c)
        {
            c = new Color(abgr);
        }

        public static Color Multiply(Color value, float scale)
        {
            return new Color((int)(value.Rf * scale), (int)(value.Gf * scale), (int)(value.Bf * scale), (int)(value.Af * scale));
        }

        public static Color operator *(Color value, float scale)
        {
            return new Color((int)(value.Rf * scale), (int)(value.Gf * scale), (int)(value.Bf * scale), (int)(value.Af * scale));
        }

        public static Color operator +(Color value1, Color value2)
        {
            return new Color(value1.Rf + value2.Rf, value1.Gf + value2.Gf, value1.Bf + value2.Bf, value1.Af + value2.Af);
        }

        public static Color operator -(Color value1, Color value2)
        {
            return new Color(value1.Rf - value2.Rf, value1.Gf - value2.Gf, value1.Bf - value2.Bf, value1.Af - value2.Af);
        }


        public static implicit operator Color(int rgba)
        {
            uint abgr = ((uint)((byte)(rgba & 0x000000FF)) << 24) | ((uint)((byte)((rgba & 0x0000FF00) >> 8)) << 16) | ((uint)((byte)((rgba & 0x00FF0000) >> 16)) << 8) | ((uint)((byte)((rgba & 0xFF000000) >> 24)));

            return new Color(abgr);
        }

        public static implicit operator int(Color color)
        {
            return color.RGBA;
        }

        public void Deconstruct(out float r, out float g, out float b)
        {
            r = R;
            g = G;
            b = B;
        }

        public void Deconstruct(out float r, out float g, out float b, out float a)
        {
            r = R;
            g = G;
            b = B;
            a = A;
        }

        public Color WithOpacity(float opacity)
        {
            opacity = Calc.Normalize(opacity);

            return new Color(this, opacity);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(25);
            sb.Append("{R:");
            sb.Append(R);
            sb.Append(" G:");
            sb.Append(G);
            sb.Append(" B:");
            sb.Append(B);
            sb.Append(" A:");
            sb.Append(A);
            sb.Append("}");
            return sb.ToString();
        }

        public uint ABGR => abgr;

        public int RGBA => (R << 24) | (G << 16) | (B << 8) | A;

        public int CompareTo(Color other)
        {
            return abgr.CompareTo(other.abgr);
        }

        public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Color));
    }
}
