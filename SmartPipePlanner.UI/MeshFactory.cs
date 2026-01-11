using HelixToolkit.Geometry;
using HelixToolkit.Wpf;
using System.Numerics;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace SmartPipePlanner.UI;

public static class MeshFactory
{
    // Box: center + width/height/depth
    public static ModelVisual3D AddBox(Vector3 center, double width, double height, double depth, Color color)
    {
        var meshBuilder = new MeshBuilder();
        center += new Vector3(0.5f, 0.5f, 0.5f);
        meshBuilder.AddBox(center, (float)width, (float)height, (float)depth);
        return AddMesh(meshBuilder, color);
    }

    // Pipe1: start + orientation (Euler angles in degrees) + length
    public static ModelVisual3D AddPipe(Vector3 start, Vector3 orientation, float length, Color color)
    {
        var meshBuilder = new MeshBuilder();

        var q = GetQuaternion(orientation);
        var direction = Vector3.Transform(Vector3.UnitX, q); // 預設沿 X 軸
        var end = start + direction * length;

        meshBuilder.AddCylinder(start, end, 0.2f, 16);
        return AddMesh(meshBuilder, color);
    }

    public static ModelVisual3D AddLPipe(Vector3 start, Vector3 orientation, float length, Color color)
    {
        var meshBuilder = new MeshBuilder();

        // orientation 控制整個 L-Pipe 的旋轉
        var q = GetQuaternion(orientation);

        // 預設 L-Pipe：第一段沿 X，第二段沿 Y
        var firstDir = Vector3.Transform(Vector3.UnitX, q);
        var secondDir = Vector3.Transform(Vector3.UnitY, q);

        var mid = start + firstDir * length;
        var end = mid + secondDir * length;

        meshBuilder.AddCylinder(start, mid, 0.2f, 16); // 第一段
        meshBuilder.AddCylinder(mid, end, 0.2f, 16);   // 第二段

        return AddMesh(meshBuilder, color);
    }

    static System.Numerics.Quaternion GetQuaternion(Vector3 orientation)
    {
        // orientation.X = Pitch, Y = Yaw, Z = Roll
        return System.Numerics.Quaternion.CreateFromYawPitchRoll(
            MathF.PI / 180 * orientation.Y, // Yaw
            MathF.PI / 180 * orientation.X, // Pitch
            MathF.PI / 180 * orientation.Z  // Roll
        );
    }

    // 將 MeshBuilder 轉成 ModelVisual3D
    public static ModelVisual3D AddMesh(MeshBuilder meshBuilder, Color color)
    {
        var mesh = meshBuilder.ToMesh();
        var model = new GeometryModel3D
        {
            Geometry = mesh.ToWndMeshGeometry3D(),
            Material = new DiffuseMaterial(new SolidColorBrush(color))
        };

        return new ModelVisual3D { Content = model };
    }
}
