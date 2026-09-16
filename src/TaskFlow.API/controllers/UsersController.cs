using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService) => _userService = userService;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetUsersQuery query)
        => Ok(await _userService.GetUsers(query));

    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles() => Ok(await _userService.GetRoles());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
        => Ok(await _userService.GetUserById(id));

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserCommand command)
        => Ok(await _userService.CreateUser(command));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateUserCommand command)
    {
        command.UserId = id;
        await _userService.UpdateUser(command);
        return NoContent();
    }

    [HttpPatch("{id:int}/active")]
    public async Task<IActionResult> SetActive(int id, [FromBody] bool isActive)
    {
        await _userService.SetUserActive(id, isActive);
        return NoContent();
    }

    [HttpPut("{id:int}/roles")]
    public async Task<IActionResult> UpdateRoles(int id, [FromBody] List<int> roleIds)
    {
        await _userService.UpdateUserRoles(id, roleIds);
        return NoContent();
    }
}
