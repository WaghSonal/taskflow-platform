using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/time-entries")]
[Authorize]
public class TimeEntriesController : ControllerBase
{
    private readonly ITimeEntryService _timeEntryService;
    public TimeEntriesController(ITimeEntryService timeEntryService) => _timeEntryService = timeEntryService;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetTimeEntriesQuery query)
        => Ok(await _timeEntryService.GetTimeEntries(query));

    [HttpPost]
    public async Task<IActionResult> Create(LogTimeCommand command)
        => Ok(await _timeEntryService.LogTime(command));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _timeEntryService.DeleteTimeEntry(id);
        return NoContent();
    }
}
