# jevdotnet

A small .NET console app that benchmarks the [Jev](https://console.typesafe.ai/home) model's sentiment analysis against a pre-labelled dataset of Amazon product reviews.

It pulls reviews from a HuggingFace dataset that already carries a star rating, asks Jev to classify the sentiment of each review, and prints an agreement score.

In practice it lands around **90%+ agreement**:

```
Analysing 100 randomly selected reviews...

===== Sentiment Match Summary =====
Reviews analysed : 100
Matches          : 93
Mismatches       : 7
Agreement        : 93.0%
```

## Getting started

### 1. Get an API key

1. Go to <https://console.typesafe.ai/home> and sign up.
2. Create an API key.

### 2. Add the key to `appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "Jev": {
    "ApiKey": ""
  }
}
```

Put your key in the `ApiKey` field.

> [!WARNING]
> `appsettings.json` is tracked by git, so a key pasted there can easily be committed — and this repo is public. To keep it out of version control, either run:
>
> ```bash
> git update-index --skip-worktree jevdotnet/appsettings.json
> ```
>
> or use [user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) instead:
>
> ```bash
> cd jevdotnet
> dotnet user-secrets init
> dotnet user-secrets set "Jev:ApiKey" "<your-key>"
> ```

### 3. Run it

```bash
dotnet run --project jevdotnet
```

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

## What it actually does

1. **Fetches reviews** — pulls 1,000 rows from the HuggingFace [`hugginglearners/amazon-reviews-sentiment-analysis`](https://huggingface.co/datasets/hugginglearners/amazon-reviews-sentiment-analysis) dataset via the public `datasets-server` API. No auth needed, paged 100 rows at a time.
2. **Samples 100 at random** — skipping any row with empty review text.
3. **Asks Jev to classify each one** — a `POST` to `v1/systemone` with a `score` question and three criteria: `positive sentiment`, `neutral sentiment`, `negative sentiment`.
4. **Compares against the star rating** — the dataset's 1–5 `overall` score is mapped to a label:

   | Stars  | Sentiment  |
   | ------ | ---------- |
   | 4–5    | `positive` |
   | 3      | `neutral`  |
   | 1–2    | `negative` |

5. **Prints the agreement summary.**

Because the sample is random, the number moves a little between runs.

## Configuration

All settings live under the `Jev` section:

| Setting   | Default                   | Description                      |
| --------- | ------------------------- | -------------------------------- |
| `ApiKey`  | _(required)_              | Your typesafe.ai API key.        |
| `BaseUrl` | `https://api.typesafe.ai/` | Jev API base address.            |
| `Model`   | `jev-latest`              | Model identifier.                |

## Project layout

```
jevdotnet/
├── Program.cs                      # Wiring + the comparison loop
├── Configuration/
│   └── JevConfiguration.cs         # Bound from the "Jev" config section
├── Models/
│   ├── AmazonReview.cs             # A dataset row
│   └── Jev/                        # Jev request/response contracts
└── Services/
    ├── AmazonReviews/              # HuggingFace datasets-server client
    └── Jev/                        # Jev API client
```

Both clients are registered as typed `HttpClient`s through `IHttpClientFactory`.

## Notes

- Reviews are sent to Jev **sequentially**, so a 100-review run takes a little while. Fine for a benchmark; you'd want batching or parallelism for anything larger.
- If a response comes back without a `sentiment_score` answer, that review is skipped and excluded from the totals — which is why `Reviews analysed` can read lower than the sample size.
