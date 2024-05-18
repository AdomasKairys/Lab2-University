using Microsoft.EntityFrameworkCore;
using StudentsAPI.Data;
using Plain.RabbitMQ;
using RabbitMQ.Client;
using System.Text.Json.Serialization;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using StudentsAPI.Health;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<StudentsAPIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StudentsAPIContext") ?? throw new InvalidOperationException("Connection string 'StudentsAPIContext' not found.")));

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog((context, LoggerConfiguration) => LoggerConfiguration.ReadFrom.Configuration(context.Configuration));
//builder.Services.AddSingleton<IConnectionProvider>(new ConnectionProvider("amqp://guest:guest@rabbitmq:5672"));

builder.Services.AddSingleton<IConnectionProvider>(new ConnectionProvider(builder.Configuration["MessageBroker:Host"]));
builder.Services.AddSingleton<ISubscriber>(x => new Subscriber(x.GetService<IConnectionProvider>(),
 "enrollment_exchange", "enrollment_queue", "enrollment_topic", ExchangeType.Topic));
builder.Services.AddHostedService<StudentsAPI.EnrollmentDataCollector>();



builder.Services.AddHealthChecks().AddCheck<GetHealthCheck>("HTTP request");

var app = builder.Build();


app.UseSerilogRequestLogging();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health", new HealthCheckOptions()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
