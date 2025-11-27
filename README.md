**CV Site Backend API**

*General Overview*
This project is a robust and performance-oriented ASP.NET Core Web API service designed to function as the backend for a personal developer portfolio website. 
Its core functionality involves connecting to the GitHub API via the Octokit.net library, fetching detailed information on personal repositories, and providing a reliable search capability.
The architecture emphasizes efficiency, maintainability, and resilience against external API limitations, particularly Rate Limiting.

*Project Structure and DI*
The project is modularized for improved Separation of Concerns:
1. GitHub.Service: Contains the pure implementation of IGitHubService (data fetching) and the CachedGitHubService (the Decorator class).
2. CvSite.Api: The API entry point, handling Controllers, Configuration, and Dependency Injection (DI) setup.
   
*Dependency Injection*
The IGitHubService is registered as a scoped service, with the CachedGitHubService wrapping the concrete GitHubService implementation. 
This wrapping was achieved using a Manual Factory Registration in Program.cs to ensure correct construction order and reliability of the Decorator pattern.

*Configuration and Authentication*
The project uses the Options Pattern (IOptionsSnapshot<GitHubOptions>) to load authentication credentials:
PersonalAccessToken: Requires the repo scope permission in GitHub.
Security: The token is stored securely in secrets.json during development and loaded via the Configuration provider.

*API Usage*
The API exposes two main endpoints:
1. **GET** [/api/GitHub/portfolio]
Fetches the user's personal portfolio data.
Data Source: GitHub API (Cached for 10 minutes).
Data Points: Repo Name, List of Languages, Last Commit Date, Stargazer Count, Pull Request Count.
2. **GET** [/api/GitHub/search]
Performs a global search across public GitHub repositories.
Optional Parameters: repoName, language, username.
Data Source: GitHub Search API (Not Cached).

Getting StartedClone the Repository: 
git clone https://github.com/Meital2424/CvSite.git
cd "CV Site"

Configure Secrets: Create the secrets.json file for the CvSite.Api project and add your credentials:
{
  "GitHubOptions": {
    "Username": "YOUR_GITHUB_USERNAME",
    "PersonalAccessToken": "YOUR_PERSONAL_ACCESS_TOKEN"
  }
}
Run the Project: dotnet run --project CvSite.Api
Access: The API endpoints can be tested via Swagger (e.g., [http://localhost:5255/swagger]).
