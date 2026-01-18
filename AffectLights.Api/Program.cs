using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using Swashbuckle.AspNetCore.Swagger;
using AffectLights.Api.Services;
using AffectLights.Api.Models;
using AffectLights.Api.Dtos;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Govee Configuration
var goveeConfig = builder.Configuration.GetSection("Govee").Get<GoveeConfig>() ?? new GoveeConfig();
builder.Services.AddSingleton(goveeConfig);
builder.Services.AddHttpClient<ILightController, GoveeLightController>();

// AI Configuration
var openAIConfig = builder.Configuration.GetSection("OpenAI").Get<OpenAIConfig>() ?? new OpenAIConfig();
var claudeConfig = builder.Configuration.GetSection("Claude").Get<ClaudeConfig>() ?? new ClaudeConfig();
builder.Services.AddSingleton(openAIConfig);
builder.Services.AddSingleton(claudeConfig);

// Emotion Analyzer - Choose which implementation to use
var aiProvider = builder.Configuration.GetValue<string>("AI:Provider") ?? "OpenAI";
if (aiProvider.Equals("Claude", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddSingleton<IEmotionAnalyzer, ClaudeEmotionAnalyzer>();
}
else if (aiProvider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddSingleton<IEmotionAnalyzer, OpenAIEmotionAnalyzer>();
}
else
{
    // Default to rule-based analyzer if no AI provider configured
    builder.Services.AddSingleton<IEmotionAnalyzer, RuleBasedEmotionAnalyzer>();
}

// Custom services
builder.Services.AddSingleton<ISceneRepository, InMemorySceneRepository>();
// To use fake lights for testing, uncomment the line below and comment out the AddHttpClient line above
// builder.Services.AddSingleton<ILightController, FakeLightController>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Temporary test endpoint
app.MapGet("/ping", () => Results.Ok(new { message = "AffectLights API is alive!" }));

app.MapGet("/status", () =>
{
    var status = new
    {
        app = "AffectLights.Api",
        time = DateTime.UtcNow,
        version = "0.1.0"
    };

    return Results.Ok(status);
});

app.MapGet("/scenes", (ISceneRepository repo) =>
{
    var scenes = repo.GetAll();
    return Results.Ok(scenes);
});

app.MapPost("/lights/apply-scene", async (ApplySceneRequest request, ISceneRepository repo, ILightController lightController) =>
    {
        // 1. Determine how the user is choosing the scene

        Scene? scene = null;

        if (request.Emotion.HasValue)
        {
            scene = repo.GetByEmotion(request.Emotion.Value);
        }
        else if (!string.IsNullOrWhiteSpace(request.SceneName))
        {
            scene = repo.GetByName(request.SceneName);
        }

        // 2. Handle scene not found or invalid input
        if (scene is null)
        {
            return Results.NotFound(new { message = "Scene not found." });
        }

        await lightController.ApplyScene(scene);

        return Results.Ok(new
        {
            message = $"Scene '{scene.Name}' was sent to the light controller.",
            applied = scene
        });
    });

app.MapPost("/analyze-emotion", async (AnalyzeEmotionRequest request, IEmotionAnalyzer analyzer, ISceneRepository repo, ILightController lightController) =>
{
    if (string.IsNullOrWhiteSpace(request.Text))
    {
        return Results.BadRequest(new { message = "Text is required for emotion analysis." });
    }

    var emotion = await analyzer.AnalyzeAsync(request.Text);

    var response = new
    {
        text = request.Text,
        detectedEmotion = emotion,
        message = $"Detected emotion: {emotion}"
    };

    // Optionally apply the detected emotion to lights
    if (request.ApplyToLights)
    {
        var scene = repo.GetByEmotion(emotion);
        if (scene is not null)
        {
            await lightController.ApplyScene(scene);
            return Results.Ok(new
            {
                text = request.Text,
                detectedEmotion = emotion,
                appliedScene = scene,
                message = $"Detected emotion: {emotion}. Applied scene: {scene.Name}"
            });
        }
    }

    return Results.Ok(response);
});

app.Run();
