// TaskFlow.Command/Auth/LoginCommand.cs
//
// WHAT: The "log a user in" Command and its Handler — the write-side vertical slice for
//       authentication. (Login is modelled as a Command, not a Query, because issuing a JWT is a
//       side-effecting security operation, not a pure read.)
// WHY:  Follows the CQRS shape every feature in this backend uses: a plain-data Command class
//       (input), and a Handler class that does the actual work via IRequestHandler<,> (from
//       TaskFlow.Utility/Dispatcher.cs). Keeping this in TaskFlow.Command (not AppServices or API)
//       means the business rule "how do you verify a login" lives in one place, independent of
//       HTTP concerns.
// WHERE USED: Constructed by AuthController.Login (TaskFlow.API/controllers/AuthController.cs)
//             from the POST body of `/api/auth/login`, then dispatched via AuthService
//             (TaskFlow.AppServices/Services/AuthService.cs) -> Dispatcher.Send(...). The handler
//             below is what actually runs; it's registered in Program.cs as
//             `AddScoped<IRequestHandler<LoginCommand, LoginResult>, LoginCommandHandler>()`.
using Microsoft.AspNetCore.Identity;

/// <summary>
/// Input to a login attempt: the credentials submitted from the frontend's login form
/// (Frontend/src/pages/auth/LoginPage.tsx). Implements <see cref="IRequest{TResponse}"/> so it can
/// be run through <see cref="Dispatcher"/>.
/// </summary>
public class LoginCommand : IRequest<LoginResult>
{
    /// <summary>Email address submitted on the login form; matched case-sensitively against <c>Users.Email</c> by <see cref="LoginCommandHandler"/>.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Plaintext password submitted on the login form; never stored, only verified against the stored hash.</summary>
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Verifies a <see cref="LoginCommand"/> and, on success, issues a JWT. This is the only place in
/// the backend that checks a password or decides whether a login is valid.
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IPasswordHasher<User> _hasher;

    /// <summary>Dependencies are resolved by DI (Program.cs) each time the handler runs — one instance per request (scoped).</summary>
    public LoginCommandHandler(IUserRepository userRepository, IJwtTokenGenerator tokenGenerator, IPasswordHasher<User> hasher)
    { _userRepository = userRepository; _tokenGenerator = tokenGenerator; _hasher = hasher; }

    /// <summary>
    /// Looks up an active user by email, verifies the submitted password against the stored hash,
    /// and — if both succeed — issues a JWT embedding the user's roles. Throws
    /// <see cref="UnauthorizedException"/> on any failure (unknown email, inactive account, or
    /// wrong password) with the same generic message in every case, so a caller can't use the
    /// error to tell whether an email address exists in the system.
    /// </summary>
    /// <param name="request">The submitted email/password.</param>
    /// <param name="ct">Cancellation token from the incoming HTTP request.</param>
    /// <returns>A <see cref="LoginResult"/> containing the JWT and the user's id/email/name/roles.</returns>
    /// <exception cref="UnauthorizedException">Thrown for any invalid-credential case; caught by ExceptionHandlingMiddleware and turned into HTTP 401.</exception>
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken ct)
    {
        // UserRepository (not the generic Repository<T>) is injected here specifically because it
        // eager-loads UserRoles -> Role, which the roleNames projection below depends on.
        var user = await _userRepository.ReadFirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive)
            ?? throw new UnauthorizedException("Invalid email or password.");

        if (_hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            throw new UnauthorizedException("Invalid email or password.");

        var roleNames = user.UserRoles.Select(ur => ur.Role.RoleName).ToList();
        return new LoginResult(_tokenGenerator.GenerateToken(user, roleNames), user.UserId, user.Email, user.FirstName, user.LastName, roleNames);
    }
}
