// TaskFlow.API/Middleware/ExceptionHandlingMiddleware.cs
//
// WHAT: Global exception handler — maps known exception types to clean HTTP responses and logs
//       anything unexpected instead of letting it crash the request with a raw stack trace.
// WHY:  The reference backend this project was modelled on (OneRivet) has a full exception-handling
//       middleware shipped from a private NuGet package this project doesn't have access to. This
//       is the from-scratch stand-in, grown alongside the exception types each feature phase
//       actually introduced: UnauthorizedException (Phase 2 — login), NotFoundException (Phase 3+
//       — update/delete-by-id across every feature), DbUpdateException (Phase 3+ — FK/unique
//       violations from EF Core, e.g. a duplicate Project.ProjectCode or a bogus ManagerId), and
//       a final catch-all for anything else so the caller always gets a JSON body, never HTML.
// WHERE USED: Registered in Program.cs via `app.UseMiddleware<ExceptionHandlingMiddleware>()`,
//             placed before UseAuthentication/UseAuthorization so it wraps the whole request
//             pipeline including every controller action across every feature phase.
using Microsoft.EntityFrameworkCore;

/// <summary>Wraps the rest of the request pipeline in a try/catch and converts known exceptions into clean HTTP responses.</summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>ASP.NET Core supplies the next delegate and a logger when this middleware is registered via <c>UseMiddleware&lt;T&gt;()</c>.</summary>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    { _next = next; _logger = logger; }

    /// <summary>
    /// Invoked by the ASP.NET Core pipeline for every request. Runs the rest of the pipeline
    /// (routing, auth, the controller action) inside a try/catch, converting each known exception
    /// type into a JSON <c>{ "message": "..." }</c> body with the matching status code. Anything
    /// not recognized is logged with its full stack trace (so it's diagnosable) but reported to the
    /// caller only as a generic 500 message (so internal details never leak to a client).
    /// </summary>
    /// <param name="context">The current HTTP request/response context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UnauthorizedException ex)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new { message = ex.Message });
        }
        catch (DbUpdateException ex)
        {
            // Typically a unique-index or foreign-key violation the real database enforces
            // (e.g. a duplicate Project.ProjectCode, or a ManagerId that doesn't exist) —
            // a client input problem, not a server bug, so 400 rather than 500.
            _logger.LogWarning(ex, "Database update failed for {Path}", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { message = "The request could not be completed because it would violate a database constraint." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for {Path}", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { message = "An unexpected error occurred." });
        }
    }
}
