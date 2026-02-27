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

        public required Boundary Boundary { get; set; }
        public required string Description { get; set; }

        public CropType Crop { get; private set; }
        public DateTime PlantingDate { get; private set; }
        public void ChangeCrop(CropType newCrop, DateTime plantingDate)
        {
            Crop = newCrop;
            PlantingDate = plantingDate;
        }

        public FieldStatus Status { get; private set; }
        public void ChangeStatus(FieldStatus newStatus) => Status = newStatus;
        public string? Observations { get; set; }
    }
}
