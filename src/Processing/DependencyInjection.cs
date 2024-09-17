using Confluent.Kafka;
using Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;

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
    
    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("SmsDb")));

        return builder;
    }
    
    public static WebApplication CreateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var context = scopedServices.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();

        return app;
    }
}