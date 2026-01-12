namespace SmartPipePlanner.Core;

public record Pipe(
    Coordinate Location,
    Direction Direction,
    PipeGeometry Geometry,
    PipeCategory Category,
    Direction? LPipeDirection = null
);

public enum Direction { PosX, NegX, PosY, NegY, PosZ, NegZ }
public enum PipeGeometry { Pipe1, Pipe2, LPipe1 }
public enum PipeCategory { HotWaterPipe, ColdWaterPipe }
public enum CellType { Empty, Obstacle, HotPipe, ColdPipe }

public record State(
    Coordinate Location,
    Direction Direction
);
