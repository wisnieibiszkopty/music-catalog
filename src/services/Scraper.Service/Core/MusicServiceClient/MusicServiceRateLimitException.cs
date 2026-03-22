namespace Scraper.Service.Core.MusicServiceClient;

public class MusicServiceRateLimitException : Exception
{
    public double RetryAfterSeconds { get; }
    
    public MusicServiceRateLimitException(double retryAfterSeconds)
        : base($"Rate limited by Spotify. Retry after {TimeSpan.FromSeconds(retryAfterSeconds).ToString(@"hh\:mm\:ss")}.")
    {
        RetryAfterSeconds = retryAfterSeconds;
    }
}