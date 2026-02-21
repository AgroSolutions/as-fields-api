namespace AS.Fields.Domain.Interfaces.Messaging
{
    public interface IQueuePublisher
    {
        Task PublishAsync<T>(T message, string queueName, CancellationToken cancellationToken = default);
    }
}
