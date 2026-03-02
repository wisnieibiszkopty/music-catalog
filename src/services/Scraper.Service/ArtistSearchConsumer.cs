using MassTransit;
using Contracts;

namespace Scraper.Service;

public class ArtistSearchConsumer : IConsumer<SearchArtist>
{
    public async Task Consume(ConsumeContext<SearchArtist> context)
    {
        Console.WriteLine(context.Message.Name);

        await Task.CompletedTask;
    }
}