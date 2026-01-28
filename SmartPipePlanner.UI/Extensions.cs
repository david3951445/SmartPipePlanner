using System.Numerics;

namespace SmartPipePlanner.UI
{
    public static class Extensions
    {
        public static float AngleBetween(this Vector3 a, Vector3 b)
        {
            float dot = Vector3.Dot(a, b);
            float mag = a.Length() * b.Length();
            if (mag == 0f)
                return float.NaN;

            float cos = dot / mag;
            cos = cos.Clamp(-1f, 1f);
            return MathF.Acos(cos); // radians
        }

        public static float AngleBetweenDegrees(this Vector3 a, Vector3 b)
        {
            return a.AngleBetween(b) * 180f / MathF.PI;
        }

        public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0) return min;
            if (value.CompareTo(max) > 0) return max;
            return value;
        }
    }
}