public interface IProjectRepository : IRepository<Project>
{
    Task<Project?> GetByIdWithMembersAsync(int projectId);
}
