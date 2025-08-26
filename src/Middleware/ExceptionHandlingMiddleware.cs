using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Inquiries.Api.Middleware;

public class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) => _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ArgumentException ex)
        {
            await WriteProblem(context, ex, HttpStatusCode.UnprocessableEntity);
        }
        catch (KeyNotFoundException ex)
        {
            await WriteProblem(context, ex, HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            await WriteProblem(context, ex, HttpStatusCode.InternalServerError);
        }
    }

    private async Task WriteProblem(HttpContext ctx, Exception ex, HttpStatusCode code)
    {
        _logger.LogError(ex, "Unhandled error");
        ctx.Response.StatusCode = (int)code;
        ctx.Response.ContentType = "application/problem+json";

        var pd = new ProblemDetails
        {
            Status = (int)code,
            Title = code.ToString(),
            Detail = ex.Message,
            Instance = ctx.Request.Path
        };
        await ctx.Response.WriteAsJsonAsync(pd);
    }
}
