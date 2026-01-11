using Microsoft.Extensions.Logging;

namespace SmartPipePlanner.Core.Search;

/// <summary>
/// Plan the path with <see cref="Coordinate"/>.
/// </summary>
/// <remarks>
/// The state space is too large.
/// </remarks>
public class AStarPlanner2(Grid grid, ILogger logger)
{
    readonly Grid _grid = grid;
    readonly ILogger _logger = logger;

    public List<Pipe>? PlanPath(Coordinate start, Direction startDir, Coordinate goal, PipeCategory category)
    {
        var startState = new State(start, startDir);
        var startNode = new Node(startState, null, 0, ComputeH(startState, goal), null);
        var openSet = new PriorityQueue<Node, double>();
        var closedSet = new HashSet<Coordinate>();

        openSet.Enqueue(startNode, startNode.FCost);

        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue(); // always node with lowest FCost

            // Goal reached → reconstruct path
            if (current.State.Location.Equals(goal))
            {
                var path = new List<Pipe>();
                var node = current;
                while (node.Action != null)
                {
                    path.Add(node.Action);
                    node = node.Parent!;
                }
                path.Reverse();
                return path;
            }

            closedSet.Add(current.State.Location);

            // Generate successors for current node
            foreach (var (pipe, nextState) in GenerateSuccessors(current, category))
            {
                _logger.LogDebug("{Pipe}\n  {State}", pipe, nextState);
                if (closedSet.Contains(nextState.Location))
                    continue;

                double g = current.GCost + ComputeG(pipe);
                var nodeNew = new Node(nextState, current, g, ComputeH(nextState, goal), pipe);

                openSet.Enqueue(nodeNew, nodeNew.FCost); // add with FCost as priority
            }
        }

        // No path found
        return null;
    }

    static double ComputeG(Pipe pipe)
    {
        double length = pipe.Geometry switch
        {
            PipeGeometry.Pipe1 => 1,
            PipeGeometry.Pipe2 => 2,
            PipeGeometry.LPipe1 => 1,
            _ => throw new InvalidOperationException()
        };

        double price = pipe.Geometry switch
        {
            PipeGeometry.Pipe1 => 4,
            PipeGeometry.Pipe2 => 6,
            PipeGeometry.LPipe1 => 5,
            _ => throw new InvalidOperationException()
        };

        double alpha = 0.5; // length weight
        double beta = 1 - alpha;  // price weight

        return alpha * length + beta * price;
    }
    static double ComputeH(State state, Coordinate goal) => state.Location.Distance(goal);
    IEnumerable<(Pipe pipe, State nextState)> GenerateSuccessors(Node node, PipeCategory category)
    {
        var currentState = node.State;

        // 先定義可以嘗試的 pipe geometry
        foreach (var geometry in new[] { PipeGeometry.Pipe2, PipeGeometry.Pipe1, PipeGeometry.LPipe1 })
        {
            // 對每個可能方向都試一次
            var candidates = GetCandidateDirections(geometry, currentState.Direction);

            foreach (Direction dir in candidates)
            {
                var pipe = new Pipe(currentState.Location, dir, geometry, category);

                if (_grid.CanPlace(pipe))
                {
                    // 下一個 state 以 pipe 的方向為新方向，位置加方向向量
                    var length = GetLength(geometry);
                    var nextLoc = currentState.Location + length * Coordinate.FromDirection(dir);
                    var nextState = new State(nextLoc, dir);

                    yield return (pipe, nextState);
                }
            }
        }
    }

    static Direction[] GetCandidateDirections(PipeGeometry geometry, Direction dir) => geometry switch
    {
        PipeGeometry.Pipe1 or PipeGeometry.Pipe2 => [dir],
        PipeGeometry.LPipe1 => [.. LPipe1Directions(dir)],
        _ => throw new InvalidOperationException(),
    };

    static IEnumerable<Direction> LPipe1Directions(Direction orientation) => orientation switch
    {
        Direction.PosX or Direction.NegX =>
        [
            Direction.PosY,
            Direction.NegY,
            Direction.PosZ,
            Direction.NegZ
        ],
        Direction.PosY or Direction.NegY =>
        [
            Direction.PosX,
            Direction.NegX,
            Direction.PosZ,
            Direction.NegZ
        ],
        Direction.PosZ or Direction.NegZ =>
        [
            Direction.PosX,
            Direction.NegX,
            Direction.PosY,
            Direction.NegY
        ],
        _ => throw new InvalidOperationException()
    };

    static int GetLength(PipeGeometry geometry) => geometry switch
    {
        PipeGeometry.Pipe1 => 1,
        PipeGeometry.Pipe2 => 2,
        PipeGeometry.LPipe1 => 1,
        _ => throw new InvalidOperationException()
    };
}
