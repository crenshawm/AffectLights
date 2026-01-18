using AffectLights.Api.Models;  

namespace AffectLights.Api.Models
{
    public class Scene
    {
        public string Name { get; set; } = string.Empty;
        public Emotion Emotion { get; set; }
        public RgbColor Color { get; set; } = new RgbColor();
        public int Brightness { get; set; }
        public string Effect { get; set; } = "static";
    }
}
