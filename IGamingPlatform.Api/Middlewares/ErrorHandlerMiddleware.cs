using IGamingPlatform.Api.Extensions;
using IGamingPlatform.Shared;
using IGamingPlatform.Shared.Exceptions;
using IGamingPlatform.Shared.Extensions;
using System.Net;
using System.Net.Mime;

namespace IGamingPlatform.Api.Middlewares;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    private const string InternalServerErrorMessage = "An error occured during executing request.";
    private const string ValidationErrorMessage = "A validation error occured during executing request.";

    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<ErrorHandlerMiddleware>>();

            var responseResult = exception switch
            {
                AppException e => HandleAppException(context, logger, e),
                FluentValidation.ValidationException e => HandleValidationException(context, logger, e, GetErrors(e)),
                _ => HandleInternalError(context, logger, exception)
            };

            context.Response.ContentType = MediaTypeNames.Application.Json;
            await context.Response.WriteAsync(responseResult);
        }
    }

    private static string HandleAppException(HttpContext context, ILogger logger, AppException exception)
    {
        logger.LogError(exception.HResult, exception, exception.LogMessage);

        context.Response.StatusCode = exception.StatusCode;

        var problemDetails = exception.GetProblemDetails();
        problemDetails.Instance = context.Request.Path;

        return problemDetails.Serialize();
    }

    private static string HandleInternalError(HttpContext context, ILogger logger, Exception ex)
    {
        logger.LogError(ex.HResult, ex, InternalServerErrorMessage);

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var problemDetails = context.Request.Path.ToInternalServerErrorDetails();
        return problemDetails.Serialize();
    }

    private static string HandleValidationException(HttpContext context, ILogger logger, Exception ex, IEnumerable<Error> errors)
    {
        logger.LogWarning(ex.HResult, ex, ValidationErrorMessage);

        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var problemDetails = context.Request.Path.ToValidationErrorDetails(errors);
        return problemDetails.Serialize();
    }

    private static IEnumerable<Error> GetErrors(FluentValidation.ValidationException e)
    {
        return e.Errors.Select(x => new Error
        {
            Message = x.ErrorMessage,
            ErrorCode = x.ErrorCode
        });
    }
}

