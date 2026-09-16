public class TeamService : ITeamService
{
    private readonly Dispatcher _dispatcher;
    public TeamService(Dispatcher dispatcher) => _dispatcher = dispatcher;

    public Task<List<TeamMemberWorkloadDto>> GetWorkload(GetTeamWorkloadQuery query) => _dispatcher.Send(query);
}
