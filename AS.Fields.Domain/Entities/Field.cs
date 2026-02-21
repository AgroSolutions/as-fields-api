using AS.Fields.Domain.Enums;
using AS.Fields.Domain.ValueObjects;

namespace AS.Fields.Domain.Entities
{
    public class Field : BaseEntity
    {
        protected Field() { }
        public Field(Guid? id = null) : base(id) { }
    
        public required Guid PropertyId { get; init; }
        public Property? Property { get; protected set; }

        public required Boundary Boundary { get; init; }
        public required string Description { get; init; }

        public CropType Crop { get; private set; }
        public DateTime PlantingDate { get; private set; }
        public void ChangeCrop(CropType newCrop, DateTime plantingDate)
        {
            Crop = newCrop;
            PlantingDate = plantingDate;
        }

        public FieldStatus Status { get; init; }
        public string? Observations { get; init; }
    }
}
