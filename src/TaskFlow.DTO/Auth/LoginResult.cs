// TaskFlow.DTO/Auth/LoginResult.cs
//
// WHAT: The data returned to the API caller after a successful login.
// WHY:  Keeps the shape of "what login gives back" as a single, explicit contract instead of
//       leaking the internal `User` EF entity (with its PasswordHash column!) out of the API.
//       Only the fields the frontend actually needs are included.
// WHERE USED: Built by LoginCommandHandler.Handle (TaskFlow.Command/Auth/LoginCommand.cs) and
//             returned as-is by AuthController.Login (TaskFlow.API/controllers/AuthController.cs)
//             as the JSON body of `POST /api/auth/login`. The frontend's
//             Frontend/src/services/api/authApi.ts maps this exact shape
//             (token/userId/email/firstName/lastName/roles) into its own AuthUser type.

/// <summary>
/// Result of a successful login: the signed JWT plus the minimal identity/role info the frontend
/// needs to render the signed-in user and drive role-based navigation. Deliberately does not
/// include anything sensitive (password hash) or anything not yet needed (department, job title —
/// those will come from a future `/api/users/{id}` endpoint once Users (Phase 4) exists).
/// </summary>
/// <param name="Token">Signed JWT the frontend stores and sends back as `Authorization: Bearer &lt;token&gt;` on later requests.</param>
/// <param name="UserId">The user's primary key (`Users.UserId` in the database).</param>
/// <param name="Email">The user's login email, echoed back for display in the UI.</param>
/// <param name="FirstName">Display name — first name.</param>
/// <param name="LastName">Display name — last name.</param>
/// <param name="Roles">Role names (e.g. "Admin", "Project Manager", "Employee") baked into the JWT's role claims and used by the frontend's RoleGuard for navigation/permissions.</param>
public record LoginResult(string Token, int UserId, string Email, string FirstName, string LastName, List<string> Roles);
