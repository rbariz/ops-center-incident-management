# 1. Ops Center — Incident Management Architecture

This document describes the architecture and design decisions behind the Ops Center Incident Management system.

---

# 2. Overview

Ops Center is a system designed to manage operational incidents in real time.

It enables:

- incident tracking
- ownership assignment
- SLA-based escalation
- resolution workflows
- full timeline traceability

The system focuses on **operational reliability and workflow orchestration**, not just CRUD.

---

# 3. High-Level Architecture

The system follows a **Clean Architecture approach**:

Client (Web Dashboard)  
↓  
ASP.NET Core API  
↓  
Application Layer (Use Cases)  
↓  
Domain Layer (Entities + Rules)  
↓  
Infrastructure (In-Memory / future database)

---

# 4. Domain Model

## 4.1 Core Entities

### Incident
Represents an operational issue.

Key properties:
- Title
- Description
- Location
- Priority
- Status
- Timestamps (Created, Assigned, Resolved, Closed)

---

### OperatorAgent
Represents an operator or field agent.

- Name
- Team
- Availability status

---

### IncidentAssignment
Represents ownership of an incident.

- IncidentId
- AgentId
- AssignedAtUtc

---

### IncidentTimelineEvent
Represents any event in the lifecycle.

Examples:
- created
- assigned
- status changed
- escalated
- resolved

---

### SlaPolicy
Defines escalation rules.

- Priority
- Escalation delay (minutes)

---

# 5. Incident Lifecycle

The system enforces a strict workflow:

Created → Assigned → InProgress → Escalated → Resolved → Closed

Rules:
- Only Assigned incidents can start work
- Only active incidents can be resolved
- Closed incidents are immutable

---

# 6. Core Use Cases

## 6.1 EvaluateIncidentEscalation

This is the core intelligence of the system.

Steps:
1. load incident
2. load SLA policy based on priority
3. compute elapsed time
4. compare with SLA threshold
5. escalate if needed

Escalation triggers:
- status change to Escalated
- timeline event creation

---

## 6.2 Assign / Start / Resolve

These actions drive the workflow:

- Assign → incident becomes Assigned
- Start → incident becomes InProgress
- Resolve → incident becomes Resolved

Each action:
- updates domain state
- creates a timeline event

---

## 6.3 Incident Timeline

The system keeps a full history of:

- lifecycle transitions
- escalations
- operational actions

This provides:
- auditability
- debugging capability
- operational insight

---

## 6.4 Incident Summary

A synthetic endpoint provides:

- identity
- priority / status
- age (minutes)
- SLA target
- escalation deadline
- overdue flag
- timeline count

This is used by dashboards and operators.

---

# 7. SLA & Escalation Logic

Each incident has a policy based on priority:

Example:

| Priority | Escalation Delay |
|----------|----------------|
| Low      | 240 min        |
| Medium   | 120 min        |
| High     | 30 min         |
| Critical | 10 min         |

Escalation occurs when:

current time ≥ created time + SLA delay

Constraints:
- no escalation if already escalated
- no escalation if resolved/closed/cancelled

---

# 8. Idempotency (Critical Design)

Escalation logic is idempotent:

- calling evaluate multiple times does not duplicate effects
- escalation happens only once

This ensures:
- system stability
- no duplicate timeline events
- predictable behavior

---

# 9. Infrastructure (Current)

The system currently uses:

- In-Memory repositories
- InMemoryStore (shared state)
- UnitOfWork abstraction
- SystemClock

This allows:

- fast prototyping
- deterministic testing
- demo without external dependencies

---

# 10. API Design

Main endpoints:

GET  /api/incidents  
GET  /api/incidents/{id}/summary  
GET  /api/incidents/{id}/timeline  

POST /api/incidents/{id}/assign  
POST /api/incidents/{id}/start  
POST /api/incidents/{id}/resolve  
POST /api/incidents/{id}/evaluate-escalation  

GET  /api/ops-dashboard  

---

# 11. Web Dashboard

A minimal web UI provides:

- incident list
- status visualization
- escalation trigger
- workflow actions (assign/start/resolve)
- timeline access
- summary access

The dashboard demonstrates:

- real workflow
- state transitions
- operational control

---

# 12. Key Design Decisions

## 12.1 Domain-first design

Business rules are implemented inside:

- entities
- use cases

NOT in controllers.

---

## 12.2 Explicit workflow

Incident lifecycle is enforced via domain methods.

No implicit state changes.

---

## 12.3 Timeline as first-class concept

All actions generate events.

This enables:
- traceability
- audit
- debugging

---

## 12.4 SLA-driven system

Escalation is not manual — it is rule-based.

This reflects real production systems.

---

## 13. Future Improvements

Planned extensions:

- PostgreSQL persistence
- background jobs (Hangfire)
- real-time updates (SignalR)
- automatic assignment (routing engine)
- SLA breach alerts
- multi-team operations
- analytics dashboard

---

# 14. Conclusion

This project demonstrates:

- workflow-driven architecture
- SLA-based automation
- domain modeling for operations
- real-world system behavior

It reflects how incident management systems are designed in production environments.