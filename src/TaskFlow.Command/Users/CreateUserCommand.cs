using Microsoft.AspNetCore.Identity;

public class CreateUserCommand : IRequest<int>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public int? DepartmentId { get; set; }
    public List<int> RoleIds { get; set; } = new();
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, int>
{
    private readonly IUserRepository _userRepository;
    private readonly IRepository<UserRole> _userRoleRepository;
    private readonly IPasswordHasher<User> _hasher;

    public CreateUserCommandHandler(IUserRepository userRepository, IRepository<UserRole> userRoleRepository, IPasswordHasher<User> hasher)
    { _userRepository = userRepository; _userRoleRepository = userRoleRepository; _hasher = hasher; }

    public async Task<int> Handle(CreateUserCommand request, CancellationToken ct)
    {
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            DepartmentId = request.DepartmentId,
            IsActive = true,
        };
        user.PasswordHash = _hasher.HashPassword(user, request.Password);
        await _userRepository.CreateAsync(user);

        foreach (var roleId in request.RoleIds)
        {
            await _userRoleRepository.CreateAsync(new UserRole { UserId = user.UserId, RoleId = roleId });
        }

        return user.UserId;
    }
}
