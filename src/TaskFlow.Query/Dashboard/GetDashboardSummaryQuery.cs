using Microsoft.EntityFrameworkCore;

public class GetDashboardSummaryQuery : IRequest<DashboardSummaryDto>
{
    public int UserId { get; set; }
    public string Role { get; set; } = string.Empty; // read from the JWT claim, never from the request body
}

public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;

    public GetDashboardSummaryQueryHandler(ITaskRepository taskRepository, IProjectRepository projectRepository, IUserRepository userRepository)
    { _taskRepository = taskRepository; _projectRepository = projectRepository; _userRepository = userRepository; }

    public Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken ct) => request.Role switch
    {
        "Admin" => BuildAdminSummary(ct),
        "Project Manager" => BuildManagerSummary(request.UserId, ct),
        _ => BuildEmployeeSummary(request.UserId, ct),
    };

    private async Task<DashboardSummaryDto> BuildEmployeeSummary(int userId, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var weekFromNow = today.AddDays(7);
        var myTasks = await _taskRepository.ReadWhere(t => t.AssignedTo == userId).ToListAsync(ct);

        return new DashboardSummaryDto(
            OpenTaskCount: myTasks.Count(t => t.StatusId != TaskStatusIds.Completed),
            CompletedTaskCount: myTasks.Count(t => t.StatusId == TaskStatusIds.Completed),
            OverdueTaskCount: myTasks.Count(t => t.DueDate < today && t.StatusId != TaskStatusIds.Completed),
            DueThisWeekCount: myTasks.Count(t => t.DueDate != null && t.DueDate >= today && t.DueDate <= weekFromNow && t.StatusId != TaskStatusIds.Completed),
            TotalProjects: null, ActiveProjects: null, TotalTasks: null, TotalUsers: null, ActiveTaskCount: null);
    }

    private async Task<DashboardSummaryDto> BuildManagerSummary(int managerId, CancellationToken ct)
    {
        var myProjects = await _projectRepository.ReadWhere(p => p.ProjectManagerId == managerId).ToListAsync(ct);
        var projectIds = myProjects.Select(p => p.ProjectId).ToList();
        var myTasks = await _taskRepository.ReadWhere(t => projectIds.Contains(t.ProjectId)).ToListAsync(ct);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        return new DashboardSummaryDto(
            OpenTaskCount: null, CompletedTaskCount: null,
            OverdueTaskCount: myTasks.Count(t => t.DueDate < today && t.StatusId != TaskStatusIds.Completed),
            DueThisWeekCount: null,
            TotalProjects: myProjects.Count,
            ActiveProjects: myProjects.Count(p => p.Status == "Active"),
            TotalTasks: myTasks.Count,
            TotalUsers: null,
            ActiveTaskCount: myTasks.Count(t => t.StatusId != TaskStatusIds.Completed && t.StatusId != TaskStatusIds.Cancelled));
    }

    private async Task<DashboardSummaryDto> BuildAdminSummary(CancellationToken ct)
    {
        var totalUsers = await _userRepository.ReadWhere(u => true).CountAsync(ct);
        var totalProjects = await _projectRepository.ReadWhere(p => true).CountAsync(ct);
        var allTasks = await _taskRepository.ReadWhere(t => true).ToListAsync(ct);

        return new DashboardSummaryDto(
            OpenTaskCount: null, CompletedTaskCount: allTasks.Count(t => t.StatusId == TaskStatusIds.Completed),
            OverdueTaskCount: null, DueThisWeekCount: null,
            TotalProjects: totalProjects, ActiveProjects: null,
            TotalTasks: allTasks.Count, TotalUsers: totalUsers,
            ActiveTaskCount: allTasks.Count(t => t.StatusId != TaskStatusIds.Completed && t.StatusId != TaskStatusIds.Cancelled));
    }
}
