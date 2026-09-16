public interface ITeamService
{
    Task<List<TeamMemberWorkloadDto>> GetWorkload(GetTeamWorkloadQuery query);
}
