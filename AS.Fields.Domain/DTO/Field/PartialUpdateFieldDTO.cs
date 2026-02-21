using AS.Fields.Domain.Enums;
using AS.Fields.Domain.ValueObjects;

namespace AS.Fields.Domain.DTO.Field
{
    public class PartialUpdateFieldDTO
    {
        public string? Description { get; init; }
        public Boundary? Boundary { get; init; }

        public CropType? Crop { get; init; }
        public DateTime? PlantingDate { get; init; }
        public string? Observations { get; init; }
    }
}
