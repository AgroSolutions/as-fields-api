namespace AS.Fields.Application.Observability;

public interface IFieldTelemetry
{
    void FieldCreated(Guid propertyId);
    void FieldDeleted(Guid propertyId);
}
