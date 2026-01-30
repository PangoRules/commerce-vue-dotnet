# Architecture Decision Records (ADR)

## Overview

This directory contains Architecture Decision Records (ADRs) documenting significant architectural decisions made for the Commerce Vue.NET project.

## ADR Index

| ID | Title | Status | Date |
|----|-------|--------|------|
| [001](001-layered-architecture.md) | Layered Architecture | Accepted | 2024-01 |

## Creating New ADRs

1. Copy `template.md` to `NNN-title.md` (next sequential number)
2. Fill in the template sections
3. Update this index
4. Commit with the related implementation

## ADR Lifecycle

| Status | Meaning |
|--------|---------|
| Proposed | Under discussion |
| Accepted | Decision made, implementation in progress or complete |
| Deprecated | Superseded by another decision |
| Rejected | Considered but not adopted |

## Conventions

- **Numbering**: Three-digit sequential (001, 002, ...)
- **Title**: Short, descriptive, lowercase with hyphens
- **Date**: When the decision was made
- **Context**: Problem that prompted the decision
- **Decision**: What we decided
- **Consequences**: What results from this decision
