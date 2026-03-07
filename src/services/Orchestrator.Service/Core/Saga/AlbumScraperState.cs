using MassTransit;

namespace Orchestrator.Service.Core.Saga;

public class AlbumScraperState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public required string CurrentState { get; set; }
    
    public required string ArtistId { get; set; }
    public int TotalAlbums { get; set; }
    public int ProcessedAlbums { get; set; }
    public DateTime? RequestTime { get; set; }
}