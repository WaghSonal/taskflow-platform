// TaskFlow.Infrastructure/Security/JwtTokenGenerator.cs
//
// WHAT: The concrete implementation of IJwtTokenGenerator — builds and signs the JWT issued on login.
// WHY:  Lives in Infrastructure (not Command or API) because it needs IConfiguration to read the
//       Jwt:Issuer/Audience/Key/ExpiryMinutes settings (appsettings.Development.json), and because
//       "how a token is built" is an infrastructure concern the rest of the app shouldn't need to
//       know about — it only depends on the IJwtTokenGenerator interface.
// WHERE USED: Registered in Program.cs as `AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>()`
//             — safe as a singleton because its only dependency (IConfiguration) is itself a
//             singleton and GenerateToken writes no instance state, so one shared instance is
//             thread-safe across concurrent requests. Called by LoginCommandHandler
//             (TaskFlow.Command/Auth/LoginCommand.cs) after password verification succeeds. The
//             token this produces is what Program.cs's `AddJwtBearer(...)` configuration later
//             validates on every `[Authorize]` request (e.g. AuthController's `GET /api/auth/me`).
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

/// <summary>
/// Builds signed JWTs using the HMAC-SHA256 symmetric key configured under the <c>Jwt</c> section
/// of appsettings (Issuer, Audience, Key, ExpiryMinutes). See <see cref="IJwtTokenGenerator"/> for the contract.
/// </summary>
public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    /// <summary>DI supplies the app's <see cref="IConfiguration"/> so the signing key/issuer/audience never need to be hardcoded here.</summary>
    public JwtTokenGenerator(IConfiguration configuration) => _configuration = configuration;

    /// <summary>
    /// Creates the claims (id, email, name, one role claim per entry in <paramref name="roleNames"/>),
    /// signs them with the configured key, and returns the encoded token. The claim types used here
    /// (<see cref="ClaimTypes"/>) are the same ones Program.cs's JWT bearer middleware and
    /// <c>[Authorize(Roles = "...")]</c> read back out on later requests, and the same ones
    /// AuthController's <c>/me</c> endpoint reads to build its response.
    /// </summary>
    /// <param name="user">Authenticated user — already password-verified by the caller (LoginCommandHandler).</param>
    /// <param name="roleNames">This user's role names, embedded as role claims.</param>
    /// <returns>A signed, compact JWT string ready to hand back to the client.</returns>
    public string GenerateToken(User user, List<string> roleNames)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
        };
        claims.AddRange(roleNames.Select(r => new Claim(ClaimTypes.Role, r)));

        // Symmetric key + HMAC-SHA256: simplest signing scheme suitable for a single API that both
        // issues and validates its own tokens (no separate identity provider involved).
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "120"));

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiry,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
