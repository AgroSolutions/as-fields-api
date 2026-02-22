using Amazon.SQS.Model;
using Amazon.SQS;
using Newtonsoft.Json;
using AS.Fields.Domain.Interfaces.Messaging;
using Microsoft.Extensions.Logging;

namespace AS.Fields.Infra.Messaging.Sqs
{
    public class AmazonSqsPublisher(ILogger<AmazonSqsPublisher> logger, IAmazonSQS sqs) : IQueuePublisher
    {
        public async Task PublishAsync<T>(T message, string queueName, CancellationToken cancellationToken = default)
        {
            try
            {
                logger.LogInformation("Publishing message to SQS queue {QueueName}: {@Message}", queueName, JsonConvert.SerializeObject(message));
                var queueUrlResponse = await sqs.GetQueueUrlAsync(queueName, cancellationToken);

                var request = new SendMessageRequest
                {
                    QueueUrl = queueUrlResponse.QueueUrl,
                    MessageBody = JsonConvert.SerializeObject(message)
                };

                await sqs.SendMessageAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Falha ao publicar mensagem para a fila {QueueName} do SQS", queueName);
            }
        }
    }
}
