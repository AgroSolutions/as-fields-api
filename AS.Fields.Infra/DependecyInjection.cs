using Amazon.SQS;
using AS.Fields.Domain.Interfaces.Messaging;
using AS.Fields.Domain.Interfaces.Repositories;
using AS.Fields.Infra.Messaging.Sqs;
using AS.Fields.Infra.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AS.Fields.Infra
{
    public static class DependecyInjection
    {
        public static IServiceCollection ConfigureAmazonSQS(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDefaultAWSOptions(configuration.GetAWSOptions());
            services.AddAWSService<IAmazonSQS>();

            services.AddTransient<IQueuePublisher, AmazonSqsPublisher>();
            services.AddTransient<IQueueConsumer, AmazonSqsConsumer>();
            return services;
        }

        public static IServiceCollection AddInfraModules(this IServiceCollection services)
        {
            // Repositories
            services.AddTransient<IFieldRepository, FieldRepository>();
            services.AddTransient<IPropertyRepository, PropertyRepository>();

            return services;
        }
    }
}
