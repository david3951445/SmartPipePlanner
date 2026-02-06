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

public static class PipeExtensions
{
    public static Direction Inverse(this Direction d) => d switch
    {
        Direction.PosX => Direction.NegX,
        Direction.NegX => Direction.PosX,
        Direction.PosY => Direction.NegY,
        Direction.NegY => Direction.PosY,
        Direction.PosZ => Direction.NegZ,
        Direction.NegZ => Direction.PosZ,
        _ => throw new InvalidOperationException()
    };

    public static Quaternion GetOrientation(this Pipe pipe)
    {
        var d = pipe.Direction;
        var ld = pipe.LPipeDirection;

        var x1 = Vector3.UnitX;
        var y1 = Vector3.UnitY;
        var z1 = Vector3.UnitZ;

        var c1 = Coordinate.FromDirection(d);
        var x2 = new Vector3(c1.X, c1.Y, c1.Z);

        Vector3 y2;
        if (ld == null)
        {
            // pick a vector that is vertical to x2
            y2 = Vector3.UnitY;
            if (Vector3.Dot(x2, y2) > 0.99f)
                y2 = Vector3.UnitZ;
        }
        else
        {
            var c2 = Coordinate.FromDirection(ld.Value);
            y2 = new Vector3(c2.X, c2.Y, c2.Z);
        }

        var z2 = Vector3.Cross(x2, y2);

        return Extensions.QuaternionBetweenFrames(
            x1, y1, z1,
            x2, y2, z2);
    }
}