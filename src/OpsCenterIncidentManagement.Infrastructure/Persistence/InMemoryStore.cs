using OpsCenterIncidentManagement.Domain.Entities;

namespace OpsCenterIncidentManagement.Infrastructure.Persistence
{
    public sealed class InMemoryStore
    {
        public List<Incident> Incidents { get; } = [];
        public List<OperatorAgent> Agents { get; } = [];
        public List<SlaPolicy> SlaPolicies { get; } = [];
        public List<IncidentAssignment> Assignments { get; } = [];
        public List<IncidentTimelineEvent> TimelineEvents { get; } = [];
    }
}
