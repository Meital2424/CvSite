using GitHub.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Octokit;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

builder.Services.Configure<GitHubOptions>(
    builder.Configuration.GetSection(GitHubOptions.GitHub));

// 3. רישום GitHubClient (Singleton)
builder.Services.AddSingleton<IGitHubClient>(sp =>
{
    var options = sp.GetRequiredService<IOptions<GitHubOptions>>().Value;
    var productHeader = new ProductHeaderValue("CvSite-Portfolio-Api");

    // 1. יצירת ה-Client באמצעות ProductHeaderValue בלבד
    var client = new GitHubClient(productHeader);

    // 2. הגדרת Credentials דרך המאפיין (Property), אם יש טוקן
    if (!string.IsNullOrEmpty(options.PersonalAccessToken))
    {
        // יצירת אובייקט Credentials
        var credentials = new Credentials(options.PersonalAccessToken);

        // הגדרת Credentials ל-Client שנוצר
        client.Credentials = credentials;
    }

    return client;
});

// 4. רישום IGitHubService המקורי (טרם הוספת Cache Decorator)
builder.Services.AddScoped<IGitHubService, GitHubService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

//var summaries = new[]
//{
//    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//};

//app.MapGet("/weatherforecast", () =>
//{
//    var forecast =  Enumerable.Range(1, 5).Select(index =>
//        new WeatherForecast
//        (
//            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//            Random.Shared.Next(-20, 55),
//            summaries[Random.Shared.Next(summaries.Length)]
//        ))
//        .ToArray();
//    return forecast;
//})
//.WithName("GetWeatherForecast");

//app.Run();

//record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
//{
//    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
//}
