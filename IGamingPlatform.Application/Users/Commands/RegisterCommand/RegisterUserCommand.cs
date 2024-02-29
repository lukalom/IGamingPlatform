using MediatR;

namespace IGamingPlatform.Application.Users.Commands.RegisterCommand;

public record RegisterUserCommand(string Username, string Email, string Password, string ConfirmPassword) : IRequest<RegisterUserResponse>;

public record RegisterUserResponse(string Message, string Token);
