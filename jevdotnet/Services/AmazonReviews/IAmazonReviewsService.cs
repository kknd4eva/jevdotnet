using jevdotnet.Models;

namespace jevdotnet.Services.AmazonReviews;

/// <summary>
/// Retrieves the hugginglearners/amazon-reviews-sentiment-analysis dataset
/// and maps it into strongly-typed <see cref="AmazonReview"/> objects.
/// </summary>
public interface IAmazonReviewsService
{
    /// <summary>
    /// Downloads reviews from the dataset, paging through the HuggingFace
    /// datasets-server until <paramref name="maxRecords"/> is reached or the
    /// dataset is exhausted.
    /// </summary>
    /// <param name="maxRecords">Maximum number of reviews to return.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task<IReadOnlyList<AmazonReview>> GetReviewsAsync(
        int maxRecords = 100,
        CancellationToken cancellationToken = default);
}
