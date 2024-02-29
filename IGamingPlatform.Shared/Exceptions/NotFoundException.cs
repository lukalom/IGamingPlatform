using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace IGamingPlatform.Shared.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException() { }
    public NotFoundException(string message) : base(message) { }

    /// <summary>
    /// Gets Http status code.
    /// </summary>
    protected override HttpStatusCode HttpStatusCode => HttpStatusCode.NotFound;

    /// <summary>
    /// Gets error text if Message is empty.
    /// </summary>
    protected override string DefaultMessage => "Result does not exist.";

    /// <summary>
    /// Returns default problem details.
    /// </summary>
    public override ProblemDetails GetProblemDetails()
    {
        return new()
        {
            Status = StatusCode,
            Title = "Not Found",
            Detail = LogMessage,
        };
    }
}
