using System;
using System.Linq;
using AffectLights.Api.Models;

namespace AffectLights.Api.Services
{
    public class RuleBasedEmotionAnalyzer : IEmotionAnalyzer
    {
        public Task<Emotion> AnalyzeAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return Task.FromResult(Emotion.Calm);
            }

            text = text.ToLowerInvariant();

            // Strong negative / stress indicators
            string[] stressWords = { "overwhelmed", "anxious", "stressed", "panic", "tense", "irritated", "angry" };
            if (stressWords.Any(text.Contains))
                return Task.FromResult(Emotion.Stressed);

            // Low / depleted indicators
            string[] lowWords = { "tired", "exhausted", "drained", "sad", "down", "low", "hopeless" };
            if (lowWords.Any(text.Contains))
                return Task.FromResult(Emotion.Low);

            // Upbeat / energetic words
            string[] upbeatWords = { "happy", "excited", "motivated", "energetic", "joyful", "pumped" };
            if (upbeatWords.Any(text.Contains))
                return Task.FromResult(Emotion.Upbeat);

            // Default
            return Task.FromResult(Emotion.Calm);
        }
    }
}
