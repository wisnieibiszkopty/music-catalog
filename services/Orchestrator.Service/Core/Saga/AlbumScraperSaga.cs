using Contracts;
using MassTransit;

namespace Orchestrator.Service.Core.Saga;

public class AlbumScraperSaga : MassTransitStateMachine<AlbumScraperState>
{
    private readonly ILogger<AlbumScraperSaga> _logger;
    
    public State SearchingForAlbumsList { get; private set; }
    public State ProcessingAlbums { get; private set; }
    
    public Event<StartAlbumsScraping> StartedAlbumsScraping { get; private set; }
    public Event<AlbumsDiscovered> AlbumsDiscovered { get; private set; }
    public Event<AlbumSaved> AlbumSaved { get; private set; }
    public Event<ScrapingFailed> ScrapingFailed { get; private set; }
    
    public AlbumScraperSaga(ILogger<AlbumScraperSaga> logger)
    {
        _logger = logger;
        
        InstanceState(x => x.CurrentState);

        Event(() => StartedAlbumsScraping, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => AlbumsDiscovered, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => AlbumSaved, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => ScrapingFailed, x => x.CorrelateById(m => m.Message.CorrelationId));
        
        Initially(
            When(StartedAlbumsScraping)
                .Then(context =>
                {
                    context.Saga.ArtistId = context.Message.ArtistId;
                    context.Saga.RequestTime = DateTime.UtcNow;
                    context.Saga.ProcessedAlbums = 0;
                    
                    _logger.LogInformation("Saga started for ArtistId: {ArtistId}. CorrelationId: {CorrelationId}", 
                        context.Message.ArtistId, context.Saga.CorrelationId);
                })
                .Publish(context => new DiscoverAlbums(context.Saga.CorrelationId, context.Saga.ArtistId))
                .TransitionTo(SearchingForAlbumsList)
        );
        
        During(SearchingForAlbumsList, 
            When(AlbumsDiscovered)
                .Then(context =>
                {
                    context.Saga.TotalAlbums = context.Message.AlbumIds.Count;
                    _logger.LogInformation("Discovered {TotalAlbums} albums for ArtistId: {ArtistId}. Starting scraping details...", 
                        context.Saga.TotalAlbums, context.Saga.ArtistId);
                })
                .ThenAsync(async context =>
                {
                    foreach (var id in context.Message.AlbumIds)
                    {
                        await context.Publish(new ScrapeAlbumDetails(context.Saga.CorrelationId, id, context.Saga.ArtistId));
                    }
                })
                .TransitionTo(ProcessingAlbums)
        );
        
        During(ProcessingAlbums,
            When(AlbumSaved)
                .Then(context =>
                {
                    context.Saga.ProcessedAlbums++;
                    _logger.LogDebug("Album saved ({Processed}/{Total}) for ArtistId: {ArtistId}", 
                        context.Saga.ProcessedAlbums, context.Saga.TotalAlbums, context.Saga.ArtistId);
                })
                .If(context =>
                    {
                        var processedAlbums = context.Saga.ProcessedAlbums;
                        var totalAlbums = context.Saga.TotalAlbums;
                        _logger.LogInformation(
                            "All {TotalAlbums} albums processed successfully for ArtistId: {ArtistId}. Finalizing saga.",
                            totalAlbums,
                            context.Saga.ArtistId
                        );
                        return processedAlbums >= totalAlbums;
                    },
                    binder => binder
                        .Publish(context => new AllAlbumsScraped(context.Saga.CorrelationId, context.Saga.ArtistId))
                        .Finalize())
        );
    }
}