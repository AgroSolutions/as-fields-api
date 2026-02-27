using AS.Fields.Application.Observability;
using AS.Fields.Domain.Enums;
using Prometheus;

namespace AS.Fields.API.Observability;

public class PrometheusFieldTelemetry : IFieldTelemetry
{
    private const string Service = "as.fields.api";
    private const string Env = "dev";

    public static readonly Gauge PropertyFieldsCount =
        Metrics.CreateGauge(
            "as_fields_property_fields_total",
            "Quantidade atual de fields por property",
            new GaugeConfiguration
            {
                LabelNames = ["service", "env", "property_id", "property_name"]
            });

    private static readonly Gauge CurrentFieldStatusGauge =
    Metrics.CreateGauge(
        "as_fields_current_field_status",
        "Status atual do talhao",
        new GaugeConfiguration
        {
            LabelNames = ["property_id", "property_name", "field_id"]
        });

    public void FieldCreated(Guid propertyId, string propertyName)
    {
        PropertyFieldsCount.WithLabels(Service, Env, propertyId.ToString(), propertyName)
            .Inc();
    }

    public void FieldDeleted(Guid propertyId, string propertyName)
    {
        PropertyFieldsCount.WithLabels(Service, Env, propertyId.ToString(), propertyName)
            .Dec();
    }

    public void FieldStatusChanged(Guid propertyId, string propertyName, Guid fieldId, FieldStatus status)
    {
        CurrentFieldStatusGauge
            .WithLabels(propertyId.ToString(), propertyName, fieldId.ToString())
            .Set((int)status);
    }
}
