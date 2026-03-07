using Contracts;
using MassTransit;

namespace Orchestrator.Service.Core.Saga;

public class AlbumScraperSaga : MassTransitStateMachine<AlbumScraperState>
{
    public State SearchingForAlbumsList { get; private set; }
    public State ProcessingAlbums { get; private set; }
    
    public Event<StartAlbumsScraping> StartScrapping { get; private set; }
    public Event<AlbumsDiscovered> ListFound { get; private set; }
    public Event<AlbumSaved> AlbumProcessed { get; private set; }
    public Event<ScrapingFailed> JobFailed { get; private set; }
    
    public AlbumScraperSaga()
    {
        InstanceState(x => x.CurrentState);

        Event(() => StartScrapping, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => ListFound, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => AlbumProcessed, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => JobFailed, x => x.CorrelateById(m => m.Message.CorrelationId));
        
        Initially(
            When(StartScrapping)
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
            When(ListFound)
                .Then(context =>
                {
                    context.Saga.TotalAlbums = context.Message.AlbumIds.Count;
                })
                .ThenAsync(async context =>
                {
                    var tasks = context.Message.AlbumIds.Select(id =>
                        context.Publish(new ScrapeAlbumDetails(context.Saga.CorrelationId, id)));

                    await Task.WhenAll(tasks);
                })
                .TransitionTo(ProcessingAlbums)
        );
        
        During(ProcessingAlbums,
            When(AlbumProcessed)
                .Then(context => context.Saga.ProcessedAlbums++)
                .If(context => context.Saga.ProcessedAlbums >= context.Saga.TotalAlbums,
                    binder => binder
                        .Publish(context => new AllAlbumsScraped(context.Saga.CorrelationId, context.Saga.ArtistId))
                        .Finalize())
        );
    }
}