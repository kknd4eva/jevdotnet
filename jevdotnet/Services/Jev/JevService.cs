using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using jevdotnet.Configuration;
using jevdotnet.Models.Jev;

namespace jevdotnet.Services.Jev;

public interface IJevService
{
    /// <summary>
    /// Sends <paramref name="state"/> to the Jev system-one API and returns a
    /// sentiment score answer.
    /// </summary>
    Task<JevResponse?> ScoreSentimentAsync(
        string state,
        CancellationToken cancellationToken = default);
}

internal class JevService : IJevService
{
    private readonly HttpClient _httpClient;
    private readonly JevConfiguration _config;

    public JevService(HttpClient httpClient, IOptions<JevConfiguration> options)
    {
        _httpClient = httpClient;
        _config = options.Value;
    }

    public async Task<JevResponse?> ScoreSentimentAsync(
        string state,
        CancellationToken cancellationToken = default)
    {
        var request = new JevRequest
        {
            State = state,
            Model = _config.Model,
            Questions = new Dictionary<string, JevQuestion>
            {
                ["sentiment_score"] = new JevQuestion
                {
                    Type = "score",
                    Instructions = "Where does the state fall on the scale?",
                    Criteria =
                    [
                        "positive sentiment",
                        "neutral sentiment",
                        "negative sentiment"
                    ]
                }
            }
        };

        using var message = new HttpRequestMessage(HttpMethod.Post, "v1/systemone")
        {
            Content = JsonContent.Create(request)
        };
        message.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _config.ApiKey);

        using var response = await _httpClient
            .SendAsync(message, cancellationToken)
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadFromJsonAsync<JevResponse>(cancellationToken)
            .ConfigureAwait(false);
    }
}
