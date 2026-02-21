using AS.Fields.Application.Publishers.Interfaces;
using AS.Fields.Domain.DTO.Messaging.Sensor;
using AS.Fields.Domain.Interfaces.Messaging;
using AS.Fields.Infra.Messaging.Config;
using Microsoft.Extensions.Options;

namespace AS.Fields.Application.Publishers
{
    public class SensorPublisher(
        IQueuePublisher publisher,
        IOptions<QueuesOptions> queues
    ) : ISensorPublisher
    {
        public Task CreateSensorAsync(CreateSensorDTO dto)
        {
            return publisher.PublishAsync(
                dto,
                queues.Value.CreateSensorQueue
            );
        }
    }
}
