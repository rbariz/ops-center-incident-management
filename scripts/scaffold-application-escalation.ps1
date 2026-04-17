param(
    [string]$Root = "D:\SAASs\ops-center-incident-management\src\OpsCenterIncidentManagement.Application"
)

function Write-CodeFile {
    param(
        [string]$Path,
        [string]$Content
    )

    $directory = Split-Path $Path -Parent
    New-Item -ItemType Directory -Path $directory -Force | Out-Null
    Set-Content -Path $Path -Value $Content -Encoding UTF8
}

$AbstractionsPath = Join-Path $Root "Abstractions"
$UseCasePath = Join-Path $Root "UseCases\EvaluateIncidentEscalation"
$DependencyInjectionPath = Join-Path $Root "DependencyInjection"

Write-CodeFile -Path (Join-Path $AbstractionsPath "IIncidentRepository.cs") -Content @'
using OpsCenterIncidentManagement.Domain.Entities;

namespace OpsCenterIncidentManagement.Application.Abstractions;

public interface IIncidentRepository
{
    Task<Incident?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
'@

Write-CodeFile -Path (Join-Path $AbstractionsPath "ISlaPolicyRepository.cs") -Content @'
using OpsCenterIncidentManagement.Domain.Entities;
using OpsCenterIncidentManagement.Domain.Enums;

namespace OpsCenterIncidentManagement.Application.Abstractions;

public interface ISlaPolicyRepository
{
    Task<SlaPolicy?> GetByPriorityAsync(IncidentPriority priority, CancellationToken cancellationToken = default);
}
'@

Write-CodeFile -Path (Join-Path $AbstractionsPath "IIncidentTimelineEventRepository.cs") -Content @'
using OpsCenterIncidentManagement.Domain.Entities;

namespace OpsCenterIncidentManagement.Application.Abstractions;

public interface IIncidentTimelineEventRepository
{
    Task AddAsync(IncidentTimelineEvent timelineEvent, CancellationToken cancellationToken = default);
}
'@

Write-CodeFile -Path (Join-Path $AbstractionsPath "IUnitOfWork.cs") -Content @'
namespace OpsCenterIncidentManagement.Application.Abstractions;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
'@

Write-CodeFile -Path (Join-Path $AbstractionsPath "IClock.cs") -Content @'
namespace OpsCenterIncidentManagement.Application.Abstractions;

public interface IClock
{
    DateTime UtcNow { get; }
}
'@

Write-CodeFile -Path (Join-Path $UseCasePath "EvaluateIncidentEscalationRequest.cs") -Content @'
namespace OpsCenterIncidentManagement.Application.UseCases.EvaluateIncidentEscalation;

public sealed record EvaluateIncidentEscalationRequest(
    Guid IncidentId
);
'@

Write-CodeFile -Path (Join-Path $UseCasePath "EvaluateIncidentEscalationResult.cs") -Content @'
namespace OpsCenterIncidentManagement.Application.UseCases.EvaluateIncidentEscalation;

public sealed record EvaluateIncidentEscalationResult(
    Guid IncidentId,
    bool IsEscalated,
    string Reason
);
'@

Write-CodeFile -Path (Join-Path $UseCasePath "EvaluateIncidentEscalationHandler.cs") -Content @'
using OpsCenterIncidentManagement.Application.Abstractions;
using OpsCenterIncidentManagement.Domain.Entities;
using OpsCenterIncidentManagement.Domain.Enums;
using OpsCenterIncidentManagement.Domain.Exceptions;

namespace OpsCenterIncidentManagement.Application.UseCases.EvaluateIncidentEscalation;

public sealed class EvaluateIncidentEscalationHandler
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly ISlaPolicyRepository _slaPolicyRepository;
    private readonly IIncidentTimelineEventRepository _timelineRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public EvaluateIncidentEscalationHandler(
        IIncidentRepository incidentRepository,
        ISlaPolicyRepository slaPolicyRepository,
        IIncidentTimelineEventRepository timelineRepository,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _incidentRepository = incidentRepository;
        _slaPolicyRepository = slaPolicyRepository;
        _timelineRepository = timelineRepository;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<EvaluateIncidentEscalationResult> HandleAsync(
        EvaluateIncidentEscalationRequest request,
        CancellationToken cancellationToken = default)
    {
        var incident = await _incidentRepository.GetByIdAsync(request.IncidentId, cancellationToken);
        if (incident is null)
            throw new DomainException("Incident not found.");

        if (incident.Status is IncidentStatus.Resolved or IncidentStatus.Closed or IncidentStatus.Cancelled)
        {
            return new EvaluateIncidentEscalationResult(
                incident.Id,
                false,
                "Incident is already finalized.");
        }

        if (incident.Status == IncidentStatus.Escalated)
        {
            return new EvaluateIncidentEscalationResult(
                incident.Id,
                false,
                "Incident is already escalated.");
        }

        var policy = await _slaPolicyRepository.GetByPriorityAsync(incident.Priority, cancellationToken);
        if (policy is null)
            throw new DomainException("SLA policy not found for incident priority.");

        var nowUtc = _clock.UtcNow;
        var escalationDueAt = incident.CreatedAtUtc.AddMinutes(policy.EscalationAfterMinutes);

        if (nowUtc < escalationDueAt)
        {
            return new EvaluateIncidentEscalationResult(
                incident.Id,
                false,
                "SLA threshold not reached yet.");
        }

        incident.Escalate();

        var timelineEvent = new IncidentTimelineEvent(
            Guid.NewGuid(),
            incident.Id,
            TimelineEventType.Escalated,
            $"Incident escalated automatically after SLA threshold ({policy.EscalationAfterMinutes} min).",
            nowUtc);

        await _timelineRepository.AddAsync(timelineEvent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new EvaluateIncidentEscalationResult(
            incident.Id,
            true,
            "Incident escalated.");
    }
}
'@

Write-CodeFile -Path (Join-Path $DependencyInjectionPath "ServiceCollectionExtensions.cs") -Content @'
using Microsoft.Extensions.DependencyInjection;
using OpsCenterIncidentManagement.Application.UseCases.EvaluateIncidentEscalation;

namespace OpsCenterIncidentManagement.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<EvaluateIncidentEscalationHandler>();
        return services;
    }
}
'@

Write-Host "OpsCenter Application escalation scaffold generated successfully." -ForegroundColor Green