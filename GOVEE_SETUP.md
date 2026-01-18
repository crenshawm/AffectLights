# Govee API Setup Guide

## Getting Your Govee API Credentials

### 1. Get Your API Key

1. Open the **Govee Home** app on your phone
2. Go to your profile (tap the icon in the top right)
3. Tap **About Us** → **Apply for API Key**
4. Fill out the form explaining your use case (e.g., "Personal home automation project")
5. You'll receive your API key via email (usually within 1-2 business days)

### 2. Find Your Device ID and SKU

Once you have your API key, you can get your device information using the API:

**Using curl:**
```bash
curl -X GET "https://openapi.api.govee.com/router/api/v1/user/devices" \
  -H "Content-Type: application/json" \
  -H "Govee-API-Key: YOUR_API_KEY_HERE"
```

**Using PowerShell:**
```powershell
$headers = @{
    "Content-Type" = "application/json"
    "Govee-API-Key" = "YOUR_API_KEY_HERE"
}
Invoke-RestMethod -Uri "https://openapi.api.govee.com/router/api/v1/user/devices" -Method Get -Headers $headers
```

The response will look like:
```json
{
  "code": 200,
  "message": "Success",
  "payload": {
    "devices": [
      {
        "device": "AB:CD:EF:12:34:56:78:90",
        "sku": "H6159",
        "deviceName": "Living Room Light",
        ...
      }
    ]
  }
}
```

Copy the `device` and `sku` values from your desired light.

## Configuration

### Secure API Key Storage (Recommended)

To keep your API key secure and out of source control, use .NET User Secrets:

1. **Store your API key securely:**
   ```bash
   cd AffectLights.Api
   dotnet user-secrets set "Govee:ApiKey" "your-actual-api-key-here"
   ```

2. **Configure device in appsettings.json:**

   The device ID and SKU are already configured in `appsettings.json`. You can change them if needed:
   ```json
   {
     "Govee": {
       "ApiKey": "",
       "DeviceId": "5F:F3:60:74:F4:C6:19:F8",
       "Sku": "H6008"
     }
   }
   ```

   The API key is intentionally empty here - it's loaded from user secrets at runtime.

### Alternative: Environment Variables

You can also use environment variables (useful for production deployments):

**Windows PowerShell:**
```powershell
$env:Govee__ApiKey="your-actual-api-key-here"
```

**Linux/Mac:**
```bash
export Govee__ApiKey="your-actual-api-key-here"
```

**Note:** Use double underscores (`__`) in environment variables to represent nested configuration (colon `:` in JSON).

### For Testing Only: Direct Configuration

If you're just testing and don't care about security, you can put the API key directly in `appsettings.json` - but **never commit this to git**!

## Testing the Integration

1. Start the API:
   ```bash
   cd AffectLights.Api
   dotnet run
   ```

2. Open Swagger UI at: `https://localhost:7020/swagger`

3. Try the `/lights/apply-scene` endpoint with:
   ```json
   {
     "emotion": "Calm"
   }
   ```

4. Your Govee lights should turn on, change to blue, and dim to 40% brightness!

## Available Scenes

- **Calm**: Blue (80, 120, 255) @ 40% brightness
- **Stressed**: Red (200, 60, 60) @ 65% brightness
- **Upbeat**: Teal (0, 200, 160) @ 75% brightness
- **Low**: Purple (140, 80, 200) @ 30% brightness

## Troubleshooting

### "Failed to apply scene" error
- Check that your API key is valid
- Verify your device ID and SKU are correct
- Ensure your device is online and connected to Wi-Fi
- Check rate limits (10,000 requests per day per account)

### Need to test without real lights?
In `Program.cs`, comment out line 26 and uncomment line 31:
```csharp
// builder.Services.AddHttpClient<ILightController, GoveeLightController>();
builder.Services.AddSingleton<ILightController, FakeLightController>();
```

This will log to console instead of calling the real API.

## API Rate Limits

The Govee API has a limit of **10,000 requests per account per day**. Each scene application makes 3 requests (turn on, set color, set brightness), so you can apply about 3,300 scenes per day.
