using Microsoft.Extensions.DependencyInjection;
using OpsCenterIncidentManagement.Application.UseCases.EvaluateIncidentEscalation;
using OpsCenterIncidentManagement.Application.UseCases.GetIncidents;
using OpsCenterIncidentManagement.Application.UseCases.GetIncidentTimeline;
using OpsCenterIncidentManagement.Application.UseCases.GetOpsDashboard;

namespace OpsCenterIncidentManagement.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<EvaluateIncidentEscalationHandler>(); 
        services.AddScoped<GetIncidentsHandler>();
        services.AddScoped<GetIncidentTimelineHandler>();
        services.AddScoped<GetOpsDashboardHandler>();
        return services;
    }
}
