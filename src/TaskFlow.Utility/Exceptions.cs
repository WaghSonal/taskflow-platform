// TaskFlow.Utility/Exceptions.cs
//
// WHAT: Cross-cutting custom exception types shared by every layer.
// WHY:  TaskFlow.Utility has no project references of its own, so any type that needs to be
//       thrown from deep inside a Command/Query handler (TaskFlow.Command / TaskFlow.Query) and
//       caught up in the API layer (TaskFlow.API) has to live somewhere both ends can see without
//       creating a circular project reference. This file is that shared location.
// WHERE USED: Thrown by TaskFlow.Command/Auth/LoginCommand.cs (LoginCommandHandler) when a login
//             attempt fails. Caught by TaskFlow.API/Middleware/ExceptionHandlingMiddleware.cs,
//             which turns it into an HTTP 401 response instead of an unhandled 500 error.

/// <summary>
/// Thrown when a request is not authenticated/authorized to do what it's asking — currently used
/// for "invalid email or password" during login. <see cref="ExceptionHandlingMiddleware"/> catches
/// this specific type and converts it into an HTTP 401 Unauthorized response with the exception's
/// message as the body, so callers get a clean error instead of a stack trace.
/// </summary>
public class UnauthorizedException : Exception
{
    /// <summary>Creates the exception with the message that will be sent back to the API caller.</summary>
    /// <param name="message">Human-readable reason, safe to expose to the client (e.g. "Invalid email or password.").</param>
    public UnauthorizedException(string message) : base(message) { }
}

/// <summary>
/// Thrown when a lookup by id (or similar) finds nothing — e.g. updating a project that doesn't
/// exist. <see cref="ExceptionHandlingMiddleware"/> converts this into an HTTP 404 response.
/// </summary>
public class NotFoundException : Exception
{
    /// <param name="message">Human-readable reason, e.g. "Project 42 not found."</param>
    public NotFoundException(string message) : base(message) { }
}
