using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AffectLights.Api.Models;

namespace AffectLights.Api.Services;

public class GoveeLightController : ILightController
{
    private readonly HttpClient _httpClient;
    private readonly GoveeConfig _config;
    private readonly ILogger<GoveeLightController> _logger;
    private const string BaseUrl = "https://openapi.api.govee.com";

    public GoveeLightController(HttpClient httpClient, GoveeConfig config, ILogger<GoveeLightController> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;

        _httpClient.BaseAddress = new Uri(BaseUrl);
        _httpClient.DefaultRequestHeaders.Add("Govee-API-Key", _config.ApiKey);
    }

    public async Task ApplyScene(Scene scene)
    {
        _logger.LogInformation("Applying scene '{SceneName}' to Govee device", scene.Name);

        try
        {
            // Step 1: Turn the light on
            await TurnOnAsync();

            // Step 2: Set the color
            await SetColorAsync(scene.Color);

            // Step 3: Set the brightness
            await SetBrightnessAsync(scene.Brightness);

            _logger.LogInformation("Successfully applied scene '{SceneName}'", scene.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply scene '{SceneName}'", scene.Name);
            throw;
        }
    }

    private async Task TurnOnAsync()
    {
        var request = new GoveeControlRequest
        {
            Payload = new GoveePayload
            {
                Sku = _config.Sku,
                Device = _config.DeviceId,
                Capability = new GoveeCapability
                {
                    Type = "devices.capabilities.on_off",
                    Instance = "powerSwitch",
                    Value = 1
                }
            }
        };

        await SendControlRequestAsync(request, "turn on");
    }

    private async Task SetColorAsync(RgbColor color)
    {
        // Convert RGB (0-255, 0-255, 0-255) to single integer (0-16777215)
        int rgbValue = (color.R << 16) | (color.G << 8) | color.B;

        var request = new GoveeControlRequest
        {
            Payload = new GoveePayload
            {
                Sku = _config.Sku,
                Device = _config.DeviceId,
                Capability = new GoveeCapability
                {
                    Type = "devices.capabilities.color_setting",
                    Instance = "colorRgb",
                    Value = rgbValue
                }
            }
        };

        await SendControlRequestAsync(request, $"set color to RGB({color.R},{color.G},{color.B})");
    }

    private async Task SetBrightnessAsync(int brightness)
    {
        var request = new GoveeControlRequest
        {
            Payload = new GoveePayload
            {
                Sku = _config.Sku,
                Device = _config.DeviceId,
                Capability = new GoveeCapability
                {
                    Type = "devices.capabilities.range",
                    Instance = "brightness",
                    Value = brightness
                }
            }
        };

        await SendControlRequestAsync(request, $"set brightness to {brightness}%");
    }

    private async Task SendControlRequestAsync(GoveeControlRequest request, string operationDescription)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
        };

        var json = JsonSerializer.Serialize(request, options);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _logger.LogInformation("Sending request to {Operation}: {Json}", operationDescription, json);

        var response = await _httpClient.PostAsync("/router/api/v1/device/control", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        _logger.LogInformation("Response Status: {StatusCode}, Body: {Response}", response.StatusCode, responseBody);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to {Operation}. Status: {StatusCode}, Response: {Response}",
                operationDescription, response.StatusCode, responseBody);
            throw new HttpRequestException($"Govee API error: {response.StatusCode} - {responseBody}");
        }

        var apiResponse = JsonSerializer.Deserialize<GoveeApiResponse>(responseBody);

        if (apiResponse?.Code != 200)
        {
            _logger.LogWarning("Govee API returned non-200 code for {Operation}: {Code} - {Message}",
                operationDescription, apiResponse?.Code, apiResponse?.Message);
        }
        else
        {
            _logger.LogDebug("Successfully completed {Operation}", operationDescription);
        }
    }
}
