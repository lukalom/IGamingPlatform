namespace IGamingPlatform.Shared;

public class ApplicationContext
{
    public int UserId { get; init; }

    public string Username { get; }

    public string? AccessToken { get; }

    public ApplicationContext(int userId, string username, string? accessToken)
    {
        UserId = userId;
        Username = username;
        AccessToken = accessToken;
    }

    public ApplicationContext()
    {
        
    }
}