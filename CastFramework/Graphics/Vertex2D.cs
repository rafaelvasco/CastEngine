using System.Runtime.InteropServices;

namespace CastFramework
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex2D
    {
        public float X;

        public float Y;

        public float Tx;

        public float Ty;

        public uint Col;

        public Vertex2D(float x, float y, float tx, float ty, uint abgr)
        {
            this.X = x;
            this.Y = y;
            this.Tx = tx;
            this.Ty = ty;
            this.Col = abgr;
        }

        public static int Stride => 20;

       
    }
}
