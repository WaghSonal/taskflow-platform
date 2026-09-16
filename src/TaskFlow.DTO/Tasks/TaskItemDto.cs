public record TaskItemDto(
    int TaskId,
    int ProjectId,
    int? ParentTaskId,
    string Title,
    string? Description,
    int? AssignedTo,
    string? AssigneeName,
    int CreatedBy,
    int StatusId,
    string StatusName,
    int PriorityId,
    string PriorityName,
    DateOnly? StartDate,
    DateOnly? DueDate,
    decimal? EstimatedHours,
    decimal ActualHours,
    DateTime CreatedDate,
    DateTime? UpdatedDate,
    DateTime? CompletedDate,
    List<string> Tags,
    List<int> TagIds);

public record TaskStatusDto(int StatusId, string StatusName, int DisplayOrder);
public record TaskPriorityDto(int PriorityId, string PriorityName, int Level);
public record TagDto(int TagId, string TagName);
public record TaskDependencyDto(int TaskDependencyId, int TaskId, int DependsOnTaskId, string DependencyType);
