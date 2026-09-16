using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    public DashboardController(IDashboardService dashboardService) => _dashboardService = dashboardService;

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        // UserId/Role come from the caller's own JWT claims, never from a query-string parameter —
        // nobody can ask for someone else's dashboard by passing a different id.
        var query = new GetDashboardSummaryQuery
        {
            UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            Role = User.FindFirstValue(ClaimTypes.Role)!,
        };
        return Ok(await _dashboardService.GetSummary(query));
    }
}
