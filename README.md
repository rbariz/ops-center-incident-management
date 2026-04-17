# 1. Ops Center — Incident Management

Operational incident management platform designed to track, assign, escalate, and resolve incidents in real time.

---

## 2. Overview

This project aims to design an operations control system capable of handling:

- incident creation
- assignment to operators or field agents
- escalation when SLA is at risk
- incident timeline and resolution workflow

The focus is on operational reliability, traceability, and workflow orchestration.

---

## 3. Core Problem

Operational teams face multiple challenges:

- incidents must be tracked consistently
- ownership must be clear
- escalation must happen before SLA breaches
- timelines must remain auditable

This project addresses these issues through domain modeling and workflow-driven system design.

---

## 4. Key Features

- Incident lifecycle management
- Assignment workflow
- SLA / escalation evaluation
- Incident timeline
- Operator dashboard

---

## 5. Architecture (Planned)

- Backend: ASP.NET Core Web API
- Admin UI: Blazor Server or minimal web dashboard
- Database: PostgreSQL (future)
- Real-time: SignalR (future)

---

## 6. Status

Design phase.

Current focus:
- domain model
- incident lifecycle
- escalation logic
- operational visibility

---

## 7. Author

Rachid Bariz  
Senior Full-Stack .NET Architect

---

# 8. Présentation (FR)

Plateforme de gestion d’incidents opérationnels.

Objectifs :
- suivre les incidents
- assigner les responsables
- escalader selon SLA
- garder une traçabilité complète

Ce projet se concentre sur :
- workflow métier
- orchestration
- supervision opérationnelle