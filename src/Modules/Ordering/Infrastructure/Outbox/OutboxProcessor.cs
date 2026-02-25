using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SwiftScale.Modules.Ordering.Infrastructure.Outbox
{
    internal sealed class OutboxProcessor(IServiceProvider serviceProvider, ILogger<OutboxProcessor> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();
                    var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

                    // 1. Get unprocessed messages
                    var messages = await context.OutboxMessages
                        .Where(m => m.ProcessedOnUtc == null)
                        .OrderBy(m => m.OccurredOnUtc)
                        .Take(20)
                        .ToListAsync(stoppingToken);

                    foreach (var message in messages)
                    {
                        // 2. Reconstruct the Event object
                        var domainEvent = JsonConvert.DeserializeObject<INotification>(
                            message.Content,
                            new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

                        if (domainEvent is null) continue;

                        // 3. Publish to handlers
                        await publisher.Publish(domainEvent, stoppingToken);

                        // 4. Mark as finished
                        message.ProcessedOnUtc = DateTime.UtcNow;
                    }

                    await context.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing outbox messages");
                }

                // Wait 10 seconds before checking again
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
