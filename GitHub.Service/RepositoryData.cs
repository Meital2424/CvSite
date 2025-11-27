namespace GitHub.Service;

public class RepositoryData
{
    public string Name { get; set; } = string.Empty;
    public IReadOnlyList<string> Languages { get; set; } = new List<string>();
    public DateTime LastCommitDate { get; set; }
    public int StargazersCount { get; set; }
    public int PullRequestsCount { get; set; }
    public string? HomepageUrl { get; set; }
}