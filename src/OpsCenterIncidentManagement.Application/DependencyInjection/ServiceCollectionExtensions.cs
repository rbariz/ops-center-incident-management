using Microsoft.Extensions.DependencyInjection;
using OpsCenterIncidentManagement.Application.UseCases.AssignIncident;
using OpsCenterIncidentManagement.Application.UseCases.EvaluateIncidentEscalation;
using OpsCenterIncidentManagement.Application.UseCases.GetIncidents;
using OpsCenterIncidentManagement.Application.UseCases.GetIncidentSummary;
using OpsCenterIncidentManagement.Application.UseCases.GetIncidentTimeline;
using OpsCenterIncidentManagement.Application.UseCases.GetOpsDashboard;
using OpsCenterIncidentManagement.Application.UseCases.ResolveIncident;
using OpsCenterIncidentManagement.Application.UseCases.StartIncidentWork;

namespace OpsCenterIncidentManagement.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<EvaluateIncidentEscalationHandler>(); 
        services.AddScoped<GetIncidentsHandler>();
        services.AddScoped<GetIncidentTimelineHandler>();
        services.AddScoped<GetOpsDashboardHandler>();
        services.AddScoped<GetIncidentSummaryHandler>();
        services.AddScoped<AssignIncidentHandler>();
        services.AddScoped<StartIncidentWorkHandler>();
        services.AddScoped<ResolveIncidentHandler>();

        return services;
    }
}
