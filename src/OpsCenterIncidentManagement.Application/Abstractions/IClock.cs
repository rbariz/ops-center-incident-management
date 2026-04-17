namespace OpsCenterIncidentManagement.Application.Abstractions;

public interface IClock
{
    DateTime UtcNow { get; }
}
