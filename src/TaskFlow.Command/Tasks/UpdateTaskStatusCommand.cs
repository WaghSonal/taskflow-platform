// The Kanban drag-and-drop endpoint — one focused command instead of a full UpdateTaskCommand,
// matching how the frontend's board only ever sends a status change.
public class UpdateTaskStatusCommand : IRequest
{
    public int TaskId { get; set; }
    public int StatusId { get; set; }
}

public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand>
{
    private readonly ITaskRepository _taskRepository;
    public UpdateTaskStatusCommandHandler(ITaskRepository taskRepository) => _taskRepository = taskRepository;

    public async Task Handle(UpdateTaskStatusCommand request, CancellationToken ct)
    {
        var task = await _taskRepository.ReadFirstOrDefaultAsync(t => t.TaskId == request.TaskId)
            ?? throw new NotFoundException($"Task {request.TaskId} not found.");

        task.StatusId = request.StatusId;
        task.UpdatedDate = DateTime.UtcNow;
        task.CompletedDate = request.StatusId == TaskStatusIds.Completed ? DateTime.UtcNow : null;

        await _taskRepository.UpdateAsync(task);
    }
}
