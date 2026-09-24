using System.Text.Json.Serialization;

namespace jevdotnet.Models;

/// <summary>
/// Represents a single row from the
/// hugginglearners/amazon-reviews-sentiment-analysis dataset.
/// </summary>
public class AmazonReview
{
    [JsonPropertyName("reviewerName")]
    public string? ReviewerName { get; init; }

    [JsonPropertyName("overall")]
    public double Overall { get; init; }

    [JsonPropertyName("reviewText")]
    public string? ReviewText { get; init; }

    [JsonPropertyName("reviewTime")]
    public string? ReviewTime { get; init; }

    [JsonPropertyName("day_diff")]
    public long DayDiff { get; init; }

    [JsonPropertyName("helpful_yes")]
    public long HelpfulYes { get; init; }

    [JsonPropertyName("helpful_no")]
    public long HelpfulNo { get; init; }

    [JsonPropertyName("total_vote")]
    public long TotalVote { get; init; }

    [JsonPropertyName("score_pos_neg_diff")]
    public long ScorePosNegDiff { get; init; }

    [JsonPropertyName("score_average_rating")]
    public double ScoreAverageRating { get; init; }

    [JsonPropertyName("wilson_lower_bound")]
    public double WilsonLowerBound { get; init; }
}
