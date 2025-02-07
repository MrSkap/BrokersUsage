using Common.Configuration;
using Producer.Application;
using Producer.Models.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
builder.Services.AddTransient<KafkaConfiguration>(provider => builder.Configuration.GetSection(KafkaConfiguration.SectionName).Get<KafkaConfiguration>() ?? new KafkaConfiguration());
builder.Services.AddTransient<TopicsConfiguration>(provider => builder.Configuration.GetSection("KafkaTopics").Get<TopicsConfiguration>() ?? new TopicsConfiguration());
builder.Services.AddSingleton<TopicInitializer>();
builder.Services.AddTransient<IProducer, Producer.Application.Producer>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

await app.Services.GetService<TopicInitializer>()!.InitializeAsync();
app.Run();
