namespace AffectLights.Api.Dtos;

public class AnalyzeEmotionRequest
{
    public string Text { get; set; } = string.Empty;
    public bool ApplyToLights { get; set; } = false;
}
