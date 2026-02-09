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
        List<Coordinate>? path = planner.PlanPath(start, goal);
        Assert.InRange(path!.Count, 0, start.ManhattanDistance(goal) + 1);
    }

    [Fact]
    public void PlanPath_WithObstacles_CubeInMiddle_ReturnsPath()
    {
        int size = 5;
        var grid = new Grid(size, size, size);
        grid.SetBox(new Coordinate(1, 1, 1), new Coordinate(size - 2, size - 2, size - 2), CellType.Obstacle);
        var planner = new AStarPlanner(grid);

        Coordinate start = new(0, 0, 0);
        Coordinate goal = new(4, 4, 4);

        // Act & Assert
        List<Coordinate>? path = planner.PlanPath(start, goal);
        Assert.Equal(13, path!.Count);
    }

    [Fact]
    public void PlanPath_WithObstacles_PlaneWithGaps_ReturnsPath()
    {
        int size = 5;
        var grid = new Grid(size, size, size);
        grid.SetBox(new Coordinate(0, 0, 1), new Coordinate(size - 1, size - 2, 1), CellType.Obstacle);
        grid.SetBox(new Coordinate(0, 1, 3), new Coordinate(size - 1, size - 1, 3), CellType.Obstacle);
        var planner = new AStarPlanner(grid);

        Coordinate start = new(0, 0, 0);
        Coordinate goal = new(4, 4, 4);

        // Act & Assert
        List<Coordinate>? path = planner.PlanPath(start, goal);
        Assert.Equal(21, path!.Count);
    }

    [Fact]
    public void PathPlan_WithObstacles_CavityWithHole_ReturnsPath()
    {
        int size = 5;
        var grid = new Grid(size, size, size);
        grid.SetBox(new Coordinate(1, 1, 1), new Coordinate(3, 3, 3), CellType.Obstacle);
        grid.Cells[2, 2, 3] = CellType.Empty;
        grid.Cells[2, 2, 2] = CellType.Empty;
        var planner = new AStarPlanner(grid);

        Coordinate start = new(0, 0, 0);
        Coordinate goal = new(2, 2, 2);

        // Act & Assert
        List<Coordinate>? path = planner.PlanPath(start, goal);
        Assert.Equal(11, path!.Count);
    }

    [Fact]
    public void PlanPath_Blocked_ReturnsNull()
    {
        int size = 5;
        var grid = new Grid(size, size, size);
        grid.SetBox(new Coordinate(1, 0, 0), new Coordinate(1, size - 1, size - 1), CellType.Obstacle);
        var planner = new AStarPlanner(grid);

        Coordinate start = new(0, 0, 0);
        Coordinate goal = new(size - 1, size - 1, size - 1);

        // Act & Assert
        List<Coordinate>? path = planner.PlanPath(start, goal);
        Assert.Null(path);
    }

    [Fact]
    public void PlanPath_SingleCell_ReturnsThatCell()
    {
        int size = 1;
        var grid = new Grid(size, size, size);
        var planner = new AStarPlanner(grid);

        Coordinate single = new(0, 0, 0);

        // Act & Assert
        List<Coordinate>? path = planner.PlanPath(single, single);
        Assert.Equal(path![0], single);
    }
}