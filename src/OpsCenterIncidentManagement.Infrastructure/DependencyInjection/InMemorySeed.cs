using OpsCenterIncidentManagement.Domain.Entities;
using OpsCenterIncidentManagement.Domain.Enums;
using OpsCenterIncidentManagement.Infrastructure.Persistence;

namespace OpsCenterIncidentManagement.Infrastructure.DependencyInjection
{
    public static class InMemorySeed
    {
        public static readonly Guid DemoIncidentId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        public static void Seed(InMemoryStore store)
        {
            if (store.Incidents.Count > 0)
                return;

            var now = DateTime.UtcNow;

            var agent = new OperatorAgent(
                Guid.Parse("22222222-2222-2222-2222-222222222222"),
                "Ops Agent 1",
                "Control Room");

            var policyLow = new SlaPolicy(Guid.NewGuid(), "Low SLA", IncidentPriority.Low, 240);
            var policyMedium = new SlaPolicy(Guid.NewGuid(), "Medium SLA", IncidentPriority.Medium, 120);
            var policyHigh = new SlaPolicy(Guid.NewGuid(), "High SLA", IncidentPriority.High, 30);
            var policyCritical = new SlaPolicy(Guid.NewGuid(), "Critical SLA", IncidentPriority.Critical, 10);

            var incident = new Incident(
                DemoIncidentId,
                "Network outage on Site A",
                "Connectivity lost on site perimeter devices.",
                "Site A",
                IncidentPriority.High,
                now.AddMinutes(-45));

            incident.Assign(now.AddMinutes(-40));
            incident.StartWork();

            var createdEvent = new IncidentTimelineEvent(
                Guid.NewGuid(),
                incident.Id,
                TimelineEventType.Created,
                "Incident created by monitoring system.",
                now.AddMinutes(-45));

            var assignedEvent = new IncidentTimelineEvent(
                Guid.NewGuid(),
                incident.Id,
                TimelineEventType.Assigned,
                "Incident assigned to Ops Agent 1.",
                now.AddMinutes(-40));

            var inProgressEvent = new IncidentTimelineEvent(
                Guid.NewGuid(),
                incident.Id,
                TimelineEventType.StatusChanged,
                "Incident moved to InProgress.",
                now.AddMinutes(-38));

            store.Agents.Add(agent);
            store.SlaPolicies.Add(policyLow);
            store.SlaPolicies.Add(policyMedium);
            store.SlaPolicies.Add(policyHigh);
            store.SlaPolicies.Add(policyCritical);
            store.Incidents.Add(incident);
            store.TimelineEvents.Add(createdEvent);
            store.TimelineEvents.Add(assignedEvent);
            store.TimelineEvents.Add(inProgressEvent);
        }
    }
}
