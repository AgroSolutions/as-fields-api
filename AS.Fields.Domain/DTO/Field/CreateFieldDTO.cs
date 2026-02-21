using AS.Fields.Domain.Enums;
using AS.Fields.Domain.ValueObjects;

namespace AS.Fields.Domain.DTO.Field
{
    public class CreateFieldDTO
    {
        public required string Description { get; set; }
        public required Boundary Boundary { get; set; }

        public CropType Crop { get; set; }
        public DateTime PlantingDate { get; set; }
        public string? Observations { get; set; }
    }
}
