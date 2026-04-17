# 1. Ops Center — Architecture

This document describes the architecture and design principles of the Ops Center Incident Management system.

---

## 2. Overview

The system is designed to manage operational incidents through a structured workflow:

- incident creation
- assignment
- work in progress
- escalation
- resolution
- closure

---

## 3. Core Concepts

Main domain objects:

- Incident
- IncidentAssignment
- IncidentTimelineEvent
- OperatorAgent
- SlaPolicy

---

## 4. Workflow

Typical incident lifecycle:

Created → Assigned → InProgress → Escalated → Resolved → Closed

---

## 5. Design Goals

- clear ownership of incidents
- SLA-aware escalation
- full timeline traceability
- production-style workflow orchestration