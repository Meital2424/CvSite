using Octokit;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace GitHub.Service;
public class CachedGitHubService : IGitHubService
{
    private readonly IGitHubService _decorated;
    private readonly IMemoryCache _cache;
    private readonly GitHubOptions _options;
    private string CacheKey => $"Portfolio_{_options.Username}";

    public string Username => _decorated.Username;

    public CachedGitHubService(
        IGitHubService decorated,
        IMemoryCache cache,
        IOptionsSnapshot<GitHubOptions> options)
    {
        _decorated = decorated;
        _cache = cache;
        _options = options.Value;
    }

    public async Task<IEnumerable<RepositoryData>> GetPortfolio()
    {
        var portfolio = await _cache.GetOrCreateAsync(
            CacheKey,
            cacheEntry =>
            {
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                return _decorated.GetPortfolio();
            });

        return portfolio ?? Enumerable.Empty<RepositoryData>();
    }

    public Task<IEnumerable<Repository>> SearchRepositories(string repoName, string language, string username)
    {
        return _decorated.SearchRepositories(repoName, language, username);
    }
}