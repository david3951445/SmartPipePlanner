namespace SmartPipePlanner.Core.Search;

public class AStarPlanner(Grid grid)
{
    readonly Grid _grid = grid;

    public List<Coordinate>? PlanPath(Coordinate start, Coordinate goal)
    {
        var openSet = new PriorityQueue<Node, double>();
        var closedSet = new HashSet<Coordinate>();

        var startNode = new Node(
            new State(start, default),
            null,
            0,
            start.Distance(goal),
            default);

        openSet.Enqueue(startNode, startNode.FCost);

        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();

            if (current.State.Location == goal)
            {
                var path = new List<Coordinate>();
                Node? node = current;
                while (node != null)
                {
                    path.Add(node.State.Location);
                    node = node.Parent;
                }
                path.Reverse();
                return path;
            }

            if (!closedSet.Add(current.State.Location))
                continue;

            foreach (var nextState in GenerateSuccessors(current.State))
            {
                if (closedSet.Contains(nextState.Location))
                    continue;

                var g = current.GCost + 1; // 每走一步 cost = 1
                var h = nextState.Location.ManhattanDistance(goal);

                openSet.Enqueue(
                    new Node(nextState, current, g, h, default),
                    g + h);
            }
        }

        return null;
    }

    IEnumerable<State> GenerateSuccessors(State state)
    {
        foreach (var dir in Enum.GetValues<Direction>())
        {
            var next = state.Location + Coordinate.FromDirection(dir);

            if (!_grid.IsInside(next))
                continue;

            if (!_grid.IsCellType(next, CellType.Empty))
                continue;

            yield return new State(next, default);
        }
    }
}
