using System.Net.Http.Json;
using System.Text.Json.Serialization;
using jevdotnet.Models;

namespace jevdotnet.Services.AmazonReviews;

internal sealed class AmazonReviewsService : IAmazonReviewsService
{
    // The datasets-server caps each page at 100 rows.
    private const int PageSize = 100;

    private const string Dataset = "hugginglearners/amazon-reviews-sentiment-analysis";
    private const string Config = "default";
    private const string Split = "train";

    private readonly HttpClient _httpClient;

    public AmazonReviewsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<AmazonReview>> GetReviewsAsync(
        int maxRecords = 100,
        CancellationToken cancellationToken = default)
    {
        if (maxRecords <= 0)
        {
            return Array.Empty<AmazonReview>();
        }

        var reviews = new List<AmazonReview>(Math.Min(maxRecords, PageSize));
        var offset = 0;

        while (reviews.Count < maxRecords)
        {
            var length = Math.Min(PageSize, maxRecords - reviews.Count);
            var requestUri =
                $"rows?dataset={Uri.EscapeDataString(Dataset)}" +
                $"&config={Uri.EscapeDataString(Config)}" +
                $"&split={Uri.EscapeDataString(Split)}" +
                $"&offset={offset}&length={length}";

            var page = await _httpClient
                .GetFromJsonAsync<RowsResponse>(requestUri, cancellationToken)
                .ConfigureAwait(false);

            if (page?.Rows is null || page.Rows.Count == 0)
            {
                break;
            }

            foreach (var entry in page.Rows)
            {
                if (entry.Row is not null)
                {
                    reviews.Add(entry.Row);
                }
            }

            offset += page.Rows.Count;

            if (offset >= page.NumRowsTotal)
            {
                break;
            }
        }

        return reviews;
    }

    /// <summary>Shape of the datasets-server /rows response.</summary>
    private sealed class RowsResponse
    {
        [JsonPropertyName("rows")]
        public List<RowEntry> Rows { get; init; } = [];

        [JsonPropertyName("num_rows_total")]
        public int NumRowsTotal { get; init; }
    }

    private sealed class RowEntry
    {
        [JsonPropertyName("row")]
        public AmazonReview? Row { get; init; }
    }
}
