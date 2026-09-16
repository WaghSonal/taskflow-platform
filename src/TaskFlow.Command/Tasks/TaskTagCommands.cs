public class AddTaskTagCommand : IRequest
{
    public int TaskId { get; set; }
    public int TagId { get; set; }
}

public class AddTaskTagCommandHandler : IRequestHandler<AddTaskTagCommand>
{
    private readonly IRepository<TaskTag> _taskTagRepository;
    public AddTaskTagCommandHandler(IRepository<TaskTag> taskTagRepository) => _taskTagRepository = taskTagRepository;

    public Task Handle(AddTaskTagCommand request, CancellationToken ct) =>
        _taskTagRepository.CreateAsync(new TaskTag { TaskId = request.TaskId, TagId = request.TagId });
}

public class RemoveTaskTagCommand : IRequest
{
    public int TaskId { get; set; }
    public int TagId { get; set; }
}

public class RemoveTaskTagCommandHandler : IRequestHandler<RemoveTaskTagCommand>
{
    private readonly IRepository<TaskTag> _taskTagRepository;
    public RemoveTaskTagCommandHandler(IRepository<TaskTag> taskTagRepository) => _taskTagRepository = taskTagRepository;

    public async Task Handle(RemoveTaskTagCommand request, CancellationToken ct)
    {
        var taskTag = await _taskTagRepository.ReadFirstOrDefaultAsync(tt => tt.TaskId == request.TaskId && tt.TagId == request.TagId)
            ?? throw new NotFoundException($"Tag {request.TagId} is not on task {request.TaskId}.");

        await _taskTagRepository.DeleteAsync(taskTag);
    }
}
