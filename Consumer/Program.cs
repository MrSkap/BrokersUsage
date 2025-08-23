using Common.Nats;
using Common.Nats.Configuration;
using Common.Nats.Extensions;
using Common.Nats.Initializers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<NatsConnectionOptions>(provider =>
    builder.Configuration.GetSection(NatsConnectionOptions.SectionName).Get<NatsConnectionOptions>()!);
builder.Services.AddTransient<NatsStreamSubscriptionConfiguration>(provider =>
    builder.Configuration.GetSection(NatsStreamSubscriptionConfiguration.SectionName)
        .Get<NatsStreamSubscriptionConfiguration>()!);
builder.Services.AddNats();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

NatsInitializer.StartNatsProcessing(app.Services, NatsServiceName.ConsumerService);
NatsInitializer.StartStreamsMessageProcessing(app.Services);

app.Run();