public class TimeEntryService : ITimeEntryService
{
    private readonly Dispatcher _dispatcher;
    public TimeEntryService(Dispatcher dispatcher) => _dispatcher = dispatcher;

    public Task<List<TimeEntryDto>> GetTimeEntries(GetTimeEntriesQuery query) => _dispatcher.Send(query);
    public Task<int> LogTime(LogTimeCommand command) => _dispatcher.Send(command);
    public Task DeleteTimeEntry(int timeEntryId) => _dispatcher.Send(new DeleteTimeEntryCommand { TimeEntryId = timeEntryId });
}
