public class DeleteTaskCommand : IRequest
{
    public int TaskId { get; set; }
}

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand>
{
    private readonly ITaskRepository _taskRepository;
    public DeleteTaskCommandHandler(ITaskRepository taskRepository) => _taskRepository = taskRepository;

    public async Task Handle(DeleteTaskCommand request, CancellationToken ct)
    {
        var task = await _taskRepository.ReadFirstOrDefaultAsync(t => t.TaskId == request.TaskId)
            ?? throw new NotFoundException($"Task {request.TaskId} not found.");

        await _taskRepository.DeleteAsync(task);
    }
}
