using System.Numerics;
using SmartPipePlanner.Core;
using SmartPipePlanner.Core.Search;

namespace SmartPipePlanner.Data;

public class Element
{
    public int Id { get; set; }
    public ElementCategory Category { get; set; }
    public required Geometry Geometry { get; set; }
    public Vector3 Location { get; set; }
    public double Price { get; set; }
}

public class ProblemDto
{
    public int Id { get; set; }
    public Coordinate Start { get; set; }
    public Direction StartDir { get; set; }
    public Coordinate End { get; set; }
    public PipeCategory Category { get; set; }

    public Problem ToProblem() => new(Start, StartDir, End, Category);
    public static ProblemDto FromProblem(Problem value) => new()
    {
        Start = value.Start,
        StartDir = value.StartDir,
        End = value.End,
        Category = value.Category
    };
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
