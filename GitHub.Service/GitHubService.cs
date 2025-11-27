using Octokit;
using Microsoft.Extensions.Options;

namespace GitHub.Service;
public class GitHubService : IGitHubService
{
    private readonly IGitHubClient _githubClient;
    public string Username { get; }

    public GitHubService(IGitHubClient githubClient, IOptionsSnapshot<GitHubOptions> options)
    {
        _githubClient = githubClient;
        if (string.IsNullOrEmpty(options.Value.Username))
        {
            throw new ArgumentNullException(nameof(options.Value.Username),
                "GitHub Username must be provided in configuration (User Secrets).");
        }
        Username = options.Value.Username;
    }

    public async Task<IEnumerable<RepositoryData>> GetPortfolio()
    {
        var repos = await _githubClient.Repository.GetAllForCurrent().ConfigureAwait(false);

        var results = new List<RepositoryData>();

        foreach (var repo in repos)
        {
            var ownerName = repo.Owner.Login;
            var repoName = repo.Name;

            var languages = await _githubClient.Repository.GetAllLanguages(ownerName, repoName).ConfigureAwait(false);
            var languageNames = languages.Select(l => l.Name).ToList();

            var commitRequest = new ApiOptions { PageSize = 1 };
            var lastCommit = await _githubClient.Repository.Commit.GetAll(ownerName, repoName, commitRequest).ConfigureAwait(false);

            var prRequest = new PullRequestRequest { State = ItemStateFilter.All };
            var pullRequests = await _githubClient.PullRequest.GetAllForRepository(ownerName, repoName, prRequest).ConfigureAwait(false); 

            results.Add(new RepositoryData
            {
                Name = repo.Name,
                Languages = languageNames,
                LastCommitDate = lastCommit.FirstOrDefault()?.Commit.Committer.Date.DateTime ?? repo.UpdatedAt.DateTime,
                StargazersCount = repo.StargazersCount,
                PullRequestsCount = pullRequests.Count,
                HomepageUrl = repo.Homepage,
            });
        }
        return results;
    }

    public async Task<IEnumerable<Repository>> SearchRepositories(string repoName, string language, string username)
    {
        var queryBuilder = new List<string>();

        if (!string.IsNullOrEmpty(repoName))
            queryBuilder.Add(repoName);

        if (!string.IsNullOrEmpty(language))
            queryBuilder.Add($"language:{language}");

        if (!string.IsNullOrEmpty(username))
            queryBuilder.Add($"user:{username}");

        var fullQuery = string.Join(" ", queryBuilder);

        if (string.IsNullOrEmpty(fullQuery))
        {
            return Enumerable.Empty<Repository>();
        }

        var request = new SearchRepositoriesRequest(fullQuery)
        {
            SortField = RepoSearchSort.Stars,
            Order = SortDirection.Descending
        };

        var result = await _githubClient.Search.SearchRepo(request).ConfigureAwait(false); 

        return result.Items.ToList();
    }
}