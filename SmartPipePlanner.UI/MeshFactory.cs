using HelixToolkit.Geometry;
using HelixToolkit.Wpf;
using System.Numerics;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Quaternion = System.Numerics.Quaternion;

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

    public static ModelVisual3D AddPipe(Vector3 start, Quaternion quaternion, float length, Color color)
    {
        var meshBuilder = new MeshBuilder();

        var direction = Vector3.Transform(Vector3.UnitX, quaternion); // 預設沿 X 軸
        start -= direction / 2;
        var end = start + direction * length;

        meshBuilder.AddCylinder(start, end, 0.2f, 16);
        return AddMesh(meshBuilder, color);
    }

    public static ModelVisual3D AddLPipe(Vector3 start, Quaternion quaternion, float length, Color color)
    {
        var meshBuilder = new MeshBuilder();

        // 預設 L-Pipe：第一段沿 X，第二段沿 Y
        var firstDir = Vector3.Transform(Vector3.UnitX, quaternion);
        var secondDir = Vector3.Transform(Vector3.UnitY, quaternion);

        var end1 = start + firstDir * length / 2;
        var end2 = start + secondDir * length / 2;

        meshBuilder.AddCylinder(start, end1, 0.2f, 16); // 第一段
        meshBuilder.AddCylinder(start, end2, 0.2f, 16);   // 第二段
        meshBuilder.AddSphere(start, 0.1f); // 連接處
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
