using Contracts;
using MassTransit;

namespace Processing;

public sealed class SmsProcessedConsumer : IConsumer<SmsProcessed>
{
    public async Task Consume(ConsumeContext<SmsProcessed> context)
    {
        await Task.Delay(50); // simulate external call

        switch (context.Message.Id)
        {
            case 0:
                Console.WriteLine($"{context.Message.Id} : Started {context.Message.StartedAt} ");
                break;
            case 999_999:
                Console.WriteLine($"{context.Message.Id} : Processed {DateTime.UtcNow}");
                break;
        }
    }
}