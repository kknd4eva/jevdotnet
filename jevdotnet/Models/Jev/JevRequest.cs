using System.Text.Json.Serialization;

namespace jevdotnet.Models.Jev;

/// <summary>Request body for POST /v1/systemone.</summary>
public class JevRequest
{
    [JsonPropertyName("state")]
    public required string State { get; init; }

    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("questions")]
    public required Dictionary<string, JevQuestion> Questions { get; init; }
}

public class JevQuestion
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("instructions")]
    public required string Instructions { get; init; }

    [JsonPropertyName("criteria")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<string>? Criteria { get; init; }
}
