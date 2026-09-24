namespace jevdotnet.Configuration;

public class JevConfiguration
{
    public required string ApiKey { get; init; }

    public string BaseUrl { get; init; } = "https://api.typesafe.ai/";

    public string Model { get; init; } = "jev-latest";
}
