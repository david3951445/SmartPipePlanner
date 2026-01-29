using HelixToolkit.Geometry;
using HelixToolkit.Wpf;
using SmartPipePlanner.Core;
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
        meshBuilder.AddBox(center, (float)width, (float)height, (float)depth);
        return AddMesh(meshBuilder, color);
    }

    public static ModelVisual3D AddSphere(Point3D center, double radius, Color color)
    {
        var meshBuilder = new MeshBuilder();
        meshBuilder.AddSphere(center.ToVector3(), (float)radius);
        return AddMesh(meshBuilder, color);
    }

    // Pipe1: start + orientation (Euler angles in degrees) + length
    public static ModelVisual3D AddPipe(Vector3 start, Vector3 orientation, float length, Color color)
    {
        var meshBuilder = new MeshBuilder();

        var q = orientation.ToQuaternion();
        var direction = Vector3.Transform(Vector3.UnitX, q); // 預設沿 X 軸
        start -= direction / 2;
        var end = start + direction * length;

        meshBuilder.AddCylinder(start, end, 0.2f, 16);
        return AddMesh(meshBuilder, color);
    }

    public static ModelVisual3D AddLPipe(Vector3 start, Vector3 orientation, float length, Color color)
    {
        var meshBuilder = new MeshBuilder();

        // orientation 控制整個 L-Pipe 的旋轉
        var q = orientation.ToQuaternion();

        // 預設 L-Pipe：第一段沿 X，第二段沿 Y
        var firstDir = Vector3.Transform(Vector3.UnitX, q);
        var secondDir = Vector3.Transform(Vector3.UnitY, q);

        var end1 = start + firstDir * length / 2;
        var end2 = start + secondDir * length / 2;

        meshBuilder.AddCylinder(start, end1, 0.2f, 16); // 第一段
        meshBuilder.AddCylinder(start, end2, 0.2f, 16);   // 第二段

        return AddMesh(meshBuilder, color);
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
