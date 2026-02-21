namespace AS.Fields.Domain.Entities
{
    public class Property : BaseEntity
    {
        public Property(Guid? id = null) : base(id) { }
        protected Property() { }

        public required string Name { get; init; }
        public required string Description { get; init; }
        public required Guid FarmerId { get; init; }
        public List<Field> Fields { get; set; } = [];
    }
}
