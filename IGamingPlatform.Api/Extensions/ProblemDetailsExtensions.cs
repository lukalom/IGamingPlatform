using System.Net;
using IGamingPlatform.Shared;
using Microsoft.AspNetCore.Mvc;

namespace IGamingPlatform.Api.Extensions;

internal static class ProblemDetailsExtensions
{
    internal static ProblemDetails ToInternalServerErrorDetails(this PathString path) =>
        new()
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "Internal Server Error",
            Detail = "An internal server error occurred while processing your request.",
            Instance = path
        };

    internal static ProblemDetails ToValidationErrorDetails(this PathString path, IEnumerable<Error> errors) =>
        new()
        {
            Status = (int)HttpStatusCode.BadRequest,
            Title = "Validation Error",
            Detail = "One or more validation errors occurred.",
            Instance = path,
            Extensions =
            {
                ["errors"] = errors
            }
        };
}