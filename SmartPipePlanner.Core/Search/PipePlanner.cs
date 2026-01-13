namespace SmartPipePlanner.Core.Search;

public class PipePlanner()
{
    public Pipe[]?[] PlanPaths(Grid grid, Problem[] problems)
    {
        AStarPlanner planner = new(grid);
        var len = problems.Length;
        var result = new Pipe[]?[len];

        for (int i = 0; i < len; i++)
        {
            var pb = problems[i];
            var path = planner.PlanPath(pb.Start, pb.End);
            if (path == null)
                break;

            path.Insert(0, pb.Start - Coordinate.FromDirection(pb.StartDir));

            var pipes = new List<Pipe>();
            int j = 1;
            while (j < path.Count)
            {
                var prev = path[j - 1];
                var current = path[j];
                var currentDir = GetDirection(prev, current);

                // pipe2 first
                if (j < path.Count - 1)
                {
                    var next = path[j + 1];
                    var nextDir = GetDirection(current, next);
                    if (j < path.Count - 2)
                    {
                        var next2 = path[j + 2];
                        var next2Dir = GetDirection(next, next2);
                        if (currentDir == nextDir && nextDir == next2Dir)
                        {
                            var pipe2 = new Pipe(current, currentDir, PipeGeometry.Pipe2, pb.Category);
                            pipes.Add(pipe2);
                            grid.Place(pipe2);
                            j += 2;
                            continue;
                        }                    
                    }

                    // lpipe
                    if (currentDir != nextDir)
                    {
                        var lpipe = new Pipe(current, currentDir, PipeGeometry.LPipe1, pb.Category, nextDir);
                        pipes.Add(lpipe);
                        grid.Place(lpipe);
                        j++;
                        continue;
                    }
                }

                // pipe1
                var pipe1 = new Pipe(current, currentDir, PipeGeometry.Pipe1, pb.Category);
                pipes.Add(pipe1);
                grid.Place(pipe1);
                j++;
            }

            result[i] = [.. pipes];
        }

        return [.. result];
    }

    static Direction GetDirection(Coordinate from, Coordinate to) => (to - from).ToDirection();
}

public record class Problem(Coordinate Start, Direction StartDir, Coordinate End, PipeCategory Category);
