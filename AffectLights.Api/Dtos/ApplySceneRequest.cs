using AffectLights.Api.Models;  

namespace AffectLights.Api.Dtos
{
    public class ApplySceneRequest
    {
        // Option 1: choose by emotion
        public Emotion? Emotion { get; set; }

        // Option 2: choose by scene name
        public string? SceneName { get; set; } 
    }
}
