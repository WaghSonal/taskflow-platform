public class TaskService : ITaskService
{
    private readonly Dispatcher _dispatcher;
    public TaskService(Dispatcher dispatcher) => _dispatcher = dispatcher;

    public Task<List<TaskItemDto>> GetTasks(GetTasksQuery query) => _dispatcher.Send(query);
    public Task<TaskItemDto> GetTaskById(int taskId) => _dispatcher.Send(new GetTaskByIdQuery { TaskId = taskId });
    public Task<int> CreateTask(CreateTaskCommand command) => _dispatcher.Send(command);
    public Task UpdateTask(UpdateTaskCommand command) => _dispatcher.Send(command);
    public Task UpdateTaskStatus(int taskId, int statusId) => _dispatcher.Send(new UpdateTaskStatusCommand { TaskId = taskId, StatusId = statusId });
    public Task DeleteTask(int taskId) => _dispatcher.Send(new DeleteTaskCommand { TaskId = taskId });
    public Task AddTag(int taskId, int tagId) => _dispatcher.Send(new AddTaskTagCommand { TaskId = taskId, TagId = tagId });
    public Task RemoveTag(int taskId, int tagId) => _dispatcher.Send(new RemoveTaskTagCommand { TaskId = taskId, TagId = tagId });
    public Task<List<TaskDependencyDto>> GetDependencies(int taskId) => _dispatcher.Send(new GetTaskDependenciesQuery { TaskId = taskId });
    public Task<int> AddDependency(AddTaskDependencyCommand command) => _dispatcher.Send(command);
    public Task RemoveDependency(int dependencyId) => _dispatcher.Send(new RemoveTaskDependencyCommand { TaskDependencyId = dependencyId });
    public Task<List<TaskStatusDto>> GetStatuses() => _dispatcher.Send(new GetTaskStatusesQuery());
    public Task<List<TaskPriorityDto>> GetPriorities() => _dispatcher.Send(new GetTaskPrioritiesQuery());
    public Task<List<TagDto>> GetTags() => _dispatcher.Send(new GetTagsQuery());

    public Task<List<TaskCommentDto>> GetComments(int taskId) => _dispatcher.Send(new GetTaskCommentsQuery { TaskId = taskId });
    public Task<int> AddComment(AddTaskCommentCommand command) => _dispatcher.Send(command);
    public Task UpdateComment(UpdateTaskCommentCommand command) => _dispatcher.Send(command);
    public Task DeleteComment(int commentId, int requestingUserId) => _dispatcher.Send(new DeleteTaskCommentCommand { CommentId = commentId, RequestingUserId = requestingUserId });

    public Task<List<TaskAttachmentDto>> GetAttachments(int taskId) => _dispatcher.Send(new GetTaskAttachmentsQuery { TaskId = taskId });
    public Task<int> AddAttachment(AddTaskAttachmentCommand command) => _dispatcher.Send(command);
    public Task DeleteAttachment(int attachmentId) => _dispatcher.Send(new DeleteTaskAttachmentCommand { AttachmentId = attachmentId });
}
