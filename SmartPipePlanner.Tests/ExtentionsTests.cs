using System.Numerics;
using SmartPipePlanner.Core;

namespace SmartPipePlanner.Tests;

public class ExtensionsTests
{
    [Theory]
    [InlineData(0f, 0f, 0f)]
    [InlineData(45f, 0f, 0f)]
    [InlineData(0f, 90f, 0f)]
    [InlineData(0f, 0f, 180f)]
    [InlineData(30f, 60f, 90f)]
    [InlineData(-45f, -90f, 120f)] // this fail
    [InlineData(90f, 180f, -90f)]
    public static void ToQuaternionAndBack_RoundTrip(float x, float y, float z)
    {
        // Euler angles in degrees
        Vector3 euler = new(x, y, z);

        Quaternion q1 = euler.ToQuaternion();
        Quaternion q2 = euler.ToQuaternion().ToEulerAngles().ToQuaternion();

        // Normalize (optional)
        q1 = Quaternion.Normalize(q1);
        q2 = Quaternion.Normalize(q2);

        // They are equivalent if dot ≈ 1 or -1
        float dot = Quaternion.Dot(q1, q2);
        Assert.True(MathF.Abs(dot) > 1 - 1e-3f, $"Round-trip failed: dot={dot}");
    }

    [Fact]
    public static void IdentityQuaternion_ReturnsZeroEuler()
    {
        Quaternion q = Quaternion.Identity;
        Vector3 euler = q.ToEulerAngles();

        Assert.Equal(0f, euler.X, 3);
        Assert.Equal(0f, euler.Y, 3);
        Assert.Equal(0f, euler.Z, 3);
    }

    public static TheoryData<Vector3, Vector3, Vector3, Vector3, Vector3, Vector3> FrameData => new()
    {
        {
            new(1, 0, 0), new(0, 1, 0), new(0, 0, 1), // Frame 1 axes
            new(1, 0, 0), new (0, 0, 1), new(0, -1, 0) // Frame 2 axes
        },
        {
            new(1, 0, 0), new(0, 1, 0), new(0, 0, 1),
            new(0, 1, 0), new(-1, 0, 0), new(0, 0, 1)
        }
    };

    [Theory]
    [MemberData(nameof(FrameData))]
    public void QuaternionBetweenFrames_RotatesAxesCorrectly(
        Vector3 x1, Vector3 y1, Vector3 z1,
        Vector3 x2, Vector3 y2, Vector3 z2)
    {
        // Compute quaternion
        Quaternion q = Extensions.QuaternionBetweenFrames(x1, y1, z1, x2, y2, z2);

        // Rotate frame 1 axes
        Vector3 x1_rot = Vector3.Transform(x1, q);
        Vector3 y1_rot = Vector3.Transform(y1, q);
        Vector3 z1_rot = Vector3.Transform(z1, q);

        float epsilon = 1e-4f;

        // Assert each axis approximately matches target frame
        Assert.InRange(x1_rot.X, x2.X - epsilon, x2.X + epsilon);
        Assert.InRange(x1_rot.Y, x2.Y - epsilon, x2.Y + epsilon);
        Assert.InRange(x1_rot.Z, x2.Z - epsilon, x2.Z + epsilon);

        Assert.InRange(y1_rot.X, y2.X - epsilon, y2.X + epsilon);
        Assert.InRange(y1_rot.Y, y2.Y - epsilon, y2.Y + epsilon);
        Assert.InRange(y1_rot.Z, y2.Z - epsilon, y2.Z + epsilon);

        Assert.InRange(z1_rot.X, z2.X - epsilon, z2.X + epsilon);
        Assert.InRange(z1_rot.Y, z2.Y - epsilon, z2.Y + epsilon);
        Assert.InRange(z1_rot.Z, z2.Z - epsilon, z2.Z + epsilon);
    }
}