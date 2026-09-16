public class LogTimeCommand : IRequest<int>
{
    public int TaskId { get; set; }
    public int UserId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? DurationMinutes { get; set; }
    public string? Description { get; set; }
}

public class LogTimeCommandHandler : IRequestHandler<LogTimeCommand, int>
{
    private readonly IRepository<TimeEntry> _timeEntryRepository;
    private readonly ITaskRepository _taskRepository;
    public LogTimeCommandHandler(IRepository<TimeEntry> timeEntryRepository, ITaskRepository taskRepository)
    { _timeEntryRepository = timeEntryRepository; _taskRepository = taskRepository; }

    public async Task<int> Handle(LogTimeCommand request, CancellationToken ct)
    {
        var entry = new TimeEntry
        {
            TaskId = request.TaskId,
            UserId = request.UserId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            DurationMinutes = request.DurationMinutes,
            Description = request.Description,
        };
        await _timeEntryRepository.CreateAsync(entry);

        // Logging time also rolls up into the parent task's running total — the one handler in this
        // slice that writes through two repositories in the same request.
        if (request.DurationMinutes is > 0)
        {
            var task = await _taskRepository.ReadFirstOrDefaultAsync(t => t.TaskId == request.TaskId);
            if (task is not null)
            {
                task.ActualHours += Math.Round(request.DurationMinutes.Value / 60m, 2);
                await _taskRepository.UpdateAsync(task);
            }
        }

        return entry.TimeEntryId;
    }
}
