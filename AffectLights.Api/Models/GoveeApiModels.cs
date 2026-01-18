namespace AffectLights.Api.Models;

public class GoveeControlRequest
{
    public string RequestId { get; set; } = Guid.NewGuid().ToString();
    public GoveePayload Payload { get; set; } = new();
}

public class GoveePayload
{
    public string Sku { get; set; } = string.Empty;
    public string Device { get; set; } = string.Empty;
    public GoveeCapability Capability { get; set; } = new();
}

public class GoveeCapability
{
    public string Type { get; set; } = string.Empty;
    public string Instance { get; set; } = string.Empty;
    public object Value { get; set; } = new();
}

public class GoveeApiResponse
{
    public int Code { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Payload { get; set; }
}
