namespace SmartPipePlanner.Core.Search;

public class PipePlanner(Grid grid)
{
    readonly Grid _grid = grid;
    readonly AStarPlanner _planner = new(grid);

    public Pipe[][]? PlanPaths(Problem[] problems)
    {
        var result = new List<Pipe[]>();

        foreach (var p in problems)
        {
            var path = _planner.PlanPath(p.Start, p.End);
            if (path == null)
                return null;

            var pipes = new List<Pipe>();

            int i = 0;
            while (i < path.Count - 1)
            {
                var cur = path[i];
                var next = path[i + 1];
                var dir = GetDirection(cur, next);

                // ¹Á¸Õ pipe2
                if (i + 2 < path.Count)
                {
                    var next2 = path[i + 2];
                    var dir2 = GetDirection(next, next2);

                    if (dir == dir2)
                    {
                        var pipe2 = new Pipe(cur,dir,PipeGeometry.Pipe2,p.Category);

                        pipes.Add(pipe2);
                        _grid.Place(pipe2);

                        i += 2;
                        continue;
                    }
                }

                // ÂàÅs ¡÷ lpipe1
                if (i > 0)
                {
                    var prev = path[i - 1];
                    var prevDir = GetDirection(prev, cur);

                    if (prevDir != dir)
                    {
                        var lpipe = new Pipe(cur,prevDir, PipeGeometry.LPipe1,p.Category);

                        pipes.Add(lpipe);
                        _grid.Place(lpipe);
                    }
                }

                // fallback ¡÷ pipe1
                var pipe1 = new Pipe(cur,dir,PipeGeometry.Pipe1,p.Category);

                pipes.Add(pipe1);
                _grid.Place(pipe1);

                i += 1;
            }

            result.Add([.. pipes]);
        }

        return [.. result];
    }

    static Direction GetDirection(Coordinate from, Coordinate to) => (to - from).ToDirection();
}

public record class Problem(Coordinate Start, Direction StartDir, Coordinate End, PipeCategory Category);
