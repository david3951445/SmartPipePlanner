using SmartPipePlanner.Core;
using SmartPipePlanner.Core.Search;

namespace SmartPipePlanner.Tests.Search;

public class AStarPlannerTests
{
    [Fact]
    public void PlanPath_NoObstacles_ReturnsPath()
    {
        int size = 5;
        var grid = new Grid(size, size, size);
        var planner = new AStarPlanner(grid);
        Coordinate start = new(0, 0, 0);
        Coordinate goal = new(size - 1, size - 1, size - 1);

        // Act & Assert
        var path = planner.PlanPath(start, goal);
        Assert.True(path!.Count > 0);
    }
}