using Microsoft.EntityFrameworkCore;

public class TaskRepository : Repository<TaskItem>, ITaskRepository
{
    private readonly TaskFlowDbContext _context;
    public TaskRepository(TaskFlowDbContext context) : base(context) => _context = context;

    public async Task<TaskItem?> GetByIdWithDetailsAsync(int taskId) =>
        await _context.Tasks
            .Include(t => t.Status)
            .Include(t => t.Priority)
            .Include(t => t.Assignee)
            .Include(t => t.TaskTags).ThenInclude(tt => tt.Tag)
            .FirstOrDefaultAsync(t => t.TaskId == taskId);
}
