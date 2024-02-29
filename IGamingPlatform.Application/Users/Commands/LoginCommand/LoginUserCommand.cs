using MediatR;

namespace IGamingPlatform.Application.Users.Commands.LoginCommand;

public record LoginUserResponse(int UserId, string Username, string Token);

public record LoginUserCommand(string Username, string Password) : IRequest<LoginUserResponse>;