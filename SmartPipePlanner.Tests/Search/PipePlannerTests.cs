using SmartPipePlanner.Core;
using SmartPipePlanner.Core.Search;

namespace SmartPipePlanner.Tests.Search;

public class PipePlannerTests
{
    [Fact]
    public void PlanPaths_NotNull()
    {
        int size = 5;
        var grid = new Grid(size, size, size);
        var planner = new PipePlanner(grid);

        Coordinate start = new(0, 0, 0);
        Direction startDir = Direction.PosX;
        Coordinate goal = new(size - 1, size - 1, size - 1);
        Coordinate offset = new(1, 0, 0);

        Problem[] problems = 
        [
            new(start, startDir, goal, PipeCategory.HotWaterPipe),
            new(start + offset, startDir, goal + offset, PipeCategory.ColdWaterPipe)
        ];

        var result = planner.PlanPaths(problems);
        Assert.NotNull(result);
    }
}
