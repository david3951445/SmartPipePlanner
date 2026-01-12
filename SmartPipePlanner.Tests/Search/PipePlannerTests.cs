using SmartPipePlanner.Core;
using SmartPipePlanner.Core.Search;

namespace SmartPipePlanner.Tests.Search;

public class PipePlannerTests
{
    [Fact]
    public void PlanPaths_NoObstacles_ReturnsPaths()
    {
        int size = 5;
        var grid = new Grid(size, size, size);
        var planner = new PipePlanner(grid);

        Coordinate[] starts = [new(0, 0, 0), new(1, 0, 0)];
        Coordinate[] ends = [new(size - 1, size - 1, size - 1), new(size - 2, size - 1, size - 1)];
        Problem[] problems =
        [
            new Problem(starts[0],
                        Direction.PosX,
                        ends[0],
                        PipeCategory.HotWaterPipe),
            new Problem(starts[1],
                        Direction.PosX,
                        ends[1],
                        PipeCategory.ColdWaterPipe)
        ];

        // Act & Assert
        Pipe[]?[] paths = planner.PlanPaths(problems);
        Assert.InRange(paths[0]!.Length, 0, starts[0].ManhattanDistance(ends[0]) + 1);
        Assert.InRange(paths[1]!.Length, 0, starts[1].ManhattanDistance(ends[1]) + 1);
    }

    [Fact]
    public void PlanPaths_SecondCollidesFirst_ReturnsFirst()
    {
        var grid = new Grid(3, 3, 1);
        grid.SetBox(new Coordinate(1, 0, 0), new Coordinate(1, 2, 0), CellType.Obstacle);
        grid.SetBox(new Coordinate(1, 2, 0), new Coordinate(1, 0, 0), CellType.Obstacle);
        var planner = new PipePlanner(grid);

        Coordinate[] starts = [new(0, 0, 0), new(0, 2, 0)];
        Coordinate[] ends = [new(2, 2, 0), new(2, 0, 0)];
        Problem[] problems =
        [
            new Problem(starts[0],
                        Direction.PosX,
                        ends[0],
                        PipeCategory.HotWaterPipe),
            new Problem(starts[1],
                        Direction.PosX,
                        ends[1],
                        PipeCategory.ColdWaterPipe)
        ];

        // Act & Assert
        Pipe[]?[] paths = planner.PlanPaths(problems);
        Assert.InRange(paths[0]!.Length, 0, starts[0].ManhattanDistance(ends[0]) + 1);
        Assert.Null(paths[1]);
    }
}
