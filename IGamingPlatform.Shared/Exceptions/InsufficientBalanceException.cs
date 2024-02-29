using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace IGamingPlatform.Shared.Exceptions;

public class InsufficientBalanceException : AppException
{
    public InsufficientBalanceException() { }
    public InsufficientBalanceException(string message) : base(message) { }

    /// <summary>
    /// Gets Http status code.
    /// </summary>
    protected override HttpStatusCode HttpStatusCode => HttpStatusCode.BadRequest;

    /// <summary>
    /// Gets error text if Message is empty.
    /// </summary>
    protected override string DefaultMessage => "Insufficient balance.";

    /// <summary>
    /// Returns default problem details.
    /// </summary>
    public override ProblemDetails GetProblemDetails()
    {
        return new()
        {
            Status = StatusCode,
            Title = "Insufficient Balance",
            Detail = LogMessage,
        };
    }
}