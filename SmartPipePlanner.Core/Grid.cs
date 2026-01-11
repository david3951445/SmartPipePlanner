namespace SmartPipePlanner.Core;

public class Grid
{
    public CellType[,,] Cells { get; }

    public Grid(int sizeX, int sizeY, int sizeZ)
    {
        Cells = new CellType[sizeX, sizeY, sizeZ];
        for (int x = 0; x < sizeX; x++)
            for (int y = 0; y < sizeY; y++)
                for (int z = 0; z < sizeZ; z++)
                    Cells[x, y, z] = CellType.Empty;
    }

    public bool CanPlace(Pipe pipe)
    {
        var loc = pipe.Location;

        // 先檢查當前 cell
        if (!IsInside(loc) || Cells[loc.X, loc.Y, loc.Z] != CellType.Empty)
            return false;

        switch (pipe.Geometry)
        {
            case PipeGeometry.Pipe1:
                var prev = loc + Coordinate.FromDirection(pipe.Direction);
                if (!IsInside(prev) || IsCellType(prev, CellType.Obstacle))
                    return false;
                return true;

            case PipeGeometry.Pipe2:
                var prev1 = loc + Coordinate.FromDirection(pipe.Direction);
                var prev2 = loc + 2 * Coordinate.FromDirection(pipe.Direction);
                if ((!IsInside(prev1) || IsCellType(prev1, CellType.Obstacle)) ||
                    (!IsInside(prev2) || IsCellType(prev2, CellType.Obstacle)))
                    return false;
                return true;

            case PipeGeometry.LPipe1:
                //foreach (var offset in LPipe1Directions(pipe.Direction))
                {
                    var neighbor = loc + Coordinate.FromDirection(pipe.Direction);
                    if (!IsInside(neighbor) || IsCellType(neighbor, CellType.Obstacle))
                        return false;
                }
                return true;
        }

        return false;
    }

    public bool IsInside(Coordinate c)
        => c.X >= 0 && c.Y >= 0 && c.Z >= 0 &&
           c.X < Cells.GetLength(0) &&
           c.Y < Cells.GetLength(1) &&
           c.Z < Cells.GetLength(2);

    public bool IsCellType(Coordinate c, CellType type) => Cells[c.X, c.Y, c.Z] == type;

    public void Place(Pipe pipe)
    {
        var cellType = pipe.Category == PipeCategory.HotWaterPipe ? CellType.HotPipe : CellType.ColdPipe;
        foreach (var c in GetOccupiedCells(pipe))
            if (IsInside(c))
                Cells[c.X, c.Y, c.Z] = cellType;
    }

    public void Remove(Pipe pipe)
    {
        foreach (var c in GetOccupiedCells(pipe))
            if (IsInside(c))
                Cells[c.X, c.Y, c.Z] = CellType.Empty;
    }

    static IEnumerable<Coordinate> GetOccupiedCells(Pipe pipe)
    {
        var loc = pipe.Location;

        switch (pipe.Geometry)
        {
            case PipeGeometry.Pipe1:
                yield return loc;
                break;

            case PipeGeometry.Pipe2:
                yield return loc;
                yield return loc + Coordinate.FromDirection(pipe.Direction); // 前一格
                break;

            case PipeGeometry.LPipe1:
                yield return loc; // 只占當前格
                break;
        }
    }
}
