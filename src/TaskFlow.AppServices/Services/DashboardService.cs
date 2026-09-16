public class DashboardService : IDashboardService
{
    private readonly Dispatcher _dispatcher;
    public DashboardService(Dispatcher dispatcher) => _dispatcher = dispatcher;

    public Task<DashboardSummaryDto> GetSummary(GetDashboardSummaryQuery query) => _dispatcher.Send(query);
}
