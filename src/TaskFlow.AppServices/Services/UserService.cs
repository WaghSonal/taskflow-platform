public class UserService : IUserService
{
    private readonly Dispatcher _dispatcher;
    public UserService(Dispatcher dispatcher) => _dispatcher = dispatcher;

    public Task<List<UserDto>> GetUsers(GetUsersQuery query) => _dispatcher.Send(query);
    public Task<UserDto> GetUserById(int userId) => _dispatcher.Send(new GetUserByIdQuery { UserId = userId });
    public Task<int> CreateUser(CreateUserCommand command) => _dispatcher.Send(command);
    public Task UpdateUser(UpdateUserCommand command) => _dispatcher.Send(command);
    public Task SetUserActive(int userId, bool isActive) => _dispatcher.Send(new SetUserActiveCommand { UserId = userId, IsActive = isActive });
    public Task UpdateUserRoles(int userId, List<int> roleIds) => _dispatcher.Send(new UpdateUserRolesCommand { UserId = userId, RoleIds = roleIds });
    public Task<List<RoleDto>> GetRoles() => _dispatcher.Send(new GetRolesQuery());
}
