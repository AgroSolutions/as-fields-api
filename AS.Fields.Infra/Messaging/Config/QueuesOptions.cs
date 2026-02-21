namespace AS.Fields.Infra.Messaging.Config
{
    public class QueuesOptions
    {
        public string UpdateFieldStatusQueue { get; set; } = string.Empty;
        public string CreateSensorQueue { get; set; } = string.Empty;
    }
}
