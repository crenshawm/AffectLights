using System.Collections.Generic;
using AffectLights.Api.Models;

namespace AffectLights.Api.Services
{
    public interface ISceneRepository
    {
        IReadOnlyList<Scene> GetAll();
        Scene? GetByEmotion(Emotion emotion);
        Scene? GetByName(string name);
    }
}
