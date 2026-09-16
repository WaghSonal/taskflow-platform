using Microsoft.EntityFrameworkCore;

public class GetProjectsQuery : IRequest<List<ProjectDto>>
{
    public string? Search { get; set; }
    public string? Status { get; set; }
    public int? ProjectManagerId { get; set; }
}

public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, List<ProjectDto>>
{
    private readonly IProjectRepository _projectRepository;
    public GetProjectsQueryHandler(IProjectRepository projectRepository) => _projectRepository = projectRepository;

    public async Task<List<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken ct)
    {
        var query = _projectRepository.ReadWhere(p =>
            (request.Status == null || p.Status == request.Status) &&
            (request.ProjectManagerId == null || p.ProjectManagerId == request.ProjectManagerId) &&
            (request.Search == null || p.ProjectName.Contains(request.Search) || p.ProjectCode.Contains(request.Search)));

        return await query
            .Select(p => new ProjectDto(
                p.ProjectId, p.ProjectCode, p.ProjectName, p.Description, p.StartDate, p.EndDate,
                p.ProjectManagerId, p.ProjectManager.FirstName + " " + p.ProjectManager.LastName,
                p.Status, p.Priority, p.CreatedBy, p.CreatedDate, p.UpdatedDate))
            .ToListAsync(ct);
    }
}
