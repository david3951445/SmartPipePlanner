using Microsoft.AspNetCore.Mvc;
using SmartPipePlanner.API.Contracts;
using SmartPipePlanner.Core;
using SmartPipePlanner.Core.Search;

namespace SmartPipePlanner.API.Controllers;

[ApiController]
[Route("api/pipe-planner")]
public class PipePlannerController : ControllerBase
{
    readonly PipePlanner _planner;

    public PipePlannerController(PipePlanner planner)
    {
        _planner = planner;
    }

    [HttpPost("plan")]
    public ActionResult<PlanPathsResponse> Plan([FromBody] PlanPathsRequest request)
    {
        if (request.Problems.Length == 0)
            return BadRequest("No problems provided.");

        Problem[] domainProblems = [.. request.Problems];

        var grid = new Grid(request.Size.X, request.Size.Y, request.Size.Z);
        Pipe[]?[] results = _planner.PlanPaths(grid, domainProblems);

        if (results == null)
        {
            return Ok(new PlanPathsResponse
            {
                Results = [.. domainProblems
                    .Select(_ => new PathResultDto
                    {
                        Success = false,
                        Error = "Planning failed."
                    })]
            });
        }

        return Ok(new PlanPathsResponse
        {
            Results = [.. results.Select(pipes =>
                pipes == null
                    ? new PathResultDto
                    {
                        Success = false,
                        Error = "No feasible path."
                    }
                    : new PathResultDto
                    {
                        Success = true,
                        Pipes = pipes.ToArray()
                    }
            )]
        });
    }
}
