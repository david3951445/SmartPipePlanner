using System.Numerics;

namespace SmartPipePlanner.Core;

public static class Extensions
{
    public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0) return min;
        if (value.CompareTo(max) > 0) return max;
        return value;
    }

    public static Quaternion ToQuaternion(this Vector3 euler)
    {
        // Convert degrees to radians
        float roll = euler.X * MathF.PI / 180f;
        float pitch = euler.Y * MathF.PI / 180f;
        float yaw = euler.Z * MathF.PI / 180f;

        float cy = MathF.Cos(yaw * 0.5f);
        float sy = MathF.Sin(yaw * 0.5f);
        float cp = MathF.Cos(pitch * 0.5f);
        float sp = MathF.Sin(pitch * 0.5f);
        float cr = MathF.Cos(roll * 0.5f);
        float sr = MathF.Sin(roll * 0.5f);

        float w = cr * cp * cy + sr * sp * sy;
        float x = sr * cp * cy - cr * sp * sy;
        float y = cr * sp * cy + sr * cp * sy;
        float z = cr * cp * sy - sr * sp * cy;

        return new Quaternion(x, y, z, w);
    }

    // https://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles
    public static Vector3 ToEulerAngles(this Quaternion q)
    {
        // Roll (X-axis rotation)
        float sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
        float cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
        float roll = MathF.Atan2(sinr_cosp, cosr_cosp);

        // Pitch (Y-axis rotation)
        float sinp = 2 * (q.W * q.Y - q.Z * q.X);
        sinp = sinp.Clamp(-1f, 1f); // clamp for numeric stability
        float pitch = MathF.Asin(sinp);

        // Yaw (Z-axis rotation)
        float siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
        float cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
        float yaw = MathF.Atan2(siny_cosp, cosy_cosp);

        return new Vector3(
            roll * 180f / MathF.PI,
            pitch * 180f / MathF.PI,
            yaw * 180f / MathF.PI
        );
    }

    public static Quaternion QuaternionBetweenFrames(
        Vector3 x1, Vector3 y1, Vector3 z1,   // Frame 1 axes
        Vector3 x2, Vector3 y2, Vector3 z2)   // Frame 2 axes
    {
        // Build 4x4 rotation matrices (columns = frame axes)
        var R1 = new Matrix4x4(
            x1.X, x1.Y, x1.Z, 0,
            y1.X, y1.Y, y1.Z, 0,
            z1.X, z1.Y, z1.Z, 0,
            0, 0, 0, 1
        );

        var R2 = new Matrix4x4(
            x2.X, x2.Y, x2.Z, 0,
            y2.X, y2.Y, y2.Z, 0,
            z2.X, z2.Y, z2.Z, 0,
            0, 0, 0, 1
        );

        // Compute relative rotation: R = R2 * R1^T
        var R = Matrix4x4.Multiply(R2, Matrix4x4.Transpose(R1));

        // Convert to quaternion
        return Quaternion.CreateFromRotationMatrix(R);
    }
}