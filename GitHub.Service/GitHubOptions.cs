namespace GitHub.Service;

public class GitHubOptions
{
    public const string GitHub = "GitHubOptions";

    public string? Username { get; set; }
    public string? PersonalAccessToken { get; set; }
}
