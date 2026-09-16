public class CreateTaskCommand : IRequest<int>
{
    public int ProjectId { get; set; }
    public int? ParentTaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? AssignedTo { get; set; }
    public int CreatedBy { get; set; }
    public int StatusId { get; set; } = TaskStatusIds.Backlog;
    public int PriorityId { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public decimal? EstimatedHours { get; set; }
}

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, int>
{
    private readonly ITaskRepository _taskRepository;
    public CreateTaskCommandHandler(ITaskRepository taskRepository) => _taskRepository = taskRepository;

    public async Task<int> Handle(CreateTaskCommand request, CancellationToken ct)
    {
        var task = new TaskItem
        {
            ProjectId = request.ProjectId,
            ParentTaskId = request.ParentTaskId,
            Title = request.Title,
            Description = request.Description,
            AssignedTo = request.AssignedTo,
            CreatedBy = request.CreatedBy,
            StatusId = request.StatusId,
            PriorityId = request.PriorityId,
            StartDate = request.StartDate,
            DueDate = request.DueDate,
            EstimatedHours = request.EstimatedHours,
        };
        await _taskRepository.CreateAsync(task);
        return task.TaskId;
    }
}
