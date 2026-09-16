public class RemoveProjectMemberCommand : IRequest
{
    public int ProjectMemberId { get; set; }
}

public class RemoveProjectMemberCommandHandler : IRequestHandler<RemoveProjectMemberCommand>
{
    private readonly IRepository<ProjectMember> _projectMemberRepository;
    public RemoveProjectMemberCommandHandler(IRepository<ProjectMember> projectMemberRepository) => _projectMemberRepository = projectMemberRepository;

    public async Task Handle(RemoveProjectMemberCommand request, CancellationToken ct)
    {
        var member = await _projectMemberRepository.ReadFirstOrDefaultAsync(m => m.ProjectMemberId == request.ProjectMemberId)
            ?? throw new NotFoundException($"Project member {request.ProjectMemberId} not found.");

        await _projectMemberRepository.DeleteAsync(member);
    }
}
