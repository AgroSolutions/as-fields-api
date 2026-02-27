using AS.Fields.Application.Observability;
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
}
