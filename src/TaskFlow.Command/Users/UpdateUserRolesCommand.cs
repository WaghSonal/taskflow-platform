using Microsoft.EntityFrameworkCore;

// Replaces a user's full role set — added for Phase 12 (frontend wiring), where the Users page's
// generic "edit user" flow can change role assignments and no endpoint existed for that yet.
public class UpdateUserRolesCommand : IRequest
{
    public int UserId { get; set; }
    public List<int> RoleIds { get; set; } = new();
}

public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand>
{
    private readonly IRepository<UserRole> _userRoleRepository;
    public UpdateUserRolesCommandHandler(IRepository<UserRole> userRoleRepository) => _userRoleRepository = userRoleRepository;

    public async Task Handle(UpdateUserRolesCommand request, CancellationToken ct)
    {
        var existing = await _userRoleRepository.ReadWhere(ur => ur.UserId == request.UserId).ToListAsync(ct);
        foreach (var userRole in existing)
            await _userRoleRepository.DeleteAsync(userRole);

        foreach (var roleId in request.RoleIds)
            await _userRoleRepository.CreateAsync(new UserRole { UserId = request.UserId, RoleId = roleId });
    }
}
