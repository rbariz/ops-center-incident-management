using OpsCenterIncidentManagement.Application.DependencyInjection;
using OpsCenterIncidentManagement.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApplication();
builder.Services.AddInfrastructure();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Services.SeedInMemoryData();

app.Run();