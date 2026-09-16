using Microsoft.EntityFrameworkCore;

public class GetTeamWorkloadQuery : IRequest<List<TeamMemberWorkloadDto>>
{
    public int ManagerId { get; set; }
}

public class GetTeamWorkloadQueryHandler : IRequestHandler<GetTeamWorkloadQuery, List<TeamMemberWorkloadDto>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;

    public GetTeamWorkloadQueryHandler(ITaskRepository taskRepository, IProjectRepository projectRepository, IUserRepository userRepository)
    { _taskRepository = taskRepository; _projectRepository = projectRepository; _userRepository = userRepository; }

    public async Task<List<TeamMemberWorkloadDto>> Handle(GetTeamWorkloadQuery request, CancellationToken ct)
    {
        var ownedProjectIds = await _projectRepository.ReadWhere(p => p.ProjectManagerId == request.ManagerId)
            .Select(p => p.ProjectId).ToListAsync(ct);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var tasks = await _taskRepository
            .ReadWhere(t => ownedProjectIds.Contains(t.ProjectId) && t.AssignedTo != null)
            .ToListAsync(ct);

        var grouped = tasks
            .GroupBy(t => t.AssignedTo!.Value)
            .Select(g => new
            {
                UserId = g.Key,
                AssignedCount = g.Count(),
                OpenCount = g.Count(t => t.StatusId != TaskStatusIds.Completed),
                OverdueCount = g.Count(t => t.DueDate < today && t.StatusId != TaskStatusIds.Completed),
            })
            .ToList();

        var result = new List<TeamMemberWorkloadDto>();
        foreach (var g in grouped)
        {
            var user = await _userRepository.ReadFirstOrDefaultAsync(u => u.UserId == g.UserId);
            result.Add(new TeamMemberWorkloadDto(g.UserId, user is null ? "Unknown" : $"{user.FirstName} {user.LastName}", g.AssignedCount, g.OpenCount, g.OverdueCount));
        }
        return result;
    }
}
