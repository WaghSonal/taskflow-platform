// TaskFlow.Infrastructure/contracts/IUserRepository.cs
//
// WHAT: Marker interface for the User-specific repository.
// WHY:  Doesn't add any new members over IRepository<User> today — it exists so
//       LoginCommandHandler can depend on "the repository for users" by name/intent
//       (IUserRepository) rather than the generic IRepository<User>, and so a future phase
//       (Phase 4 — Users) has an obvious, already-established place to add user-specific query
//       methods (e.g. GetByDepartment) without touching the generic contract.
// WHERE USED: Injected into LoginCommandHandler (TaskFlow.Command/Auth/LoginCommand.cs).
//             Implemented by UserRepository (Repositories/UserRepository.cs) and registered in
//             Program.cs as `AddScoped<IUserRepository, UserRepository>()`.

/// <summary>Repository contract for <see cref="User"/>. See file header for why this exists separately from <see cref="IRepository{T}"/>.</summary>
public interface IUserRepository : IRepository<User>
{
}
