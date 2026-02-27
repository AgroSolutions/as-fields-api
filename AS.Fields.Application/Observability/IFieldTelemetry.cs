using AS.Fields.Domain.Enums;

namespace AS.Fields.Application.Observability;

public interface IFieldTelemetry
{
    void FieldCreated(Guid propertyId, string propertyName);
    void FieldDeleted(Guid propertyId, string propertyName);
    void FieldStatusChanged(Guid propertyId, string propertyName, Guid fieldId, FieldStatus status);
}
