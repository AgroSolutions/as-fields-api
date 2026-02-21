using AS.Fields.Domain.Enums;

namespace AS.Fields.Domain.DTO.Messaging.Field
{
    public class UpdateFieldStatusDTO
    {
        public Guid FieldId { get; set; }
        public FieldStatus Status { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
