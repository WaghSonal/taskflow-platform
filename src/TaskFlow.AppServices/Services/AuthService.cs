// TaskFlow.AppServices/Services/AuthService.cs
//
// WHAT: Thin AppServices façade implementing IAuthService by delegating straight to Dispatcher.
// WHY:  Every AppServices class follows the same pattern — no business logic lives here, only a
//       call into Dispatcher.Send(...). The actual login logic lives in LoginCommandHandler
//       (TaskFlow.Command/Auth/LoginCommand.cs); this class exists purely so AuthController can
//       depend on an interface (IAuthService) instead of Dispatcher/Command types directly.
// WHERE USED: Registered in Program.cs as `AddScoped<IAuthService, AuthService>()` and injected
//             into AuthController (TaskFlow.API/controllers/AuthController.cs).

/// <summary>See <see cref="IAuthService"/> — this is its only implementation.</summary>
public class AuthService : IAuthService
{
    private readonly Dispatcher _dispatcher;

    /// <summary>DI supplies the shared, scoped <see cref="Dispatcher"/> (TaskFlow.Utility/Dispatcher.cs).</summary>
    public AuthService(Dispatcher dispatcher) => _dispatcher = dispatcher;

    /// <summary>Forwards the command to whichever handler is registered for <see cref="LoginCommand"/> (currently <c>LoginCommandHandler</c>).</summary>
    /// <param name="command">The submitted email/password.</param>
    /// <returns>The result produced by <c>LoginCommandHandler.Handle</c>.</returns>
    public Task<LoginResult> Login(LoginCommand command) => _dispatcher.Send(command);
}
