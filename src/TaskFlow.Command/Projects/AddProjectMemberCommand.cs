public class AddProjectMemberCommand : IRequest<int>
{
    public int ProjectId { get; set; }
    public int UserId { get; set; }
}

public class AddProjectMemberCommandHandler : IRequestHandler<AddProjectMemberCommand, int>
{
    private readonly IRepository<ProjectMember> _projectMemberRepository;
    public AddProjectMemberCommandHandler(IRepository<ProjectMember> projectMemberRepository) => _projectMemberRepository = projectMemberRepository;

    public async Task<int> Handle(AddProjectMemberCommand request, CancellationToken ct)
    {
        var member = new ProjectMember
        {
            ProjectId = request.ProjectId,
            UserId = request.UserId,
            JoinedDate = DateOnly.FromDateTime(DateTime.UtcNow),
            IsActive = true,
        };
        await _projectMemberRepository.CreateAsync(member);
        return member.ProjectMemberId;
    }
}
