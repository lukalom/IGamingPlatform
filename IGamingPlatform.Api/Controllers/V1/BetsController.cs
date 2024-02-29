using Asp.Versioning;
using IGamingPlatform.Api.Abstractions;
using IGamingPlatform.Application.Bets.Commands;
using IGamingPlatform.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IGamingPlatform.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class BetsController : ApiControllerBase
{
    private readonly IMediator _mediator;
    private readonly ApplicationContext _applicationContext;

    public BetsController(IMediator mediator, ApplicationContext applicationContext)
    {
        _mediator = mediator;
        _applicationContext = applicationContext;
    }

    /// <summary>
    /// Description: This endpoint should allow authenticated users to place bets.
    /// </summary>
    /// <param name="model"></param>
    /// <returns>RegisterUserResponse</returns>
    [HttpPost("place")]
    public async Task<ActionResult<PlaceBetResponse>> RegisterAsync([FromBody] PlaceBetModel model)
        => await _mediator.Send(new PlaceBetCommand(_applicationContext.UserId, model));
}