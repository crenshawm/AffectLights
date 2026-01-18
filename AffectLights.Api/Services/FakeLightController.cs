using System;
using AffectLights.Api.Models;

namespace AffectLights.Api.Services
{
    public class FakeLightController : ILightController
    {
        public Task ApplyScene(Scene scene)
        {
            if (scene is null)
            {
                Console.WriteLine("[FakeLightController] Scene is null. No scene provided to apply.");
                return Task.CompletedTask;
            }

            Console.WriteLine($"Applying Scene: {scene.Name}");
            Console.WriteLine($" - Color: R={scene.Color.R}, G={scene.Color.G}, B={scene.Color.B}");
            Console.WriteLine($" - Emotion: {scene.Emotion}");
            Console.WriteLine($" - Brightness: {scene.Brightness}%");
            Console.WriteLine($" - Effect: {scene.Effect}");

            return Task.CompletedTask;
        }
    }
}
