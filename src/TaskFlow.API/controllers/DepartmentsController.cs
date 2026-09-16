using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/departments")]
[Authorize(Roles = "Admin")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;
    public DepartmentsController(IDepartmentService departmentService) => _departmentService = departmentService;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetDepartmentsQuery query)
        => Ok(await _departmentService.GetDepartments(query));

    [HttpPost]
    public async Task<IActionResult> Create(CreateDepartmentCommand command)
        => Ok(await _departmentService.CreateDepartment(command));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateDepartmentCommand command)
    {
        command.DepartmentId = id;
        await _departmentService.UpdateDepartment(command);
        return NoContent();
    }
}
