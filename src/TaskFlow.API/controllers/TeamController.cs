using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/team")]
[Authorize(Roles = "Project Manager,Admin")]
public class TeamController : ControllerBase
{
    private readonly ITeamService _teamService;
    public TeamController(ITeamService teamService) => _teamService = teamService;

    [HttpGet("workload")]
    public async Task<IActionResult> GetWorkload()
    {
        var managerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _teamService.GetWorkload(new GetTeamWorkloadQuery { ManagerId = managerId }));
    }
}
