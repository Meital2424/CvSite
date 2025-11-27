using Octokit;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;

namespace GitHub.Service;
// תפקיד: היישום הטהור של הלוגיקה העסקית.
public class GitHubService : IGitHubService
{
    private readonly IGitHubClient _githubClient;
    private readonly GitHubOptions _options;

    // הקונסטרקטור מקבל את ה-Client ואת האפשרויות בהזרקה
    public GitHubService(IGitHubClient githubClient, IOptions<GitHubOptions> options)
    {
        _githubClient = githubClient;
        // גישה לערכים דרך ה-Value property של IOptions
        _options = options.Value;
    }

    public async Task<IEnumerable<RepositoryData>> GetPortfolio()
    {
        // 1. שלוף רפוזיטורים אישיים
        var repos = await _githubClient.Repository.GetAllForCurrent();

        var results = new List<RepositoryData>();

        foreach (var repo in repos)
        {
            // 2. שלוף שפות
            var languages = await _githubClient.Repository.GetAllLanguages(_options.Username, repo.Name);
            var languageNames = languages.Select(l => l.Name).ToList();

            // 3. שלוף קומיט אחרון (רק 1)
            var commitRequest = new ApiOptions { PageSize = 1 };
            var lastCommit = await _githubClient.Repository.Commit.GetAll(_options.Username, repo.Name, commitRequest);

            // 4. שלוף Pull Requests (פתוחים וסגורים)
            var prRequest = new PullRequestRequest { State = ItemStateFilter.All };
            var pullRequests = await _githubClient.PullRequest.GetAllForRepository(_options.Username, repo.Name, prRequest);

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

        var result = await _githubClient.Search.SearchRepo(request);

        return result.Items.ToList();
    }
}