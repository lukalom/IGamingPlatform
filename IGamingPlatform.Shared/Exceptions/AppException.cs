using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace IGamingPlatform.Shared.Exceptions;

public abstract class AppException : Exception
{
    protected AppException() { }
    protected AppException(string message) : base(message) { }

    /// <summary>
    /// Gets Http status code.
    /// </summary>
    protected abstract HttpStatusCode HttpStatusCode { get; }

    /// <summary>
    /// Gets error text if Message is empty.
    /// </summary>
    protected virtual string DefaultMessage => "A error occurred during executing request.";

    /// <summary>
    /// Gets http status code number.
    /// </summary>
    public int StatusCode => (int)HttpStatusCode;

    /// <summary>
    /// Gets message for logging.
    /// </summary>
    public string LogMessage => string.IsNullOrWhiteSpace(Message) ? DefaultMessage : Message;

    /// <summary>
    /// Returns default problem details.
    /// </summary>
    public abstract ProblemDetails GetProblemDetails();
}
