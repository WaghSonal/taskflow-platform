public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummary(GetDashboardSummaryQuery query);
}
