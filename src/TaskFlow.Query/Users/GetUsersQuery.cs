using Microsoft.EntityFrameworkCore;

public class GetUsersQuery : IRequest<List<UserDto>>
{
    public string? Search { get; set; }
    public int? RoleId { get; set; }
    public int? DepartmentId { get; set; }
    public bool? IsActive { get; set; }
}

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
{
    private readonly IUserRepository _userRepository;
    public GetUsersQueryHandler(IUserRepository userRepository) => _userRepository = userRepository;

    public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken ct)
    {
        var query = _userRepository.ReadWhere(u =>
            (request.DepartmentId == null || u.DepartmentId == request.DepartmentId) &&
            (request.IsActive == null || u.IsActive == request.IsActive) &&
            (request.RoleId == null || u.UserRoles.Any(ur => ur.RoleId == request.RoleId)) &&
            (request.Search == null || u.FirstName.Contains(request.Search) || u.LastName.Contains(request.Search) || u.Email.Contains(request.Search)));

        var users = await query
            .Select(u => new
            {
                u.UserId, u.FirstName, u.LastName, u.Email, u.PhoneNumber, u.DepartmentId, u.IsActive, u.CreatedDate,
                Roles = u.UserRoles.Select(ur => ur.Role.RoleName).ToList(),
                RoleIds = u.UserRoles.Select(ur => ur.RoleId).ToList(),
            })
            .ToListAsync(ct);

        return users.Select(u => new UserDto(u.UserId, u.FirstName, u.LastName, u.Email, u.PhoneNumber, u.DepartmentId, u.IsActive, u.CreatedDate, u.Roles, u.RoleIds)).ToList();
    }
}
