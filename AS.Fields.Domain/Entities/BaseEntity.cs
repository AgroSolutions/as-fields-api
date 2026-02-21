namespace AS.Fields.Domain.Entities
{
    public abstract class BaseEntity(Guid? id = null)
    {
        public Guid Id { get; protected set; } = id ?? Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = id is null ? DateTime.UtcNow : default;
        public DateTime UpdatedAt { get; set; }
    }
}