# AffectLights

A .NET-based smart lighting control system that uses AI to analyze emotional states from text and automatically adjusts Govee lights to match your mood. Built with modern ASP.NET Core patterns including dependency injection, clean architecture, and secure configuration management.

## Features

- **AI-Powered Emotion Analysis** - Detects emotions from text using OpenAI or Claude
- **Emotion-Based Lighting Control** - Automatically maps emotional states to custom light scenes
- **Govee API Integration** - Controls real Govee smart lights via their cloud API
- **Multiple AI Providers** - Switch between OpenAI GPT-4, Claude Sonnet, or rule-based analysis
- **Secure Secret Management** - Uses .NET User Secrets for API key storage
- **Clean Architecture** - Interface-based design with dependency injection
- **Testable Design** - Includes fake implementations for testing without hardware
- **Swagger Documentation** - Interactive API documentation built-in

## Tech Stack

- **.NET 9** - Modern C# with minimal APIs
- **ASP.NET Core** - Web API framework
- **OpenAI SDK** - GPT-4 emotion analysis
- **Anthropic SDK** - Claude Sonnet emotion analysis
- **Swagger/OpenAPI** - API documentation
- **User Secrets** - Secure local configuration
- **Dependency Injection** - Built-in DI container

## Emotional Scenes

The system includes four predefined emotional states:

