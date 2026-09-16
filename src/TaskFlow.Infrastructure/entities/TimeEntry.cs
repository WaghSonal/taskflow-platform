public class TimeEntry
{
    public int TimeEntryId { get; set; }
    public int TaskId { get; set; }
    public int UserId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? DurationMinutes { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public TaskItem Task { get; set; } = null!;
    public User User { get; set; } = null!;
}
