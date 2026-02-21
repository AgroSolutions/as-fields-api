using AS.Fields.Domain.DTO.Messaging.Sensor;

namespace AS.Fields.Application.Publishers.Interfaces
{
    public interface ISensorPublisher
    {
        Task CreateSensorAsync(CreateSensorDTO dto);
    }
}