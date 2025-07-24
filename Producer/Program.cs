using Common.Configuration;
using Common.Nats;
using Common.Nats.Extensions;
using Common.Nats.Initializers;
using Producer.Application;
using Producer.Models.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
builder.Services.AddTransient<KafkaConfiguration>(provider =>
    builder.Configuration.GetSection(KafkaConfiguration.SectionName).Get<KafkaConfiguration>() ??
    new KafkaConfiguration());
builder.Services.AddTransient<TopicsConfiguration>(provider =>
    builder.Configuration.GetSection("KafkaTopics").Get<TopicsConfiguration>() ?? new TopicsConfiguration());
builder.Services.AddTransient<NatsConnectionOptions>(provider =>
    builder.Configuration.GetSection("Nats").Get<NatsConnectionOptions>()!);
builder.Services.AddSingleton<TopicInitializer>();
builder.Services.AddTransient<IProducer, Producer.Application.Producer>();
builder.Services.AddNats();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


await app.Services.GetService<TopicInitializer>()!.InitializeAsync();
NatsInitializer.StartNatsProcessing(app.Services);

app.Run();