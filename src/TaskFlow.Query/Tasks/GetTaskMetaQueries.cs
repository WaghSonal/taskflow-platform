// Small read-only lookup queries backing the frontend's task-create/edit form dropdowns
// (status, priority, tag pickers) — mirrors mock/taskMeta.mock.ts.
using Microsoft.EntityFrameworkCore;

public class GetTaskStatusesQuery : IRequest<List<TaskStatusDto>> { }

public class GetTaskStatusesQueryHandler : IRequestHandler<GetTaskStatusesQuery, List<TaskStatusDto>>
{
    private readonly IRepository<TaskStatusLookup> _repository;
    public GetTaskStatusesQueryHandler(IRepository<TaskStatusLookup> repository) => _repository = repository;

    public Task<List<TaskStatusDto>> Handle(GetTaskStatusesQuery request, CancellationToken ct) =>
        _repository.ReadWhere(s => s.IsActive).OrderBy(s => s.DisplayOrder)
            .Select(s => new TaskStatusDto(s.StatusId, s.StatusName, s.DisplayOrder))
            .ToListAsync(ct);
}

public class GetTaskPrioritiesQuery : IRequest<List<TaskPriorityDto>> { }

public class GetTaskPrioritiesQueryHandler : IRequestHandler<GetTaskPrioritiesQuery, List<TaskPriorityDto>>
{
    private readonly IRepository<TaskPriorityLookup> _repository;
    public GetTaskPrioritiesQueryHandler(IRepository<TaskPriorityLookup> repository) => _repository = repository;

    public Task<List<TaskPriorityDto>> Handle(GetTaskPrioritiesQuery request, CancellationToken ct) =>
        _repository.ReadWhere(p => p.IsActive).OrderBy(p => p.Level)
            .Select(p => new TaskPriorityDto(p.PriorityId, p.PriorityName, p.Level))
            .ToListAsync(ct);
}

public class GetTagsQuery : IRequest<List<TagDto>> { }

public class GetTagsQueryHandler : IRequestHandler<GetTagsQuery, List<TagDto>>
{
    private readonly IRepository<Tag> _repository;
    public GetTagsQueryHandler(IRepository<Tag> repository) => _repository = repository;

    public Task<List<TagDto>> Handle(GetTagsQuery request, CancellationToken ct) =>
        _repository.ReadWhere(t => true).OrderBy(t => t.TagName)
            .Select(t => new TagDto(t.TagId, t.TagName))
            .ToListAsync(ct);
}