| Emotion | Color | Brightness | Effect |
|---------|-------|------------|--------|
| **Calm** | Blue (80, 120, 255) | 40% | Fade |
| **Stressed** | Red (200, 60, 60) | 65% | Pulse |
| **Upbeat** | Teal (0, 200, 160) | 75% | Shimmer |
| **Low** | Purple (140, 80, 200) | 30% | Slow Fade |

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- Govee API Key ([apply here](https://developer.govee.com/))
- Govee smart lights
- **Optional:** OpenAI API Key ([get one here](https://platform.openai.com/api-keys))
- **Optional:** Claude API Key ([get one here](https://console.anthropic.com/settings/keys))

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/AffectLights.git
   cd AffectLights
   ```

2. **Get your Govee device information**

   Use your API key to fetch your devices:
   ```powershell
   $headers = @{
       "Content-Type" = "application/json"
       "Govee-API-Key" = "YOUR_API_KEY"
   }
   Invoke-RestMethod -Uri "https://openapi.api.govee.com/router/api/v1/user/devices" -Method Get -Headers $headers
   ```

3. **Configure your device**

   Edit `AffectLights.Api/appsettings.json` with your device ID and SKU:
   ```json
   {
     "Govee": {
       "ApiKey": "",
       "DeviceId": "YOUR_DEVICE_ID",
       "Sku": "YOUR_DEVICE_SKU"
     }
   }
   ```

4. **Store your secrets securely**
   ```bash
   cd AffectLights.Api

   # Govee configuration
   dotnet user-secrets set "Govee:ApiKey" "YOUR_GOVEE_API_KEY"
   dotnet user-secrets set "Govee:DeviceId" "YOUR_DEVICE_ID"
   dotnet user-secrets set "Govee:Sku" "YOUR_DEVICE_SKU"

   # Optional: For AI emotion analysis (choose one or both)
   dotnet user-secrets set "OpenAI:ApiKey" "YOUR_OPENAI_API_KEY"
   dotnet user-secrets set "Claude:ApiKey" "YOUR_CLAUDE_API_KEY"
   ```

5. **Configure AI provider (optional)**

   Edit `AffectLights.Api/appsettings.json` to choose your AI provider:
   ```json
   {
     "AI": {
       "Provider": "OpenAI"  // Options: "OpenAI", "Claude", or "RuleBased"
     }
   }
   ```

6. **Run the application**
   ```bash
   dotnet run
   ```

7. **Open Swagger UI**

   Navigate to `https://localhost:7020/swagger` to interact with the API.

## Usage

### Analyze Text and Control Lights with AI

```bash
POST https://localhost:7020/analyze-emotion
Content-Type: application/json

{
  "text": "I'm feeling really stressed out and overwhelmed with work",
  "applyToLights": true
}
```

Response:
```json
{
  "text": "I'm feeling really stressed out and overwhelmed with work",
  "detectedEmotion": "Stressed",
  "appliedScene": {
    "name": "StressedRed",
    "emotion": "Stressed",
    "color": { "r": 200, "g": 60, "b": 60 },
    "brightness": 65
  },
  "message": "Detected emotion: Stressed. Applied scene: StressedRed"
}
```

### Apply a Scene by Emotion

```bash
POST https://localhost:7020/lights/apply-scene
Content-Type: application/json

{
  "emotion": "Calm"
}
```

### Apply a Scene by Name

```bash
POST https://localhost:7020/lights/apply-scene
Content-Type: application/json

{
  "sceneName": "CalmBlue"
}
```

### Get Available Scenes

```bash
GET https://localhost:7020/scenes
```

## Architecture

### Project Structure

```
AffectLights.Api/
├── Models/           # Domain models and DTOs
│   ├── Emotion.cs
│   ├── Scene.cs
│   ├── RgbColor.cs
│   ├── GoveeConfig.cs
│   ├── GoveeApiModels.cs
│   ├── OpenAIConfig.cs
│   └── ClaudeConfig.cs
├── Services/         # Business logic and integrations
│   ├── ISceneRepository.cs
│   ├── InMemorySceneRepository.cs
│   ├── ILightController.cs
│   ├── GoveeLightController.cs
│   ├── FakeLightController.cs
│   ├── IEmotionAnalyzer.cs
│   ├── OpenAIEmotionAnalyzer.cs
│   ├── ClaudeEmotionAnalyzer.cs
│   └── RuleBasedEmotionAnalyzer.cs
├── Dtos/             # Data transfer objects
│   ├── ApplySceneRequest.cs
│   └── AnalyzeEmotionRequest.cs
└── Program.cs        # Application entry point
```

### Key Design Patterns

- **Repository Pattern** - Scene data access abstraction
- **Strategy Pattern** - Swappable light controller implementations
- **Dependency Injection** - Loose coupling and testability
- **Interface Segregation** - Clean contracts for services

### Emotion Analyzer Implementations

The system includes three implementations of `IEmotionAnalyzer`:

1. **OpenAIEmotionAnalyzer** - Uses GPT-4 to analyze emotions from text
2. **ClaudeEmotionAnalyzer** - Uses Claude Sonnet 4.5 to analyze emotions from text
3. **RuleBasedEmotionAnalyzer** - Simple keyword matching (no API required)

Switch between them in `appsettings.json`:

```json
{
  "AI": {
    "Provider": "OpenAI"  // "OpenAI", "Claude", or "RuleBased"
  }
}
```

### Light Controller Implementations

The system includes two implementations of `ILightController`:

1. **GoveeLightController** - Communicates with real Govee hardware via HTTP API
2. **FakeLightController** - Logs to console for testing without hardware

Switch between them in `Program.cs`:

```csharp
// Use real Govee lights
builder.Services.AddHttpClient<ILightController, GoveeLightController>();

// Or use fake lights for testing
// builder.Services.AddSingleton<ILightController, FakeLightController>();
```

## Configuration

### User Secrets (Development)

Recommended for local development:
```bash
# Govee configuration (required)
dotnet user-secrets set "Govee:ApiKey" "your-govee-key"
dotnet user-secrets set "Govee:DeviceId" "your-device-id"
dotnet user-secrets set "Govee:Sku" "your-device-sku"

# AI providers (optional - choose one or both)
dotnet user-secrets set "OpenAI:ApiKey" "your-openai-key"
dotnet user-secrets set "Claude:ApiKey" "your-claude-key"
```

### Environment Variables (Production)

For deployed environments:
```bash
# Windows PowerShell
$env:Govee__ApiKey="your-govee-key"
$env:Govee__DeviceId="your-device-id"
$env:Govee__Sku="your-device-sku"
$env:OpenAI__ApiKey="your-openai-key"
$env:Claude__ApiKey="your-claude-key"

# Linux/Mac
export Govee__ApiKey="your-govee-key"
export Govee__DeviceId="your-device-id"
export Govee__Sku="your-device-sku"
export OpenAI__ApiKey="your-openai-key"
export Claude__ApiKey="your-claude-key"
```

Note: Use double underscores (`__`) for nested configuration.

## API Rate Limits

### Govee API
The Govee API has a limit of **10,000 requests per account per day**. Each scene application makes 3 API calls:
1. Turn on the light
2. Set the color
3. Set the brightness

This allows approximately **3,300 scene applications per day**.

### AI APIs
- **OpenAI GPT-4**: Pay-as-you-go (~$0.03 per 1K input tokens, ~$0.06 per 1K output tokens)
- **Claude Sonnet 4.5**: Pay-as-you-go (~$3 per 1M input tokens, ~$15 per 1M output tokens)

For emotion analysis with short responses, expect pennies per request.

## Future Enhancements

- [ ] Voice input for emotion analysis
- [ ] Multiple device support
- [ ] Scheduled scene changes (circadian rhythm lighting)
- [ ] Weather-based adaptive lighting
- [ ] RESTful scene management (create/update/delete)
- [ ] WebSocket support for real-time updates
- [ ] Historical emotion tracking and visualization

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- [Govee Developer API](https://developer.govee.com/) for smart light control
- [OpenAI](https://openai.com/) for GPT-4 emotion analysis
- [Anthropic](https://www.anthropic.com/) for Claude AI emotion analysis
- Built with [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet)

## Author

**Mark Crenshaw**
- GitHub: [@crenshawm](https://github.com/crenshawm)
- LinkedIn: [Mark Crenshaw](https://linkedin.com/in/crenshawm)

---

*An AI-powered emotion-responsive lighting system exploring the intersection of natural language processing, IoT hardware control, and ambient computing.*
