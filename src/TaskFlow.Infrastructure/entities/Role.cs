// TaskFlow.Infrastructure/entities/Role.cs
//
// WHAT: EF Core entity mapping onto the hand-authored `Roles` lookup table in TaskManagementDB.
// WHY:  Like User, this table was created by hand in SSMS (see
//       "Task Management Queries/SQLQuery3.sql"), so property names mirror the real column names
//       (RoleId, RoleName) rather than EF's default "Id"/"Name" conventions.
// WHERE USED: Joined through UserRole and read by UserRepository during login so
//             LoginCommandHandler (TaskFlow.Command/Auth/LoginCommand.cs) can collect the role
//             names for the current user. Those names become the JWT's role claims
//             (JwtTokenGenerator) and drive the frontend's role-based navigation/RoleGuard.

/// <summary>
/// One of the three fixed access levels in TaskFlow: "Admin", "Project Manager", "Employee".
/// Maps 1:1 onto the existing <c>Roles</c> table (already seeded with those three rows).
/// </summary>
public class Role
{
    /// <summary>Primary key. Matches column <c>Roles.RoleId</c>.</summary>
    public int RoleId { get; set; }

    /// <summary>The role's display/claim name — "Admin" | "Project Manager" | "Employee". Matches <c>Roles.RoleName</c>.</summary>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>Optional human-readable description of the role. Matches <c>Roles.Description</c>. Not currently surfaced by any API.</summary>
    public string? Description { get; set; }
}
