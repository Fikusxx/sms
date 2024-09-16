using Confluent.Kafka;
using Contracts;
using MassTransit;

namespace Templates;

public static class DependencyInjection
{
    public static void AddKafka(this WebApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(x =>
        {
            x.UsingInMemory();

            x.AddRider(rider =>
            {
                rider.AddMessageProducer();
                rider.AddConsumer<SmsRequestedConsumer>();

                rider.UsingKafka((ctx, cfg) =>
                {
                    cfg.Host("localhost:9092");
                    cfg.AddMessageEndpoint(ctx);
                });
            });
        });
    }

    private static void AddMessageProducer(this IRiderRegistrationConfigurator cfg)
    {
        cfg.AddProducer<int, SmsProcessed>("templates",
            m => m.Message.Id,
            (_, producerCfg) =>
            {
                producerCfg.EnableIdempotence = true;
                producerCfg.Linger = TimeSpan.FromMilliseconds(20);
            });
    }

    private static void AddMessageEndpoint(this IKafkaFactoryConfigurator cfg, IRiderRegistrationContext ctx)
    {
        var topic1Group = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "templates",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            Acks = Acks.All,
            AutoCommitIntervalMs = 5000,
            EnableAutoCommit = true,
            AllowAutoCreateTopics = true
        };

        cfg.TopicEndpoint<int, SmsRequested>("actors",
            topic1Group,
            e =>
            {
                e.PrefetchCount = 1000;
                e.ConcurrentMessageLimit = 200;
                e.ConcurrentConsumerLimit = 1;
                e.ConcurrentDeliveryLimit = 1;
                e.ConfigureConsumer<SmsRequestedConsumer>(ctx);
            });
    }
}