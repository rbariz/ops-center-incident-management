using Microsoft.Extensions.DependencyInjection;
using OpsCenterIncidentManagement.Application.Abstractions;
using OpsCenterIncidentManagement.Infrastructure.Persistence;
using OpsCenterIncidentManagement.Infrastructure.Time;

namespace OpsCenterIncidentManagement.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<InMemoryStore>();

            services.AddScoped<IIncidentRepository, InMemoryIncidentRepository>();
            services.AddScoped<ISlaPolicyRepository, InMemorySlaPolicyRepository>();
            services.AddScoped<IIncidentTimelineEventRepository, InMemoryIncidentTimelineEventRepository>();
            services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
            services.AddSingleton<IClock, SystemClock>();

            return services;
        }

        public static void SeedInMemoryData(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var store = scope.ServiceProvider.GetRequiredService<InMemoryStore>();
            InMemorySeed.Seed(store);
        }
    }
}
