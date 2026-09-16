public record ProjectDto(
    int ProjectId,
    string ProjectCode,
    string ProjectName,
    string? Description,
    DateOnly StartDate,
    DateOnly? EndDate,
    int ProjectManagerId,
    string ProjectManagerName,
    string Status,
    string Priority,
    int CreatedBy,
    DateTime CreatedDate,
    DateTime? UpdatedDate);

public record ProjectMemberDto(int ProjectMemberId, int ProjectId, int UserId, string UserName, DateOnly JoinedDate, bool IsActive);
