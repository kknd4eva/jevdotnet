using System.Text.Json.Serialization;

namespace jevdotnet.Models.Jev;

/// <summary>Response body from POST /v1/systemone.</summary>
public class JevResponse
{
    [JsonPropertyName("model")]
    public string? Model { get; init; }

    [JsonPropertyName("answers")]
    public Dictionary<string, JevAnswer> Answers { get; init; } = [];

    [JsonPropertyName("usage")]
    public JevUsage? Usage { get; init; }

    [JsonPropertyName("request_id")]
    public string? RequestId { get; init; }

    [JsonPropertyName("evaluation_time_ms")]
    public double EvaluationTimeMs { get; init; }
}

public class JevAnswer
{
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("score")]
    public double Score { get; init; }

    [JsonPropertyName("legend")]
    public Dictionary<string, string> Legend { get; init; } = [];

    [JsonPropertyName("confidence")]
    public double Confidence { get; init; }

    [JsonPropertyName("probabilities")]
    public Dictionary<string, double> Probabilities { get; init; } = [];
}

public class JevUsage
{
    [JsonPropertyName("input_tokens")]
    public int InputTokens { get; init; }

    [JsonPropertyName("output_tokens")]
    public int OutputTokens { get; init; }
}
