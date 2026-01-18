using AffectLights.Api.Models;

namespace AffectLights.Api.Services
{
    public interface IEmotionAnalyzer
    {
        Task<Emotion> AnalyzeAsync(string text);
    }
}
