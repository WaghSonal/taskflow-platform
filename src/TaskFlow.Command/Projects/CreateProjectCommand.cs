public class CreateProjectCommand : IRequest<int>
{
    public string ProjectCode { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int ProjectManagerId { get; set; }
    public string Status { get; set; } = "Active";
    public string Priority { get; set; } = "Medium";
    public int CreatedBy { get; set; }
}

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, int>
{
    private readonly IProjectRepository _projectRepository;
    public CreateProjectCommandHandler(IProjectRepository projectRepository) => _projectRepository = projectRepository;

    public async Task<int> Handle(CreateProjectCommand request, CancellationToken ct)
    {
        var project = new Project
        {
            ProjectCode = request.ProjectCode,
            ProjectName = request.ProjectName,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            ProjectManagerId = request.ProjectManagerId,
            Status = request.Status,
            Priority = request.Priority,
            CreatedBy = request.CreatedBy,
        };
        await _projectRepository.CreateAsync(project);
        return project.ProjectId;
    }
}
