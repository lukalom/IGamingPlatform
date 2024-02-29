using Asp.Versioning;
using IGamingPlatform.Api.Abstractions;
using IGamingPlatform.Application.Users.Commands.LoginCommand;
using IGamingPlatform.Application.Users.Commands.RegisterCommand;
using IGamingPlatform.Application.Users.Queries.UserProfileQuery;
using IGamingPlatform.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IGamingPlatform.Api.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class UsersController : ApiControllerBase
{
    private readonly IMediator _mediator;
    private readonly ApplicationContext _applicationContext;

    public UsersController(IMediator mediator, ApplicationContext applicationContext)
    {
        _mediator = mediator;
        _applicationContext = applicationContext;
    }

    /// <summary>
    /// Description: This endpoint should retrieve the user's profile information based on the provided JWT token.
    /// </summary>
    /// <returns>UserProfileDto</returns>
    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<UserProfileDto>> ProfileAsync()
        => await _mediator.Send(new UserProfileQuery(_applicationContext.UserId));

    /// <summary>
    /// Description: This endpoint should authenticate users by validating their credentials (username and password).
    /// </summary>
    /// <param name="command"></param>
    /// <returns>LoginUserResponse</returns>
    [HttpPost("authenticate")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginUserResponse>> LoginAsync([FromBody] LoginUserCommand command) => await _mediator.Send(command);


    /// <summary>
    /// Description: This endpoint should allow users to register by providing necessary details such as username, email,
    /// password, and confirming the password.
    /// </summary>
    /// <param name="command"></param>
    /// <returns>RegisterUserResponse</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<RegisterUserResponse>> RegisterAsync([FromBody] RegisterUserCommand command) => await _mediator.Send(command);
}