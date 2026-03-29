using Contracts;
using MassTransit;

namespace Orchestrator.Service.Core.Saga;

public class AlbumScraperSaga : MassTransitStateMachine<AlbumScraperState>
{
    public State SearchingForAlbumsList { get; private set; }
    public State ProcessingAlbums { get; private set; }
    
    public Event<StartAlbumsScraping> StartedAlbumsScraping { get; private set; }
    public Event<AlbumsDiscovered> AlbumsDiscovered { get; private set; }
    public Event<AlbumSaved> AlbumSaved { get; private set; }
    public Event<ScrapingFailed> ScrapingFailed { get; private set; }
    
    public AlbumScraperSaga()
    {
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
                })
                .Publish(context => new DiscoverAlbums(context.Saga.CorrelationId, context.Saga.ArtistId))
                .TransitionTo(SearchingForAlbumsList)
        );
        
        During(SearchingForAlbumsList, 
            When(AlbumsDiscovered)
                .Then(context =>
                {
                    context.Saga.TotalAlbums = context.Message.AlbumIds.Count;
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
                .Then(context => context.Saga.ProcessedAlbums++)
                .If(context => context.Saga.ProcessedAlbums >= context.Saga.TotalAlbums,
                    binder => binder
                        .Publish(context => new AllAlbumsScraped(context.Saga.CorrelationId, context.Saga.ArtistId))
                        .Finalize())
        );
    }
}