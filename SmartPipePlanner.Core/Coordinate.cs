namespace SmartPipePlanner.Core;

public readonly record struct Coordinate(int X, int Y, int Z)
{
    public static Coordinate operator +(Coordinate a, Coordinate b)
        => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Coordinate operator -(Coordinate a, Coordinate b)
        => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static Coordinate operator *(int scale, Coordinate a)
        => new(a.X * scale, a.Y * scale, a.Z * scale);

    public int ManhattanDistance(Coordinate other)
        => Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);
    public double Distance(Coordinate other) => Math.Sqrt(
        Math.Pow(X - other.X, 2) +
        Math.Pow(Y - other.Y, 2) +
        Math.Pow(Z - other.Z, 2));
    public static Coordinate FromDirection(Direction d) => d switch
    {
        Direction.PosX => new(1, 0, 0),
        Direction.NegX => new(-1, 0, 0),
        Direction.PosY => new(0, 1, 0),
        Direction.NegY => new(0, -1, 0),
        Direction.PosZ => new(0, 0, 1),
        Direction.NegZ => new(0, 0, -1),
        _ => throw new InvalidOperationException()
    };
    public Direction ToDirection()
    {
        if (this == new Coordinate(1, 0, 0)) return Direction.PosX;
        if (this == new Coordinate(-1, 0, 0)) return Direction.NegX;
        if (this == new Coordinate(0, 1, 0)) return Direction.PosY;
        if (this == new Coordinate(0, -1, 0)) return Direction.NegY;
        if (this == new Coordinate(0, 0, 1)) return Direction.PosZ;
        if (this == new Coordinate(0, 0, -1)) return Direction.NegZ;

        throw new InvalidOperationException();
    }
}
