using Microsoft.EntityFrameworkCore;

public class GetTasksQuery : IRequest<List<TaskItemDto>>
{
    public string? Search { get; set; }
    public int? ProjectId { get; set; }
    public int? StatusId { get; set; }
    public int? PriorityId { get; set; }
    public int? AssignedTo { get; set; }
    public int? ParentTaskId { get; set; }
    public bool? RootOnly { get; set; }
}

public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, List<TaskItemDto>>
{
    private readonly ITaskRepository _taskRepository;
    public GetTasksQueryHandler(ITaskRepository taskRepository) => _taskRepository = taskRepository;

    public async Task<List<TaskItemDto>> Handle(GetTasksQuery request, CancellationToken ct)
    {
        var query = _taskRepository.ReadWhere(t =>
            (request.ProjectId == null || t.ProjectId == request.ProjectId) &&
            (request.StatusId == null || t.StatusId == request.StatusId) &&
            (request.PriorityId == null || t.PriorityId == request.PriorityId) &&
            (request.AssignedTo == null || t.AssignedTo == request.AssignedTo) &&
            (request.ParentTaskId == null || t.ParentTaskId == request.ParentTaskId) &&
            (request.RootOnly != true || t.ParentTaskId == null) &&
            (request.Search == null || t.Title.Contains(request.Search)));

        return await query
            .Select(t => new TaskItemDto(
                t.TaskId, t.ProjectId, t.ParentTaskId, t.Title, t.Description,
                t.AssignedTo, t.Assignee == null ? null : t.Assignee.FirstName + " " + t.Assignee.LastName,
                t.CreatedBy, t.StatusId, t.Status.StatusName, t.PriorityId, t.Priority.PriorityName,
                t.StartDate, t.DueDate, t.EstimatedHours, t.ActualHours,
                t.CreatedDate, t.UpdatedDate, t.CompletedDate,
                t.TaskTags.Select(tt => tt.Tag.TagName).ToList(),
                t.TaskTags.Select(tt => tt.TagId).ToList()))
            .ToListAsync(ct);
    }
}
