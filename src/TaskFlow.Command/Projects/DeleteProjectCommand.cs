public class DeleteProjectCommand : IRequest
{
    public int ProjectId { get; set; }
}

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand>
{
    private readonly IProjectRepository _projectRepository;
    public DeleteProjectCommandHandler(IProjectRepository projectRepository) => _projectRepository = projectRepository;

    public async Task Handle(DeleteProjectCommand request, CancellationToken ct)
    {
        var project = await _projectRepository.ReadFirstOrDefaultAsync(p => p.ProjectId == request.ProjectId)
            ?? throw new NotFoundException($"Project {request.ProjectId} not found.");

        await _projectRepository.DeleteAsync(project);
    }
}
