using System;

namespace SlimMath
{
    public static class Single
    {
        public const float Rad2Deg = 57.29578f;
        public const float Deg2Rad = 0.01745329f;

        public static byte EulerToByte(float a)
        {
            return (byte)(a * 0.71111f);
        }

        public static byte RadianToByte(float r)
        {
            return (byte)(r * 81.48739);
        }

        public static float ByteToEuler(byte b)
        {
            return b * 1.40625f;
        }

        public static float ByteToRadian(byte b)
        {
            return b * 0.012271f;
        }

        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
            {
                return min;
            }

            if (value > max)
            {
                return max;
            }

            return value;
        }

        public static float Clamp01(float value)
        {
            if (value < 0f)
            {
                return 0f;
            }

            if (value > 1f)
            {
                return 1f;
            }

            return value;

        }

        public static float Lerp(float from, float to, float t)
        {
            return (from + ((to - from) * Clamp01(t)));
        }

        public static float SmoothStep(float from, float to, float t)
        {
            t = Clamp01(t);
            t = (((-2f * t) * t) * t) + ((3f * t) * t);
            return ((to * t) + (from * (1f - t)));
        }

        public static int FloorToInt(float f)
        {
            return (int)Math.Floor((double)f);
        }

        public static int CeilToInt(float f)
        {
            return (int)Math.Ceiling((double)f);
        }
    }
}
