using AffectLights.Api.Models;
using System.Collections.Generic;
using System.Linq;


namespace AffectLights.Api.Services
{
    public class InMemorySceneRepository : ISceneRepository
    {
        private readonly List<Scene> _scenes;

        public InMemorySceneRepository()
        {             _scenes = new List<Scene>
            {
                new Scene
                {
                    Name = "CalmBlue",
                    Emotion = Emotion.Calm,
                    Color = new RgbColor { R = 80, G = 120, B = 255 },
                    Brightness = 40,
                    Effect = "fade"
                },
                new Scene
                {
                    Name = "StressedRed",
                    Emotion = Emotion.Stressed,
                    Color = new RgbColor { R = 200, G = 60, B = 60 },
                    Brightness = 65,
                    Effect = "pulse"
                },
                new Scene
                {
                    Name = "UpbeatTeal",
                    Emotion = Emotion.Upbeat,
                    Color = new RgbColor {R = 0, G = 200, B = 160 },
                    Brightness = 75,
                    Effect = "shimmer"
                },
                new Scene
                {
                    Name = "LowPurple",
                    Emotion = Emotion.Low,
                    Color = new RgbColor {R = 140, G = 80, B = 200 },
                    Brightness = 30,
                    Effect = "slow-fade"
                }
            };
        }
        public IReadOnlyList<Scene> GetAll() => _scenes;

        public Scene? GetByEmotion(Emotion emotion) => _scenes.FirstOrDefault(s => s.Emotion == emotion);

        public Scene? GetByName(string name) => _scenes.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}
