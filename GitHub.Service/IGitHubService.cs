using Octokit;

namespace GitHub.Service;
public interface IGitHubService
{
    string Username { get; }

    Task<IEnumerable<RepositoryData>> GetPortfolio();

    Task<IEnumerable<Repository>> SearchRepositories(string repoName, string language, string username);
}