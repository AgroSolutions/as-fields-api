namespace AS.Fields.Domain.Interfaces.Messaging
{
    public interface IQueueConsumer
    {
        Task StartAsync<T>(string queueName, IMessageHandler<T> handler, 
            CancellationToken cancellationToken = default);
    }
}
