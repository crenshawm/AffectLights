using AffectLights.Api.Models;
using OpenAI.Chat;

namespace AffectLights.Api.Services;

public class OpenAIEmotionAnalyzer : IEmotionAnalyzer
{
    private readonly ChatClient _chatClient;
    private readonly ILogger<OpenAIEmotionAnalyzer> _logger;

    public OpenAIEmotionAnalyzer(OpenAIConfig config, ILogger<OpenAIEmotionAnalyzer> logger)
    {
        _chatClient = new ChatClient(config.Model, config.ApiKey);
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
            _logger.LogInformation("Analyzing emotion with OpenAI for text: {Text}", text);

            var systemPrompt = @"You are an emotion analyzer. Analyze the given text and respond with ONLY ONE of these emotions: Calm, Stressed, Upbeat, Low.

- Calm: neutral, peaceful, or balanced emotional state
- Stressed: anxious, overwhelmed, tense, or irritated
- Upbeat: happy, excited, energetic, or motivated
- Low: tired, sad, depleted, or down

Respond with only the emotion name, nothing else.";

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(systemPrompt),
                new UserChatMessage(text)
            };

            var completion = await _chatClient.CompleteChatAsync(messages);
            var response = completion.Value.Content[0].Text.Trim();

            _logger.LogInformation("OpenAI response: {Response}", response);

            // Parse the response to an Emotion enum
            if (Enum.TryParse<Emotion>(response, ignoreCase: true, out var emotion))
            {
                return emotion;
            }

            _logger.LogWarning("Could not parse emotion from OpenAI response: {Response}. Defaulting to Calm.", response);
            return Emotion.Calm;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing emotion with OpenAI. Defaulting to Calm.");
            return Emotion.Calm;
        }
    }
}
