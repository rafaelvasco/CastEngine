using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace CastFramework
{
    public static class Calc
    {
        public const float E = (float)Math.E;
        public const float PI = 3.14159265358979323846f;
        public const float PI_OVER2 = 1.57079632679489661923f;
        public const float PI_OVER4 = 0.785398163397448309616f;
        public const float PI_OVER6 = 0.523598775598299f;
        public const float TWO_PI = 6.28318530717959f;

        public const float RAD_ANGLE30 = PI_OVER6;
        public const float RAD_ANGLE45 = PI_OVER4;
        public const float RAD_ANGLE90 = PI_OVER2;
        public const float RAD_ANGLE180 = PI;
        public const float RAD_ANGLE360 = TWO_PI;

        private const float RADIANS_TO_DEGREES_FACTOR = 180f / PI;
        private const float DEGREES_TO_RADIANS_FACTOR = PI / 180f;
        private const int SIN_BITS = 13;
        private const int SIN_MASK = ~(-1 << SIN_BITS);
        private const int SIN_COUNT = SIN_MASK + 1;
        private const float RAD_FULL = PI * 2;
        private const float DEG_FULL = 360;
        private const float RAD_TO_INDEX = SIN_COUNT / RAD_FULL;
        private const float DEG_TO_INDEX = SIN_COUNT / DEG_FULL;

        private static readonly float[] sinBuffer = new float[SIN_COUNT];
        private static readonly float[] cosBuffer = new float[SIN_COUNT];

        static Calc()
        {
            for (int i = 0; i < SIN_COUNT; i++)
            {
                float angle = (i + 0.5f) / SIN_COUNT * RAD_FULL;
                sinBuffer[i] = (float)Math.Sin(angle);
                cosBuffer[i] = (float)Math.Cos(angle);
            }

            for (int i = 0; i < 360; i += 90)
            {
                sinBuffer[(int)(i * DEG_TO_INDEX) & SIN_MASK] = (float)Math.Sin(i * DEGREES_TO_RADIANS_FACTOR);
                cosBuffer[(int)(i * DEG_TO_INDEX) & SIN_MASK] = (float)Math.Cos(i * DEGREES_TO_RADIANS_FACTOR);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Abs(int value)
        {
            var mask = value >> 31;
            return (value ^ mask) - mask;
        }

        public static float Abs(float value)
        {
            return Math.Abs(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Round(float value)
        {
            return (float)Math.Round(value);
        }

        public static int RoundToInt(float f)
        {
            return (int)Math.Round(f);
        }

        public static float Ceil(float value)
        {
            return (float)Math.Ceiling(value);
        }

        public static int CeilToInt(float value)
        {
            return (int)Math.Ceiling(value);
        }

        /// <summary>
        /// Ceils the float to the nearest int value above y. note that this only works for values in the range of short
        /// </summary>
        /// <returns>The ceil to int.</returns>
        /// <param name="value">F.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FastCeilToInt(float value)
        {
            return 32768 - (int)(32768f - value);
        }

        public static float Floor(float value)
        {
            return (float)Math.Floor(value);
        }

        public static int FloorToInt(float f)
        {
            return (int)Math.Floor(f);
        }

        /// <summary>
        /// Floors the float to the nearest int value below x. note that this only works for values in the range of short
        /// </summary>
        /// <returns>The floor to int.</returns>
        /// <param name="x">The x coordinate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FastFloorToInt(float x)
        {
            return (int)(x + 32768f) - 32768;
        }

        /// <summary>
        /// Clamps float between 0f and 1f
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Normalize(float value)
        {
            if (value < 0f)
                return 0f;

            return value > 1f ? 1f : value;
        }

        public static float Pow(float x, float y)
        {
            return (float)Math.Pow(x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float value, float min, float max)
        {
            return (value > max) ? max : ((value < min) ? min : value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(int value, int min, int max)
        {
            return (value > max) ? max : ((value < min) ? min : value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clamp(Array array, ref int index)
        {
            index = Clamp(index, 0, array.Length - 1);
        }

        /// <summary>
        /// Restricts a value to be multiple of increment.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="increment"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Snap(float value, float increment)
        {
            return Round(value / increment) * increment;
        }

        public static float CeilSnap(float value, float increment)
        {
            return (Ceil(value / increment) * increment);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float a, float b, float x) => a + (b - a) * x;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float UnclampedLerp(float from, float to, float t)
        {
            return from + (to - from) * t;
        }

        /// <summary>
        /// Lerps an angle in degrees between a and b. handles wrapping around 360
        /// </summary>
        /// <returns>The angle.</returns>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="t">T.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LerpAngle(float a, float b, float t)
        {
            float num = Repeat(b - a, 360f);
            if (num > 180f)
                num -= 360f;

            return a + num * Normalize(t);
        }

        /// <summary>
        /// Loops t so that it is never larger than length and never smaller than 0
        /// </summary>
        /// <param name="t">T.</param>
        /// <param name="length">Length.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Repeat(float t, float length)
        {
            return t - Floor(t / length) * length;
        }

        /// <summary>
        /// Increments t and ensures it is always greater than or equal to 0 and less than length
        /// </summary>
        /// <param name="t">T.</param>
        /// <param name="length">Length.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IncrementWithWrap(int t, int length)
        {
            t++;
            if (t == length)
                return 0;
            return t;
        }

        /// <summary>
        /// Decrements t and ensures it is always greater than or equal to 0 and less than length
        /// </summary>
        /// <returns>The with wrap.</returns>
        /// <param name="t">T.</param>
        /// <param name="length">Length.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int DecrementWithWrap(int t, int length)
        {
            t--;
            if (t < 0)
                return length - 1;
            return t;
        }

        /// <summary>
        /// ping-pongs t so that it is never larger than length and never smaller than 0
        /// </summary>
        /// <returns>The pong.</returns>
        /// <param name="t">T.</param>
        /// <param name="length">Length.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float PingPong(float t, float length)
        {
            t = Repeat(t, length * 2f);
            return length - Abs(t - length);
        }

        /// <summary>
        /// Calculates the shortest difference between two given angles in degrees
        /// </summary>
        /// <returns>The angle.</returns>
        /// <param name="current">Current.</param>
        /// <param name="target">Target.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DeltaAngle(float current, float target)
        {
            var num = Repeat(target - current, 360f);
            if (num > 180f)
                num -= 360f;

            return num;
        }

        /// <summary>
        /// Calculates the shortest difference between two given angles given in radians
        /// </summary>
        /// <returns>The angle.</returns>
        /// <param name="current">Current.</param>
        /// <param name="target">Target.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DeltaAngleRadians(float current, float target)
        {
            var num = Repeat(target - current, TWO_PI);
            if (num > PI)
                num -= TWO_PI;

            return num;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Min(float a, float b)
        {
            return a < b ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Min(float a, float b, float c)
        {
            return a < b ? (a < c ? a : c) : (b < c ? b : c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Min(float a, float b, float c, float d)
        {
            return Min(d, Min(a, Min(b, c)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(float a, float b)
        {
            return a > b ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(float a, float b, float c)
        {
            return a < b ? (b < c ? c : b) : (a < c ? c : b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(float a, float b, float c, float d)
        {
            return Max(d, Max(a, Max(b, c)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Min(int a, int b)
        {
            return a < b ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Min(int a, int b, int c)
        {
            return a < b ? (a < c ? a : c) : (b < c ? b : c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Min(int a, int b, int c, int d)
        {
            return Min(d, Min(a, Min(b, c)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Max(int a, int b)
        {
            return a > b ? a : b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Max(int a, int b, int c)
        {
            return a < b ? (b < c ? c : b) : (a < c ? c : b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Max(int a, int b, int c, int d)
        {
            return Max(d, Max(a, Max(b, c)));
        }

        /// <summary>
        /// Moves start towards end by shift amount clamping the result. start can be less than or greater than end.
        /// example: start is 2, end is 10, shift is 4 results in 6
        /// </summary>
        /// <param name="start">Start.</param>
        /// <param name="end">End.</param>
        /// <param name="shift">Shift.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Approach(float start, float end, float shift)
        {
            if (start < end)
                return Math.Min(start + shift, end);
            return Math.Max(start - shift, end);
        }

        public static float Accelerate(float velocity, float minSpeed, float maxSpeed, float acceleration, float dt)
        {
            float min = minSpeed * dt;
            float max = maxSpeed * dt;

            return Clamp(velocity * dt + 0.5f * acceleration * dt * dt, min, max);
        }

        /// <summary>
        /// checks to see if two values are approximately the same using an acceptable tolerance for the check
        /// </summary>
        /// <param name="value1">Value1.</param>
        /// <param name="value2">Value2.</param>
        /// <param name="tolerance">Tolerance.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ApproximatelyEqual(float value1, float value2, float tolerance = E)
        {
            return Math.Abs(value1 - value2) <= tolerance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sin(float rad)
        {
            return sinBuffer[(int)(rad * RAD_TO_INDEX) & SIN_MASK];
        }

        public static float ASin(float v)
        {
            return (float)Math.Asin(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cos(float rad)
        {
            return cosBuffer[(int)(rad * RAD_TO_INDEX) & SIN_MASK];
        }

        public static float ACos(float v)
        {
            return (float)Math.Acos(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SinDeg(float deg)
        {
            return sinBuffer[(int)(deg * DEG_TO_INDEX) & SIN_MASK];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CosDeg(float deg)
        {
            return cosBuffer[(int)(deg * DEG_TO_INDEX) & SIN_MASK];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToRadians(float degree)
        {
            return degree * DEGREES_TO_RADIANS_FACTOR;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToDegrees(float radian)
        {
            return radian * RADIANS_TO_DEGREES_FACTOR;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleBetweenVectors(Vector2 from, Vector2 to)
        {
            return (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sqrt(float val)
        {
            return (float)Math.Sqrt(val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SqrtI(int val)
        {
            if (val == 0)
            {
                return 0;
            }

            int n = (val / 2) + 1;
            int n1 = (n + (val / n)) / 2;

            while (n1 < n)
            {
                n = n1;
                n1 = (n + (val / n)) / 2;
            }

            return n;
        }

        /// <summary>
        /// Gets a point on the circumference of the circle given its center, radius and angle. 0 degrees is 3 o'clock.
        /// </summary>
        /// <returns>The on circle.</returns>
        /// <param name="circleCenter">Circle center.</param>
        /// <param name="radius">Radius.</param>
        /// <param name="angleInDegrees">Angle in degrees.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 PointOnCircle(Vector2 circleCenter, float radius, float angleInDegrees)
        {
            var radians = ToRadians(angleInDegrees);
            return new Vector2
            {
                X = Cos(radians) * radius + circleCenter.X,
                Y = Sin(radians) * radius + circleCenter.Y
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 RotateAround(Vector2 point, Vector2 center, float angleRadians)
        {
            var cos = Cos(angleRadians);
            var sin = Sin(angleRadians);
            var rotatedX = cos * (point.X - center.X) - sin * (point.Y - center.Y) + center.X;
            var rotatedY = sin * (point.X - center.X) + cos * (point.Y - center.Y) + center.Y;

            return new Vector2(rotatedX, rotatedY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NextPowerOfTwo(int value)
        {
            if (value == 0)
            {
                return 1;
            }

            value -= 1;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return value + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPowerOfTwo(int value) => value != 0 && (value & value - 1) == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NormalizeVar(float var, float min, float max)
        {
            if (var >= min && var < max) return var;

            if (var < min)
                var = max + ((var - min) % max);
            else
                var = min + var % (max - min);

            return var;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(float x1, float y1, float x2, float y2)
        {
            return (float)Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }

        public static float IntBitsToFloat(int value)
        {
            var bytes = BitConverter.GetBytes(value & 0xFEFFFFFF);
            return BitConverter.ToSingle(bytes, 0);
        }

        public static bool PointInRect(float x, float y, float rx, float ry, float rw, float rh)
        {
            if (x <= rx) return false;
            if (x >= rx + rw) return false;
            if (y <= ry) return false;
            if (y >= ry + rh) return false;

            return true;
        }

        public static float DistanceRectPoint(float px, float py, float rx, float ry, float rw, float rh)
        {
            if (px >= rx && px <= rx + rw)
            {
                if (py >= ry && py <= ry + rh) return 0;

                if (py > ry) return py - (ry + rh);

                return ry - py;
            }

            if (py >= ry && py <= ry + rh)
            {
                if (px > rx) return px - (rx + rw);

                return rx - px;
            }

            if (px > rx)
            {
                if (py > ry) return Distance(px, py, rx + rw, ry + rh);

                return Distance(px, py, rx + rw, ry);
            }

            if (py > ry) return Distance(px, py, rx, ry + rh);

            return Distance(px, py, rx, ry);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap(ref float v1, ref float v2)
        {
            var temp = v1;
            v1 = v2;
            v2 = temp;
        }
    }
}
