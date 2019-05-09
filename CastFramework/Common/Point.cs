using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CastFramework
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct Point : IEquatable<Point>
    {
        private static readonly Point _zero = new Point();

        public ref readonly Point Zero => ref _zero;

        public readonly int X;
        public readonly int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point(int val) : this(val, val)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is Point point2 && Equals(point2);
        }

        public bool Equals(Point other)
        {
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + X;
                hash = hash * 23 + Y;
                return hash;
            }
        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }

        public static implicit operator Vector2(Point p)
        {
            return new Vector2(p.X, p.Y);
        }

        public static bool operator ==(Point a, Point b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Point a, Point b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }

        public static Point operator -(Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }

        public static Point operator *(Point a, Point b)
        {
            return new Point(a.X * b.X, a.Y * b.Y);
        }

        public static Point operator *(Point p, int n)
        {
            return new Point(p.X * n, p.Y / n);
        }

        public static Point operator /(Point a, Point b)
        {
            return new Point(a.X / b.X, a.Y / b.Y);
        }

        public static Point operator /(Point p, int n)
        {
            return new Point(p.X / n, p.Y / n);
        }
    }
}
