using SmartPipePlanner.Core;
using SmartPipePlanner.Core.Search;

namespace SmartPipePlanner.API.Contracts;

public record PlanPathsRequest
{
    public Coordinate Size { get; init; } = default!;
    public Problem[] Problems { get; init; } = [];
}

public record PlanPathsResponse
{
    public PathResultDto[] Results { get; init; } = [];
}

public record PathResultDto
{
    public bool Success { get; init; }
    public Pipe[]? Pipes { get; init; }
    public string? Error { get; init; }
}
