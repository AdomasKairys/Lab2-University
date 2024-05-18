using Microsoft.EntityFrameworkCore;
using CoursesAPI.Data;
using Plain.RabbitMQ;
using RabbitMQ.Client;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using CoursesAPI.Health;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<CoursesAPIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CoursesAPIContext") ?? throw new InvalidOperationException("Connection string 'CoursesAPIContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog((context, LoggerConfiguration) => LoggerConfiguration.ReadFrom.Configuration(context.Configuration));
//builder.Services.AddSingleton<IConnectionProvider>(new ConnectionProvider("amqp://guest:guest@rabbitmq:5672"));

builder.Services.AddSingleton<IConnectionProvider>(new ConnectionProvider(builder.Configuration["MessageBroker:Host"]));
builder.Services.AddScoped<IPublisher>(x => new Publisher(x.GetService<IConnectionProvider>(),
 "enrollment_exchange", ExchangeType.Topic));


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
