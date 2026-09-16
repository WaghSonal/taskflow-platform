public interface ITimeEntryService
{
    Task<List<TimeEntryDto>> GetTimeEntries(GetTimeEntriesQuery query);
    Task<int> LogTime(LogTimeCommand command);
    Task DeleteTimeEntry(int timeEntryId);
}
