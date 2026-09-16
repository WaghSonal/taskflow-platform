// TaskFlow.Infrastructure/contracts/IJwtTokenGenerator.cs
//
// WHAT: Contract for turning an authenticated user into a signed JWT.
// WHY:  Keeps LoginCommandHandler decoupled from the JWT library/implementation details — it
//       only needs "give me a token for this user and these roles", which also makes the
//       handler unit-testable with a mock token generator.
// WHERE USED: Implemented by JwtTokenGenerator (Security/JwtTokenGenerator.cs), registered in
//             Program.cs, and injected into LoginCommandHandler
//             (TaskFlow.Command/Auth/LoginCommand.cs) to produce the token returned in LoginResult.

/// <summary>Produces a signed JWT for an authenticated user. See <see cref="JwtTokenGenerator"/> for the implementation.</summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Builds and signs a JWT carrying the user's id/email/name as claims plus one role claim per
    /// entry in <paramref name="roleNames"/>, so ASP.NET Core's <c>[Authorize(Roles = "...")]</c>
    /// and the frontend's role-based navigation both work off the same token.
    /// </summary>
    /// <param name="user">The authenticated user; only used for claim values (id, email, name) — never re-verified here.</param>
    /// <param name="roleNames">Role names (e.g. "Admin") to embed as <c>ClaimTypes.Role</c> claims.</param>
    /// <returns>The signed, encoded JWT string.</returns>
    string GenerateToken(User user, List<string> roleNames);
}
