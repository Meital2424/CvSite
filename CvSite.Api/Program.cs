using GitHub.Service;
using Microsoft.Extensions.Options;
using Octokit;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddLogging(); 

builder.Services.Configure<GitHubOptions>(
    builder.Configuration.GetSection(GitHubOptions.GitHub));

builder.Services.AddSingleton<IGitHubClient>(sp =>
{
    var token = builder.Configuration.GetValue<string>("GitHubOptions:PersonalAccessToken");
    var username = builder.Configuration.GetValue<string>("GitHubOptions:Username");

    var logger = sp.GetRequiredService<ILogger<GitHubClient>>();

    if (string.IsNullOrEmpty(token))
    {
        logger.LogError("GitHub Access Token is MISSING or EMPTY!");
    }
    else
    {
        logger.LogInformation($"GitHub Token Loaded. Length: {token.Length}. Starting Client...");
    }

    var productHeader = new ProductHeaderValue("CvSite-Portfolio-Api");

    var client = new GitHubClient(productHeader)
    {
        Credentials = new Credentials(token)
    };

    return client;
});

builder.Services.AddScoped<IGitHubService>(sp =>
{
    var githubClient = sp.GetRequiredService<IGitHubClient>();
    var cache = sp.GetRequiredService<IMemoryCache>();
    var optionsSnapshot = sp.GetRequiredService<IOptionsSnapshot<GitHubOptions>>();
    var directUsername = builder.Configuration.GetValue<string>("GitHubOptions:Username");

    if (string.IsNullOrEmpty(directUsername))
    {
        throw new InvalidOperationException("GitHub Username is missing from configuration.");
    }
    optionsSnapshot.Value.Username = directUsername;

    var decoratedService = new GitHubService(githubClient, optionsSnapshot);

    return new CachedGitHubService(decoratedService, cache, optionsSnapshot);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
