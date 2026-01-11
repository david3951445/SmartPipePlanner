using System.Numerics;

namespace SmartPipePlanner.Data;

public class Element
{
    public int Id { get; set; }
    public ElementCategory Category { get; set; }
    public required Geometry Geometry { get; set; }
    public Vector3 Location { get; set; }
    public double Price { get; set; }
}

public enum ElementCategory
{
    HotWaterPipe,
    ColdWaterPipe,
    Obstacle
}

public class Geometry
{
    public Vector3 Orientation { get; set; }
    public GeometryType Type { get; set; }
}

public enum GeometryType
{
    Box,
    Pipe1,
    LPipe1,
    Pipe2
}
