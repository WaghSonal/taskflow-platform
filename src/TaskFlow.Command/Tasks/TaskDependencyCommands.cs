public class AddTaskDependencyCommand : IRequest<int>
{
    public int TaskId { get; set; }
    public int DependsOnTaskId { get; set; }
    public string DependencyType { get; set; } = "Blocks";
}

public class AddTaskDependencyCommandHandler : IRequestHandler<AddTaskDependencyCommand, int>
{
    private readonly IRepository<TaskDependency> _dependencyRepository;
    public AddTaskDependencyCommandHandler(IRepository<TaskDependency> dependencyRepository) => _dependencyRepository = dependencyRepository;

    public async Task<int> Handle(AddTaskDependencyCommand request, CancellationToken ct)
    {
        var dependency = new TaskDependency
        {
            TaskId = request.TaskId,
            DependsOnTaskId = request.DependsOnTaskId,
            DependencyType = request.DependencyType,
        };
        await _dependencyRepository.CreateAsync(dependency);
        return dependency.TaskDependencyId;
    }
}

public class RemoveTaskDependencyCommand : IRequest
{
    public int TaskDependencyId { get; set; }
}

public class RemoveTaskDependencyCommandHandler : IRequestHandler<RemoveTaskDependencyCommand>
{
    private readonly IRepository<TaskDependency> _dependencyRepository;
    public RemoveTaskDependencyCommandHandler(IRepository<TaskDependency> dependencyRepository) => _dependencyRepository = dependencyRepository;

    public async Task Handle(RemoveTaskDependencyCommand request, CancellationToken ct)
    {
        var dependency = await _dependencyRepository.ReadFirstOrDefaultAsync(d => d.TaskDependencyId == request.TaskDependencyId)
            ?? throw new NotFoundException($"Task dependency {request.TaskDependencyId} not found.");

        await _dependencyRepository.DeleteAsync(dependency);
    }
}
