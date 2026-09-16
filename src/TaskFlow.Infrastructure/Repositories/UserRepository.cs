// TaskFlow.Infrastructure/Repositories/UserRepository.cs
//
// WHAT: Concrete EF Core repository for User, specialised to eagerly load role data.
// WHY:  LoginCommandHandler needs `user.UserRoles[i].Role.RoleName` for every role the user has,
//       in the same round trip as the login lookup. The generic Repository<T>.ReadFirstOrDefaultAsync
//       does a plain FirstOrDefaultAsync with no eager loading, which would leave UserRoles empty
//       (or trigger lazy-load exceptions, since lazy loading isn't enabled) — so this class
//       overrides just that one method to add the necessary .Include/.ThenInclude.
// WHERE USED: Registered in Program.cs as `AddScoped<IUserRepository, UserRepository>()` and
//             injected into LoginCommandHandler (TaskFlow.Command/Auth/LoginCommand.cs).
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// <see cref="IUserRepository"/> implementation that always eager-loads each user's roles, since
/// every current caller (login) needs them. See file header for why this isn't just the generic
/// <see cref="Repository{T}"/> behaviour.
/// </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    private readonly TaskFlowDbContext _context;

    /// <summary>Same scoped <see cref="TaskFlowDbContext"/> is passed to the base <see cref="Repository{T}"/> and kept here for the overridden query below.</summary>
    public UserRepository(TaskFlowDbContext context) : base(context) => _context = context;

    /// <summary>
    /// Overrides the base lookup to eager-load <c>UserRoles -&gt; Role</c> in the same query, so
    /// LoginCommandHandler can read role names off the returned user without a second database call.
    /// </summary>
    /// <param name="predicate">Filter used by LoginCommandHandler: matches by email and <c>IsActive</c>.</param>
    /// <returns>The matching user with roles populated, or <c>null</c> if none match.</returns>
    public override async Task<User?> ReadFirstOrDefaultAsync(Expression<Func<User, bool>> predicate) =>
        await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(predicate);
}
