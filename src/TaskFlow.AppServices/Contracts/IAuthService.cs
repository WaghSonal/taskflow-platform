// TaskFlow.AppServices/Contracts/IAuthService.cs
//
// WHAT: The façade contract Controllers use for authentication — one method per auth-related
//       use case (currently just Login).
// WHY:  AppServices is the layer Controllers are allowed to depend on directly (per the layering
//       described in the backend blueprint: API -> AppServices -> Command/Query -> Infrastructure).
//       Controllers never call Dispatcher.Send(...) or reference a Command/Query type themselves —
//       that indirection is what lets the underlying dispatch mechanism (currently the hand-rolled
//       Dispatcher, potentially MediatR later) change without touching any controller.
// WHERE USED: Injected into AuthController (TaskFlow.API/controllers/AuthController.cs).
//             Implemented by AuthService (Services/AuthService.cs) and registered in Program.cs
//             as `AddScoped<IAuthService, AuthService>()`.

/// <summary>Authentication use cases exposed to the API layer. See <see cref="AuthService"/> for the implementation.</summary>
public interface IAuthService
{
    /// <summary>Runs a login attempt end-to-end and returns the JWT + user summary on success.</summary>
    /// <param name="command">The submitted email/password, already bound from the HTTP request body.</param>
    /// <returns>The signed token plus the minimal user info the frontend needs.</returns>
    Task<LoginResult> Login(LoginCommand command);
}
