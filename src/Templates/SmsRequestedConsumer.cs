using Contracts;
using MassTransit;

namespace Templates;

public sealed class SmsRequestedConsumer : IConsumer<SmsRequested>
{
    private readonly ITopicProducer<SmsProcessed> producer;
    
    public SmsRequestedConsumer(ITopicProducer<SmsProcessed> producer)
    {
        this.producer = producer;
    }

    public async Task Consume(ConsumeContext<SmsRequested> context)
    {
        var message = new SmsProcessed(context.Message.Id, DateTime.UtcNow);
        await producer.Produce(message);
    }
}