namespace AS.Fields.Domain.Interfaces.Messaging
{
    public interface IMessageHandler<T>
    {
        Task HandleAsync(T message, CancellationToken cancellationToken = default);
    }
}
