public class GetTaskByIdQuery : IRequest<TaskItemDto>
{
    public int TaskId { get; set; }
}

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskItemDto>
{
    private readonly ITaskRepository _taskRepository;
    public GetTaskByIdQueryHandler(ITaskRepository taskRepository) => _taskRepository = taskRepository;

    public async Task<TaskItemDto> Handle(GetTaskByIdQuery request, CancellationToken ct)
    {
        var t = await _taskRepository.GetByIdWithDetailsAsync(request.TaskId)
            ?? throw new NotFoundException($"Task {request.TaskId} not found.");

        return new TaskItemDto(
            t.TaskId, t.ProjectId, t.ParentTaskId, t.Title, t.Description,
            t.AssignedTo, t.Assignee is null ? null : t.Assignee.FirstName + " " + t.Assignee.LastName,
            t.CreatedBy, t.StatusId, t.Status.StatusName, t.PriorityId, t.Priority.PriorityName,
            t.StartDate, t.DueDate, t.EstimatedHours, t.ActualHours,
            t.CreatedDate, t.UpdatedDate, t.CompletedDate,
            t.TaskTags.Select(tt => tt.Tag.TagName).ToList(),
            t.TaskTags.Select(tt => tt.TagId).ToList());
    }
}
