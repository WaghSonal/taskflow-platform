// TaskFlow.Utility/Dispatcher.cs
//
// WHAT: A minimal, hand-rolled replacement for MediatR's request/handler/dispatch pattern.
// WHY:  MediatR's newer major versions carry a commercial license for some usage tiers, and
//       that couldn't be confirmed as safe for this project at the time this was written.
//       Rather than risk a licensing problem later, every feature (starting with Login/Auth)
//       is written against the same IRequest/IRequestHandler shape MediatR itself uses, with
//       this file providing the "Send" plumbing instead of the MediatR package. If MediatR's
//       license is later confirmed acceptable, only this file's `using` in each Command/Query
//       needs to change — no Command, Query, or Handler class needs to change at all.
// WHERE USED: Registered as a scoped service in TaskFlow.API/Program.cs. AppServices classes
//             (e.g. TaskFlow.AppServices/Services/AuthService.cs) depend on Dispatcher and call
//             Send(...) instead of calling a Command/Query handler directly. This keeps
//             AppServices decoupled from which handler implementation actually runs.
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Marker interface for a Command or Query that returns a <typeparamref name="TResponse"/>.
/// Every Command (TaskFlow.Command) and Query (TaskFlow.Query) implements this so it can be
/// dispatched generically through <see cref="Dispatcher"/> without AppServices needing to know
/// the concrete handler type.
/// </summary>
/// <typeparam name="TResponse">The type returned once the request has been handled.</typeparam>
public interface IRequest<TResponse> { }

/// <summary>
/// Contract for the class that actually performs the work for a given <see cref="IRequest{TResponse}"/>.
/// Each Command/Query has exactly one handler implementing this interface (e.g. LoginCommandHandler
/// handles LoginCommand). Handlers are resolved from the DI container by <see cref="Dispatcher"/>.
/// </summary>
/// <typeparam name="TRequest">The Command or Query type this handler knows how to process.</typeparam>
/// <typeparam name="TResponse">The result type returned by <see cref="Handle"/>.</typeparam>
public interface IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Executes the business logic for <paramref name="request"/> (e.g. verifying a login,
    /// creating a project). Called by <see cref="Dispatcher.Send{TResponse}"/> — never invoked
    /// directly by a controller, so Controllers stay thin and only talk to AppServices.
    /// </summary>
    /// <param name="request">The Command or Query instance carrying the input data.</param>
    /// <param name="ct">Cancellation token propagated from the incoming HTTP request.</param>
    /// <returns>The result of processing the request.</returns>
    Task<TResponse> Handle(TRequest request, CancellationToken ct);
}

/// <summary>
/// Marker interface for a Command that returns nothing (e.g. <c>UpdateProjectCommand</c>,
/// <c>DeleteProjectCommand</c>) — the void counterpart of <see cref="IRequest{TResponse}"/>.
/// </summary>
public interface IRequest { }

/// <summary>Contract for the handler of a void <see cref="IRequest"/>. See <see cref="IRequestHandler{TRequest, TResponse}"/> for the value-returning version.</summary>
/// <typeparam name="TRequest">The Command type this handler knows how to process.</typeparam>
public interface IRequestHandler<TRequest> where TRequest : IRequest
{
    /// <summary>Executes the business logic for <paramref name="request"/> and returns nothing.</summary>
    Task Handle(TRequest request, CancellationToken ct);
}

/// <summary>
/// Resolves the correct <see cref="IRequestHandler{TRequest, TResponse}"/> for a given request at
/// runtime and invokes it. This is the single entry point every AppServices class uses to run a
/// Command or Query, so AppServices code never needs a direct reference to a specific handler class.
/// </summary>
public class Dispatcher
{
    private readonly IServiceProvider _provider;

    /// <summary>
    /// Registered as a scoped service in Program.cs; the DI container injects the request-scoped
    /// <see cref="IServiceProvider"/> so handlers can themselves have scoped dependencies (e.g. EF Core's DbContext).
    /// </summary>
    public Dispatcher(IServiceProvider provider) => _provider = provider;

    /// <summary>
    /// Looks up the <see cref="IRequestHandler{TRequest, TResponse}"/> registered for the runtime
    /// type of <paramref name="request"/> and calls its Handle method. Uses reflection
    /// (<c>MakeGenericType</c>) plus a <c>dynamic</c> call because the compile-time type of
    /// <paramref name="request"/> is only known as the base <see cref="IRequest{TResponse}"/> here.
    /// </summary>
    /// <param name="request">A Command or Query instance, e.g. a <c>LoginCommand</c>.</param>
    /// <param name="ct">Cancellation token, defaults to none.</param>
    /// <returns>Whatever the concrete handler returns for this request.</returns>
    /// <example>Called from AuthService.Login as <c>_dispatcher.Send(command)</c>.</example>
    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken ct = default)
    {
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        dynamic handler = _provider.GetRequiredService(handlerType);
        return handler.Handle((dynamic)request, ct);
    }

    /// <summary>Same resolution as <see cref="Send{TResponse}"/>, for the void <see cref="IRequest"/> shape.</summary>
    /// <param name="request">A Command instance that returns nothing, e.g. <c>UpdateProjectCommand</c>.</param>
    /// <param name="ct">Cancellation token, defaults to none.</param>
    public Task Send(IRequest request, CancellationToken ct = default)
    {
        var handlerType = typeof(IRequestHandler<>).MakeGenericType(request.GetType());
        dynamic handler = _provider.GetRequiredService(handlerType);
        return handler.Handle((dynamic)request, ct);
    }
}
