public interface ITaskService
{
    Task<List<TaskItemDto>> GetTasks(GetTasksQuery query);
    Task<TaskItemDto> GetTaskById(int taskId);
    Task<int> CreateTask(CreateTaskCommand command);
    Task UpdateTask(UpdateTaskCommand command);
    Task UpdateTaskStatus(int taskId, int statusId);
    Task DeleteTask(int taskId);
    Task AddTag(int taskId, int tagId);
    Task RemoveTag(int taskId, int tagId);
    Task<List<TaskDependencyDto>> GetDependencies(int taskId);
    Task<int> AddDependency(AddTaskDependencyCommand command);
    Task RemoveDependency(int dependencyId);
    Task<List<TaskStatusDto>> GetStatuses();
    Task<List<TaskPriorityDto>> GetPriorities();
    Task<List<TagDto>> GetTags();

    Task<List<TaskCommentDto>> GetComments(int taskId);
    Task<int> AddComment(AddTaskCommentCommand command);
    Task UpdateComment(UpdateTaskCommentCommand command);
    Task DeleteComment(int commentId, int requestingUserId);

    Task<List<TaskAttachmentDto>> GetAttachments(int taskId);
    Task<int> AddAttachment(AddTaskAttachmentCommand command);
    Task DeleteAttachment(int attachmentId);
}
