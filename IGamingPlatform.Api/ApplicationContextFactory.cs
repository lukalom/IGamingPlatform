using IGamingPlatform.Infrastructure.Persistence;
using IGamingPlatform.Shared;
using IGamingPlatform.Shared.Settings;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace IGamingPlatform.Api;

public static class ApplicationContextFactory
{
    public static ApplicationContext Create(IServiceProvider serviceProvider)
    {
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        var httpContext = httpContextAccessor.HttpContext;
        var db = serviceProvider.GetRequiredService<GamingPlatformDBContext>();
        var secretKey = serviceProvider.GetRequiredService<IOptions<JwtSettings>>().Value.Key;

        if (httpContext != null)
        {
            var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (!string.IsNullOrEmpty(authHeader) && AuthenticationHeaderValue.TryParse(authHeader, out var parsedHeader))
            {
                var token = parsedHeader.Parameter;

                if (token == null)
                {
                    throw new ArgumentException("token is not valid");
                }

                var username = JwtService.GetUsernameFromToken(token, secretKey);
                var user = db.Users.FirstOrDefault(x => x.Username == username);

                if (user != null)
                {
                    return new ApplicationContext(user.Id, username, token);
                }
            }
        }

        return new ApplicationContext();
    }
}