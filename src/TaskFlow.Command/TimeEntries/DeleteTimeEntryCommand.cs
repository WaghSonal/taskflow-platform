public class DeleteTimeEntryCommand : IRequest
{
    public int TimeEntryId { get; set; }
}

public class DeleteTimeEntryCommandHandler : IRequestHandler<DeleteTimeEntryCommand>
{
    private readonly IRepository<TimeEntry> _timeEntryRepository;
    public DeleteTimeEntryCommandHandler(IRepository<TimeEntry> timeEntryRepository) => _timeEntryRepository = timeEntryRepository;

    public async Task Handle(DeleteTimeEntryCommand request, CancellationToken ct)
    {
        var entry = await _timeEntryRepository.ReadFirstOrDefaultAsync(t => t.TimeEntryId == request.TimeEntryId)
            ?? throw new NotFoundException($"Time entry {request.TimeEntryId} not found.");

        await _timeEntryRepository.DeleteAsync(entry);
    }
}
