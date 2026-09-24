using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using jevdotnet.Configuration;
using jevdotnet.Services.AmazonReviews;
using jevdotnet.Services.Jev;

namespace jevdotnet;

internal class Program
{
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var services = new ServiceCollection();

        // Configure JevConfiguration from appsettings.json
        services.Configure<JevConfiguration>(configuration.GetSection("Jev"));
        var jevConfig = configuration.GetSection("Jev").Get<JevConfiguration>()!;

        // Typed HTTP client for the Jev system-one API
        services.AddHttpClient<IJevService, JevService>(client =>
        {
            client.BaseAddress = new Uri(jevConfig.BaseUrl);
        });

        // Typed HTTP client for the HuggingFace datasets-server
        services.AddHttpClient<IAmazonReviewsService, AmazonReviewsService>(client =>
        {
            client.BaseAddress = new Uri("https://datasets-server.huggingface.co/");
        });

        var serviceProvider = services.BuildServiceProvider();

        var reviewsService = serviceProvider.GetRequiredService<IAmazonReviewsService>();
        var allReviews = await reviewsService.GetReviewsAsync(maxRecords: 1000);

        // Pick 100 reviews at random (that actually have text).
        var sample = allReviews
            .Where(r => !string.IsNullOrWhiteSpace(r.ReviewText))
            .OrderBy(_ => Random.Shared.Next())
            .Take(100)
            .ToList();

        Console.WriteLine($"Analysing {sample.Count} randomly selected reviews...\n");

        var jevService = serviceProvider.GetRequiredService<IJevService>();
        var matches = 0;
        var analysed = 0;

        foreach (var review in sample)
        {
            var datasetSentiment = ToSentiment(review.Overall);

            var result = await jevService.ScoreSentimentAsync(review.ReviewText!);
            var answer = result?.Answers.GetValueOrDefault("sentiment_score");
            if (answer is null)
            {
                continue;
            }

            var jevSentiment = answer.Legend
                .GetValueOrDefault(((int)answer.Score).ToString(), "unknown")
                .Split(' ')[0];

            analysed++;
            if (string.Equals(datasetSentiment, jevSentiment, StringComparison.OrdinalIgnoreCase))
            {
                matches++;
            }
        }

        var accuracy = analysed > 0 ? (double)matches / analysed * 100 : 0;

        Console.WriteLine("===== Sentiment Match Summary =====");
        Console.WriteLine($"Reviews analysed : {analysed}");
        Console.WriteLine($"Matches          : {matches}");
        Console.WriteLine($"Mismatches       : {analysed - matches}");
        Console.WriteLine($"Agreement        : {accuracy:0.0}%");
    }

    // Maps the dataset's 1-5 star "overall" rating to a sentiment label.
    private static string ToSentiment(double overall) => overall switch
    {
        >= 4 => "positive",
        >= 3 => "neutral",
        _ => "negative"
    };
}
