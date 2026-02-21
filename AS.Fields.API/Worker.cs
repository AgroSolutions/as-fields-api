using AS.Fields.Domain.DTO.Messaging.Field;
using AS.Fields.Domain.Interfaces.Messaging;
using AS.Fields.Infra.Messaging.Config;
using Microsoft.Extensions.Options;

namespace AS.Fields.API
{
    public class Worker(ILogger<Worker> logger, IOptions<QueuesOptions> queuesOptions, IQueueConsumer consumer, IMessageHandler<UpdateFieldStatusDTO> messageHandler) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await consumer.StartAsync(queuesOptions.Value.UpdateFieldStatusQueue, messageHandler, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error on Worker: {ErrorMessage}", ex.Message);
            }
            finally
            {
                logger.LogInformation("Worker stopped at: {time}", DateTimeOffset.Now);
            }
        }
    }
}
