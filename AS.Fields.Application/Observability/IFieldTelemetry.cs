namespace AS.Fields.Application.Observability;

public interface IFieldTelemetry
{
    void FieldCreated(Guid propertyId, string propertyName);
    void FieldDeleted(Guid propertyId, string propertyName);
}
