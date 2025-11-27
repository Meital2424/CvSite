using Microsoft.AspNetCore.Mvc;
using GitHub.Service;
using Octokit; 

namespace CvSite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GitHubController : ControllerBase
{
    private readonly IGitHubService _gitHubService;
    public GitHubController(IGitHubService gitHubService)
    {
        _gitHubService = gitHubService;
    }

    [HttpGet("portfolio")]
    [ProducesResponseType(typeof(IEnumerable<RepositoryData>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPortfolio()
    {
        var portfolio = await _gitHubService.GetPortfolio();

        if (portfolio == null || !portfolio.Any())
        {
            return NotFound("Portfolio data could not be retrieved. Check username and access token configuration.");
        }

        return Ok(portfolio);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<Repository>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchRepositories(
        [FromQuery] string? name,
        [FromQuery] string? lang,
        [FromQuery] string? user)
    {
        var results = await _gitHubService.SearchRepositories(name ?? string.Empty, lang ?? string.Empty, user ?? string.Empty);
        return Ok(results);
    }
}