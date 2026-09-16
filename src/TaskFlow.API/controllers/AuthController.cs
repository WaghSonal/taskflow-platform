// TaskFlow.API/controllers/AuthController.cs
//
// WHAT: The HTTP surface for authentication — currently `POST /api/auth/login` and
//       `GET /api/auth/me`.
// WHY:  Controllers in this backend are intentionally "thin": they only bind the HTTP
//       request/response and delegate everything else to AppServices (IAuthService). No business
//       logic (password checks, token building) lives here.
// WHERE USED: Called directly by the frontend — Frontend/src/services/api/authApi.ts posts to
//             `/auth/login` from the login page, and (once wired up) any future call needing a
//             protected round-trip check can hit `/auth/me` with the returned Bearer token.
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>Authentication endpoints, mounted at <c>/api/auth</c>.</summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    /// <summary>DI supplies the registered <see cref="IAuthService"/> (AppServices/Services/AuthService.cs).</summary>
    public AuthController(IAuthService authService) => _authService = authService;

    /// <summary>
    /// <c>POST /api/auth/login</c> — verifies the submitted credentials and, on success, returns a
    /// JWT plus a summary of the signed-in user. Marked <see cref="AllowAnonymousAttribute"/> since
    /// this is the one endpoint that must be reachable without already having a token.
    /// A failed login throws <see cref="UnauthorizedException"/> deep inside
    /// <c>LoginCommandHandler</c>, which <c>ExceptionHandlingMiddleware</c> turns into HTTP 401
    /// before it ever reaches this method's return path.
    /// </summary>
    /// <param name="command">Model-bound from the JSON request body (<c>{ email, password }</c>).</param>
    /// <returns>200 OK with a <see cref="LoginResult"/> body on success.</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginCommand command)
        => Ok(await _authService.Login(command));

    /// <summary>
    /// <c>GET /api/auth/me</c> — returns the identity/roles encoded in the caller's own JWT.
    /// Exists purely as a manual/automated smoke test that a token issued by <c>/login</c> is
    /// accepted by the JWT bearer middleware configured in Program.cs and that its role claims
    /// come back correctly; requires a valid <c>Authorization: Bearer &lt;token&gt;</c> header
    /// (enforced by <see cref="AuthorizeAttribute"/>).
    /// </summary>
    /// <returns>200 OK with the caller's user id, email, and roles read from JWT claims.</returns>
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
        => Ok(new
        {
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            Email = User.FindFirstValue(ClaimTypes.Email),
            Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value),
        });
}
