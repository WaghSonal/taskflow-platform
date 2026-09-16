public record TimeEntryDto(
    int TimeEntryId,
    int TaskId,
    string TaskTitle,
    int UserId,
    string UserName,
    DateTime StartTime,
    DateTime? EndTime,
    int? DurationMinutes,
    string? Description,
    DateTime CreatedDate);
