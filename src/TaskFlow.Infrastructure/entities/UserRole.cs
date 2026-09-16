// TaskFlow.Infrastructure/entities/UserRole.cs
//
// WHAT: EF Core entity mapping onto the `UserRoles` many-to-many join table in TaskManagementDB
//       (a user can technically hold more than one role, though today's seed data gives each
//       user exactly one).
// WHY:  EF Core needs an explicit join entity (rather than a plain many-to-many skip navigation)
//       whenever the join table's own shape needs to be configured explicitly — here that's
//       because the table's composite primary key (UserId, RoleId) and both foreign keys are
//       defined in UserConfiguration (configurations/UserConfigurations.cs) to match the table
//       exactly as it was created by hand in SSMS ("Task Management Queries/SQLQuery4.sql"/"5.sql").
// WHERE USED: Loaded (with .Include/.ThenInclude) by UserRepository during login so
//             LoginCommandHandler can read `user.UserRoles.Select(ur => ur.Role.RoleName)`.

/// <summary>
/// Join row linking one <see cref="User"/> to one <see cref="Role"/>. Maps 1:1 onto the existing
/// <c>UserRoles</c> table, whose primary key is the (UserId, RoleId) pair.
/// </summary>
public class UserRole
{
    /// <summary>Foreign key to <c>Users.UserId</c>; half of the composite primary key.</summary>
    public int UserId { get; set; }

    /// <summary>Foreign key to <c>Roles.RoleId</c>; the other half of the composite primary key.</summary>
    public int RoleId { get; set; }

    /// <summary>Navigation back to the owning user. Populated when eagerly loaded via <c>.Include(u => u.UserRoles)</c>.</summary>
    public User User { get; set; } = null!;

    /// <summary>Navigation to the role this row grants. Populated when eagerly loaded via <c>.ThenInclude(ur => ur.Role)</c> in UserRepository.</summary>
    public Role Role { get; set; } = null!;
}
