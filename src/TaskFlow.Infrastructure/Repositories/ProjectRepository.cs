using Microsoft.EntityFrameworkCore;

public class ProjectRepository : Repository<Project>, IProjectRepository
{
    private readonly TaskFlowDbContext _context;
    public ProjectRepository(TaskFlowDbContext context) : base(context) => _context = context;

    public async Task<Project?> GetByIdWithMembersAsync(int projectId) =>
        await _context.Projects
            .Include(p => p.Members).ThenInclude(m => m.User)
            .FirstOrDefaultAsync(p => p.ProjectId == projectId);
}
