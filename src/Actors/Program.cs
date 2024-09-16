using System.Diagnostics;
using Actors;
using Contracts;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.AddKafka();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/publish", async (ITopicProducer<SmsRequested> producer) =>
    {
        var watch = Stopwatch.StartNew();
        for (var i = 0; i < 1_000_000; i++)
        {
            producer.Produce(new SmsRequested(i));
        }

        await Task.Delay(1000);
        Console.WriteLine($"All messages have been sent.. Took {watch.ElapsedMilliseconds} ms");
    })
    .WithName("Publish");

app.Run();