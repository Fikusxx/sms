using Confluent.Kafka;
using Contracts;
using MassTransit;

namespace Processing;

public static class DependencyInjection
{
    public static void AddKafka(this WebApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(x =>
        {
            x.UsingInMemory();
            

            x.AddRider(rider =>
            {
                rider.AddConsumer<SmsProcessedConsumer>();
                
                rider.UsingKafka((ctx, cfg) =>
                {
                    cfg.Host("localhost:9092"); 
                    cfg.AddMessageEndpoint(ctx);
                });
            });
        });
    }

    private static void AddMessageEndpoint(this IKafkaFactoryConfigurator cfg, IRiderRegistrationContext ctx)
    {
        var topic1Group = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "processing",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            Acks = Acks.All,
            AutoCommitIntervalMs = 5000,
            EnableAutoCommit = true,
            AllowAutoCreateTopics = true
        };

        cfg.TopicEndpoint<int, SmsProcessed>("templates",
            topic1Group,
            e =>
            {
                e.PrefetchCount = 1000;
                e.ConcurrentMessageLimit = 200;
                e.ConcurrentConsumerLimit = 1;
                e.ConcurrentDeliveryLimit = 1;
                e.ConfigureConsumer<SmsProcessedConsumer>(ctx);
            });
    }
}