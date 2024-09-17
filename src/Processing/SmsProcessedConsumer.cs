using Contracts;
using MassTransit;

namespace Processing;

public sealed class SmsProcessedConsumer : IConsumer<SmsProcessed>
{
    private readonly AppDbContext ctx;

    public SmsProcessedConsumer(AppDbContext ctx)
    {
        this.ctx = ctx;
    }

    public async Task Consume(ConsumeContext<SmsProcessed> context)
    {
        await Task.Delay(50); // simulate external call

        await ctx.Sms.AddAsync(new SmsResult(context.Message.Id, context.Message.StartedAt, DateTime.UtcNow));
        await ctx.SaveChangesAsync();

        // switch (context.Message.Id)
        // {
        //     case 0:
        //         Console.WriteLine($"{context.Message.Id} : Started {context.Message.StartedAt} ");
        //         break;
        //     case 999_999:
        //         Console.WriteLine($"{context.Message.Id} : Processed {DateTime.UtcNow}");
        //         break;
        // }
    }
}