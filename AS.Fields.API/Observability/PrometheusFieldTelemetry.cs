using AS.Fields.Application.Observability;
using Prometheus;

namespace AS.Fields.API.Observability;

public class PrometheusFieldTelemetry : IFieldTelemetry
{
    private const string Service = "as.sensors.api";
    private const string Env = "dev";

    public static readonly Gauge PropertyFieldsCount =
        Metrics.CreateGauge(
            "property_fields_total",
            "Quantidade atual de fields por property",
            new GaugeConfiguration
            {
                LabelNames = ["service", "env", "property_id"]
            });

    public void FieldCreated(Guid propertyId)
    {
        PropertyFieldsCount.WithLabels(Service, Env, propertyId.ToString()).Inc();
    }

    public void FieldDeleted(Guid propertyId)
    {
        PropertyFieldsCount.WithLabels(Service, Env, propertyId.ToString()).Dec();
    }
}
