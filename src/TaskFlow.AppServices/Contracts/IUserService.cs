public interface IUserService
{
    Task<List<UserDto>> GetUsers(GetUsersQuery query);
    Task<UserDto> GetUserById(int userId);
    Task<int> CreateUser(CreateUserCommand command);
    Task UpdateUser(UpdateUserCommand command);
    Task SetUserActive(int userId, bool isActive);
    Task UpdateUserRoles(int userId, List<int> roleIds);
    Task<List<RoleDto>> GetRoles();
}
