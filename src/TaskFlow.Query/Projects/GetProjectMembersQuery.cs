using Microsoft.EntityFrameworkCore;

public class GetProjectMembersQuery : IRequest<List<ProjectMemberDto>>
{
    public int ProjectId { get; set; }
}

public class GetProjectMembersQueryHandler : IRequestHandler<GetProjectMembersQuery, List<ProjectMemberDto>>
{
    private readonly IRepository<ProjectMember> _projectMemberRepository;
    public GetProjectMembersQueryHandler(IRepository<ProjectMember> projectMemberRepository) => _projectMemberRepository = projectMemberRepository;

    public async Task<List<ProjectMemberDto>> Handle(GetProjectMembersQuery request, CancellationToken ct)
    {
        return await _projectMemberRepository.ReadWhere(m => m.ProjectId == request.ProjectId)
            .Select(m => new ProjectMemberDto(m.ProjectMemberId, m.ProjectId, m.UserId, m.User.FirstName + " " + m.User.LastName, m.JoinedDate, m.IsActive))
            .ToListAsync(ct);
    }
}
