public class GetProjectByIdQuery : IRequest<ProjectDto>
{
    public int ProjectId { get; set; }
}

public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto>
{
    private readonly IProjectRepository _projectRepository;
    public GetProjectByIdQueryHandler(IProjectRepository projectRepository) => _projectRepository = projectRepository;

    public async Task<ProjectDto> Handle(GetProjectByIdQuery request, CancellationToken ct)
    {
        var p = await _projectRepository.GetByIdWithMembersAsync(request.ProjectId)
            ?? throw new NotFoundException($"Project {request.ProjectId} not found.");

        return new ProjectDto(
            p.ProjectId, p.ProjectCode, p.ProjectName, p.Description, p.StartDate, p.EndDate,
            p.ProjectManagerId, p.ProjectManager?.FirstName + " " + p.ProjectManager?.LastName,
            p.Status, p.Priority, p.CreatedBy, p.CreatedDate, p.UpdatedDate);
    }
}
