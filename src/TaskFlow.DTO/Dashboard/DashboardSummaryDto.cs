// One shape for all three roles — fields not relevant to a role are left null rather than
// having three separate DTOs, since the frontend already renders per-role dashboard cards
// conditionally based on which fields are present.
public record DashboardSummaryDto(
    int? OpenTaskCount,
    int? CompletedTaskCount,
    int? OverdueTaskCount,
    int? DueThisWeekCount,
    int? TotalProjects,
    int? ActiveProjects,
    int? TotalTasks,
    int? TotalUsers,
    int? ActiveTaskCount);

public record TeamMemberWorkloadDto(int UserId, string UserName, int AssignedCount, int OpenCount, int OverdueCount);
