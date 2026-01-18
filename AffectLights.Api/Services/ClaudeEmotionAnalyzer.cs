using AffectLights.Api.Models;
using Anthropic.SDK;
using Anthropic.SDK.Messaging;

namespace AffectLights.Api.Services;

public class ClaudeEmotionAnalyzer : IEmotionAnalyzer
{
    private readonly AnthropicClient _client;
    private readonly string _model;
    private readonly ILogger<ClaudeEmotionAnalyzer> _logger;

    public ClaudeEmotionAnalyzer(ClaudeConfig config, ILogger<ClaudeEmotionAnalyzer> logger)
    {
        _client = new AnthropicClient(config.ApiKey);
        _model = config.Model;
        _logger = logger;
    }

    public async Task<Emotion> AnalyzeAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Emotion.Calm;
        }

        try
        {
            _logger.LogInformation("Analyzing emotion with Claude for text: {Text}", text);

            var systemPrompt = @"You are an emotion analyzer. Analyze the given text and respond with ONLY ONE of these emotions: Calm, Stressed, Upbeat, Low.

- Calm: neutral, peaceful, or balanced emotional state
- Stressed: anxious, overwhelmed, tense, or irritated
- Upbeat: happy, excited, energetic, or motivated
- Low: tired, sad, depleted, or down

Respond with only the emotion name, nothing else.";

            var messages = new List<Message>
            {
                new Message(RoleType.User, text)
            };

            var parameters = new MessageParameters
            {
                Messages = messages,
                Model = _model,
                MaxTokens = 10,
                Stream = false,
                Temperature = 0.3m,
                System = new List<SystemMessage> { new SystemMessage(systemPrompt) }
            };

            var response = await _client.Messages.GetClaudeMessageAsync(parameters);
            var responseText = ((TextContent)response.Content[0]).Text.Trim();

            _logger.LogInformation("Claude response: {Response}", responseText);

            // Parse the response to an Emotion enum
            if (Enum.TryParse<Emotion>(responseText, ignoreCase: true, out var emotion))
            {
                return emotion;
            }

            _logger.LogWarning("Could not parse emotion from Claude response: {Response}. Defaulting to Calm.", responseText);
            return Emotion.Calm;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing emotion with Claude. Defaulting to Calm.");
            return Emotion.Calm;
        }
    }
}
