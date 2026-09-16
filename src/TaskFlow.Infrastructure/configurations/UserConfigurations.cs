// TaskFlow.Infrastructure/configurations/UserConfigurations.cs
//
// WHAT: EF Core Fluent API configuration for User, Role, and UserRole — table names, keys, and
//       relationships.
// WHY:  Maps onto the schema already created by hand in SSMS (see "Task Management Queries/*.sql")
//       — table/column names here must match that schema exactly, since EF isn't the tool that
//       created these tables. There is deliberately no `HasData(...)` seeding in this file:
//       Users/Roles/UserRoles are already populated in TaskManagementDB, so seeding is not EF's
//       job for this database (that would either duplicate rows or require running migrations
//       against a database this project doesn't manage via migrations at all).
// WHERE USED: Applied automatically by TaskFlowDbContext.OnModelCreating via
//             ApplyConfigurationsFromAssembly — never referenced directly by other code.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/// <summary>Maps <see cref="User"/> onto the existing <c>Users</c> table.</summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <summary>
    /// Declares the table name, primary key (<c>UserId</c>), a required/unique <c>Email</c> column
    /// (login looks users up by email, so this index also makes that lookup efficient), and the
    /// one-to-many relationship to <see cref="UserRole"/>.
    /// </summary>
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(x => x.UserId);
        builder.Property(x => x.Email).HasMaxLength(255).IsRequired();
        builder.HasIndex(x => x.Email).IsUnique();

        builder.HasMany(x => x.UserRoles).WithOne(x => x.User).HasForeignKey(x => x.UserId);

        // Matches the real FK_Users_Departments constraint (added in Phase 6, once Department exists).
        builder.HasOne(x => x.Department).WithMany().HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.SetNull);
    }
}

/// <summary>Maps <see cref="UserRole"/> onto the existing <c>UserRoles</c> join table.</summary>
public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    /// <summary>Declares the composite primary key (<c>UserId</c>, <c>RoleId</c>) and the foreign key to <see cref="Role"/>.</summary>
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");
        builder.HasKey(x => new { x.UserId, x.RoleId });
        builder.HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleId);
    }
}

/// <summary>Maps <see cref="Role"/> onto the existing <c>Roles</c> lookup table.</summary>
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    /// <summary>Declares the table name and primary key (<c>RoleId</c>). Rows (Admin/Project Manager/Employee) already exist — nothing to seed.</summary>
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(x => x.RoleId);
    }
}
