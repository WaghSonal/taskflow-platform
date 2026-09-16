// Backs the role picker on the Users create/edit form (Phase 12) — Roles has no dedicated feature
// phase of its own since the three rows are fixed and already seeded.
using Microsoft.EntityFrameworkCore;

public class GetRolesQuery : IRequest<List<RoleDto>> { }

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, List<RoleDto>>
{
    private readonly IRepository<Role> _roleRepository;
    public GetRolesQueryHandler(IRepository<Role> roleRepository) => _roleRepository = roleRepository;

    public Task<List<RoleDto>> Handle(GetRolesQuery request, CancellationToken ct) =>
        _roleRepository.ReadWhere(r => true).OrderBy(r => r.RoleId)
            .Select(r => new RoleDto(r.RoleId, r.RoleName, r.Description))
            .ToListAsync(ct);
}
