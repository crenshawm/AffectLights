using AffectLights.Api.Models;

namespace AffectLights.Api.Services
{
    public interface ILightController
    {
        Task ApplyScene(Scene scene);
    }
}
