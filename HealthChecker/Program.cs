var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseWebRoot("wwwroot");

builder.Services.AddHealthChecksUI(settings =>
{
    settings.AddHealthCheckEndpoint("Student api", "http://host.docker.internal:5082/health");
    settings.AddHealthCheckEndpoint("Courses api", "http://host.docker.internal:5081/health");
}).AddInMemoryStorage();

var app = builder.Build();

app.UseStaticFiles();

app.MapHealthChecksUI(setup =>
{
    setup.ApiPath = "/health-api";
    setup.UIPath = "/health-ui";
});

app.Run();
