public class UpdateProjectCommand : IRequest
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int ProjectManagerId { get; set; }
    public string Status { get; set; } = "Active";
    public string Priority { get; set; } = "Medium";
}

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand>
{
    private readonly IProjectRepository _projectRepository;
    public UpdateProjectCommandHandler(IProjectRepository projectRepository) => _projectRepository = projectRepository;

    public async Task Handle(UpdateProjectCommand request, CancellationToken ct)
    {
        var project = await _projectRepository.ReadFirstOrDefaultAsync(p => p.ProjectId == request.ProjectId)
            ?? throw new NotFoundException($"Project {request.ProjectId} not found.");

        project.ProjectName = request.ProjectName;
        project.Description = request.Description;
        project.StartDate = request.StartDate;
        project.EndDate = request.EndDate;
        project.ProjectManagerId = request.ProjectManagerId;
        project.Status = request.Status;
        project.Priority = request.Priority;
        project.UpdatedDate = DateTime.UtcNow;

        await _projectRepository.UpdateAsync(project);
    }
}
