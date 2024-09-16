using Contracts;
using MassTransit;

namespace Actors;

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

                rider.UsingKafka((ctx, cfg) =>
                {
                    cfg.Host("localhost:9092");
                });
            });
        });
    }

    private static void AddMessageProducer(this IRiderRegistrationConfigurator cfg)
    {
        cfg.AddProducer<int, SmsRequested>("actors",
            m => m.Message.Id,
            (_, producerCfg) =>
            {
                producerCfg.EnableIdempotence = true;
                producerCfg.Linger = TimeSpan.FromMilliseconds(20);
            });
    }
}