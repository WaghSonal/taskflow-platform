using Microsoft.EntityFrameworkCore;

public class GetTaskDependenciesQuery : IRequest<List<TaskDependencyDto>>
{
    public int TaskId { get; set; }
}

public class GetTaskDependenciesQueryHandler : IRequestHandler<GetTaskDependenciesQuery, List<TaskDependencyDto>>
{
    private readonly IRepository<TaskDependency> _dependencyRepository;
    public GetTaskDependenciesQueryHandler(IRepository<TaskDependency> dependencyRepository) => _dependencyRepository = dependencyRepository;

    public Task<List<TaskDependencyDto>> Handle(GetTaskDependenciesQuery request, CancellationToken ct) =>
        _dependencyRepository.ReadWhere(d => d.TaskId == request.TaskId)
            .Select(d => new TaskDependencyDto(d.TaskDependencyId, d.TaskId, d.DependsOnTaskId, d.DependencyType))
            .ToListAsync(ct);
}
