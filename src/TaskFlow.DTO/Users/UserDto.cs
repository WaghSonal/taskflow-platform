public record UserDto(
    int UserId,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    int? DepartmentId,
    bool IsActive,
    DateTime CreatedDate,
    List<string> Roles,
    List<int> RoleIds);
