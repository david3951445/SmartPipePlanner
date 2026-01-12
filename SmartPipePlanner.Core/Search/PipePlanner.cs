namespace SmartPipePlanner.Core.Search;

public class PipePlanner(Grid grid)
{
    readonly Grid _grid = grid;
    readonly AStarPlanner _planner = new(grid);

    public Pipe[]?[] PlanPaths(Problem[] problems)
    {
        var len = problems.Length;
        var result = new Pipe[]?[len];

        for (int i = 0; i < len; i++)
        {
            var p = problems[i];
            var path = _planner.PlanPath(p.Start, p.End);
            if (path == null)
                break;

            var pipes = new List<Pipe>();

            int j = 0;
            while (j < path.Count - 1)
            {
                var cur = path[j];
                var next = path[j + 1];
                var dir = GetDirection(cur, next);

                if (j + 2 < path.Count)
                {
                    var next2 = path[j + 2];
                    var dir2 = GetDirection(next, next2);

                    if (dir == dir2)
                    {
                        var pipe2 = new Pipe(cur, dir, PipeGeometry.Pipe2, p.Category);

                        pipes.Add(pipe2);
                        _grid.Place(pipe2);

                        j += 2;
                        continue;
                    }
                }

                if (j > 0)
                {
                    var prev = path[j - 1];
                    var prevDir = GetDirection(prev, cur);

                    if (prevDir != dir)
                    {
                        var lpipe = new Pipe(cur, prevDir, PipeGeometry.LPipe1, p.Category);

                        pipes.Add(lpipe);
                        _grid.Place(lpipe);
                    }
                }

                // fallback
                var pipe1 = new Pipe(cur, dir, PipeGeometry.Pipe1, p.Category);

                pipes.Add(pipe1);
                _grid.Place(pipe1);

                j += 1;
            }

            result[i] = [.. pipes];
        }

        return [.. result];
    }

    static Direction GetDirection(Coordinate from, Coordinate to) => (to - from).ToDirection();
}

public record class Problem(Coordinate Start, Direction StartDir, Coordinate End, PipeCategory Category);
