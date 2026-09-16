public class UpdateTaskCommand : IRequest
{
    public int TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? AssignedTo { get; set; }
    public int PriorityId { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public decimal? EstimatedHours { get; set; }
}

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand>
{
    private readonly ITaskRepository _taskRepository;
    public UpdateTaskCommandHandler(ITaskRepository taskRepository) => _taskRepository = taskRepository;

    public async Task Handle(UpdateTaskCommand request, CancellationToken ct)
    {
        var task = await _taskRepository.ReadFirstOrDefaultAsync(t => t.TaskId == request.TaskId)
            ?? throw new NotFoundException($"Task {request.TaskId} not found.");

        task.Title = request.Title;
        task.Description = request.Description;
        task.AssignedTo = request.AssignedTo;
        task.PriorityId = request.PriorityId;
        task.StartDate = request.StartDate;
        task.DueDate = request.DueDate;
        task.EstimatedHours = request.EstimatedHours;
        task.UpdatedDate = DateTime.UtcNow;

        await _taskRepository.UpdateAsync(task);
    }
}
