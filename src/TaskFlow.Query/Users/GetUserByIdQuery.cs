public class GetUserByIdQuery : IRequest<UserDto>
{
    public int UserId { get; set; }
}

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserRepository _userRepository;
    public GetUserByIdQueryHandler(IUserRepository userRepository) => _userRepository = userRepository;

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var u = await _userRepository.ReadFirstOrDefaultAsync(x => x.UserId == request.UserId)
            ?? throw new NotFoundException($"User {request.UserId} not found.");

        return new UserDto(u.UserId, u.FirstName, u.LastName, u.Email, u.PhoneNumber, u.DepartmentId, u.IsActive, u.CreatedDate,
            u.UserRoles.Select(ur => ur.Role.RoleName).ToList(),
            u.UserRoles.Select(ur => ur.RoleId).ToList());
    }
}
