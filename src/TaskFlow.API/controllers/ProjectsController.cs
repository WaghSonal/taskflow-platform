using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/projects")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    public ProjectsController(IProjectService projectService) => _projectService = projectService;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetProjectsQuery query)
        => Ok(await _projectService.GetProjects(query));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
        => Ok(await _projectService.GetProjectById(id));

    [HttpPost]
    public async Task<IActionResult> Create(CreateProjectCommand command)
        => Ok(await _projectService.CreateProject(command));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateProjectCommand command)
    {
        command.ProjectId = id;
        await _projectService.UpdateProject(command);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _projectService.DeleteProject(id);
        return NoContent();
    }

    [HttpGet("{id:int}/members")]
    public async Task<IActionResult> GetMembers(int id)
        => Ok(await _projectService.GetProjectMembers(id));

    [HttpPost("{id:int}/members")]
    public async Task<IActionResult> AddMember(int id, AddProjectMemberCommand command)
    {
        command.ProjectId = id;
        return Ok(await _projectService.AddProjectMember(command));
    }

    [HttpDelete("{id:int}/members/{memberId:int}")]
    public async Task<IActionResult> RemoveMember(int id, int memberId)
    {
        await _projectService.RemoveProjectMember(memberId);
        return NoContent();
    }
}
