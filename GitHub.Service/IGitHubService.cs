using Octokit;

namespace GitHub.Service;
// תפקיד: מגדיר את החוזה (Interface) – אילו פעולות השירות הזה מבטיח לבצע (GetPortfolio, SearchRepositories).
// חשיבות: כל חלק במערכת שמבקש לעבוד עם GitHub חייב לפנות רק לממשק זה, מה שמאפשר להחליף את היישום מאחור (למשל, להחליף ל-CachedGitHubService או MockGitHubService) מבלי לשנות את ה-Controller.
public interface IGitHubService
{
    Task<IEnumerable<RepositoryData>> GetPortfolio();

    Task<IEnumerable<Repository>> SearchRepositories(string repoName, string language, string username);
}