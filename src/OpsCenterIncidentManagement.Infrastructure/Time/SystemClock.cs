using OpsCenterIncidentManagement.Application.Abstractions;

namespace OpsCenterIncidentManagement.Infrastructure.Time
{
    public sealed class SystemClock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
