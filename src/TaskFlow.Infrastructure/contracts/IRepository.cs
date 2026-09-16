// TaskFlow.Infrastructure/contracts/IRepository.cs
//
// WHAT: The generic data-access contract every entity-specific repository builds on, plus its
//       one shared EF Core implementation.
// WHY:  Keeps Command/Query handlers talking to a small, testable interface (IRepository<T>)
//       instead of an EF Core DbContext directly — handlers can be unit tested against a mock
//       IRepository<T> without spinning up a real database.
// WHERE USED: IUserRepository (contracts/IUserRepository.cs) extends this; Repository<User> is
//             the base class UserRepository (Repositories/UserRepository.cs) inherits from and
//             partially overrides. Future feature phases (Projects, Tasks, ...) each get their
//             own I{Entity}Repository the same way.
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Generic CRUD contract for any EF Core entity <typeparamref name="T"/>. Every feature-specific
/// repository (e.g. <see cref="IUserRepository"/>) extends this instead of re-declaring the same
/// four operations.
/// </summary>
/// <typeparam name="T">The EF Core entity type this repository reads/writes.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>Returns a queryable, no-tracking filter over the entity set — callers add further LINQ (e.g. paging, projection) on top.</summary>
    /// <param name="predicate">Filter expression translated to SQL by EF Core.</param>
    IQueryable<T> ReadWhere(Expression<Func<T, bool>> predicate);

    /// <summary>Returns the first matching row, or <c>null</c> if none match.</summary>
    /// <param name="predicate">Filter expression translated to SQL by EF Core.</param>
    /// <remarks>Overridden by <see cref="UserRepository"/> to eagerly load role data for login.</remarks>
    Task<T?> ReadFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

    /// <summary>Inserts a new row and saves immediately.</summary>
    /// <param name="entity">The entity to insert.</param>
    /// <returns>The same entity, with any database-generated values (e.g. identity key) populated.</returns>
    Task<T> CreateAsync(T entity);

    /// <summary>Updates an existing row and saves immediately.</summary>
    /// <param name="entity">The entity with modified values.</param>
    Task UpdateAsync(T entity);

    /// <summary>Deletes a row and saves immediately.</summary>
    /// <param name="entity">The entity to remove.</param>
    Task DeleteAsync(T entity);
}

/// <summary>
/// Default <see cref="IRepository{T}"/> implementation backed directly by EF Core's
/// <see cref="TaskFlowDbContext"/>. Feature-specific repositories inherit from this and only
/// override the methods that need custom behaviour (eager loading, custom filters, etc.).
/// </summary>
/// <typeparam name="T">The EF Core entity type this repository reads/writes.</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    private readonly TaskFlowDbContext _context;

    /// <summary>Registered per-request (scoped) via DI in Program.cs alongside the DbContext it wraps.</summary>
    public Repository(TaskFlowDbContext context) => _context = context;

    /// <inheritdoc/>
    public IQueryable<T> ReadWhere(Expression<Func<T, bool>> predicate) =>
        _context.Set<T>().AsNoTracking().Where(predicate);

    /// <inheritdoc/>
    /// <remarks>
    /// Marked <c>virtual</c> specifically so <see cref="UserRepository"/> can override it to add
    /// <c>.Include(...).ThenInclude(...)</c> for role data — login needs the user's roles loaded
    /// in the same query, not a plain lookup.
    /// </remarks>
    public virtual async Task<T?> ReadFirstOrDefaultAsync(Expression<Func<T, bool>> predicate) =>
        await _context.Set<T>().FirstOrDefaultAsync(predicate);

    /// <inheritdoc/>
    public async Task<T> CreateAsync(T entity)
    {
        _context.Set<T>().Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }
}
