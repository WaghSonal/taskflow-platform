public interface ITaskRepository : IRepository<TaskItem>
{
    Task<TaskItem?> GetByIdWithDetailsAsync(int taskId);
}
