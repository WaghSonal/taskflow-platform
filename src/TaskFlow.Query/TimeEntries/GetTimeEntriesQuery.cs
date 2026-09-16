using Microsoft.EntityFrameworkCore;

public class GetTimeEntriesQuery : IRequest<List<TimeEntryDto>>
{
    public int? UserId { get; set; }
    public int? TaskId { get; set; }
    public DateOnly? Date { get; set; }
}

public class GetTimeEntriesQueryHandler : IRequestHandler<GetTimeEntriesQuery, List<TimeEntryDto>>
{
    private readonly IRepository<TimeEntry> _timeEntryRepository;
    public GetTimeEntriesQueryHandler(IRepository<TimeEntry> timeEntryRepository) => _timeEntryRepository = timeEntryRepository;

    public Task<List<TimeEntryDto>> Handle(GetTimeEntriesQuery request, CancellationToken ct)
    {
        var dateStart = request.Date?.ToDateTime(TimeOnly.MinValue);
        var dateEnd = request.Date?.ToDateTime(TimeOnly.MaxValue);

        return _timeEntryRepository.ReadWhere(t =>
                (request.UserId == null || t.UserId == request.UserId) &&
                (request.TaskId == null || t.TaskId == request.TaskId) &&
                (dateStart == null || (t.StartTime >= dateStart && t.StartTime <= dateEnd)))
            .OrderByDescending(t => t.StartTime)
            .Select(t => new TimeEntryDto(t.TimeEntryId, t.TaskId, t.Task.Title, t.UserId, t.User.FirstName + " " + t.User.LastName,
                t.StartTime, t.EndTime, t.DurationMinutes, t.Description, t.CreatedDate))
            .ToListAsync(ct);
    }
}
