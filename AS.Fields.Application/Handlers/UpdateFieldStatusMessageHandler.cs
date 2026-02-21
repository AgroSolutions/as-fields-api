using AS.Fields.Domain.DTO.Messaging.Field;
using AS.Fields.Domain.Interfaces.Messaging;
using Microsoft.Extensions.Logging;

namespace AS.Fields.Application.Handlers
{
    public class UpdateFieldStatusMessageHandler(ILogger<UpdateFieldStatusMessageHandler> logger) : IMessageHandler<UpdateFieldStatusDTO>
    {
        public Task HandleAsync(UpdateFieldStatusDTO message, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Received message: {@Message}", message);
            return Task.CompletedTask;
        }
    }
}
