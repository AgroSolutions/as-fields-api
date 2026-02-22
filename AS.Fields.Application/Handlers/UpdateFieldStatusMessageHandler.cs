using AS.Fields.Domain.DTO.Messaging.Field;
using AS.Fields.Domain.Interfaces.Messaging;
using AS.Fields.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AS.Fields.Application.Handlers
{
    public class UpdateFieldStatusMessageHandler(ILogger<UpdateFieldStatusMessageHandler> logger, IFieldService fieldService) : IMessageHandler<UpdateFieldStatusDTO>
    {
        public async Task HandleAsync(UpdateFieldStatusDTO message, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("[UpdateFieldStatus] Received message: {@Message}", JsonConvert.SerializeObject(message));
            bool atualizado = await fieldService.UpdateStatus(message);
            logger.LogInformation("[UpdateFieldStatus] Update status result for FieldId {FieldId}: {Result}", message.FieldId, atualizado ? "Success" : "Failed");
        }
    }
}
