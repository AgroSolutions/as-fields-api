namespace AS.Fields.Domain.DTO.Messaging.Sensor
{
    public class CreateSensorDTO
    {
        public required Guid FieldId { get; set; }
        public required string Name { get; set; }
    }
}
