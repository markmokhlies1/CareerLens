#  CareerLens

> A modern Glassdoor-inspired platform built with **.NET 8**, **Clean Architecture**, and **Domain-Driven Design** principles.
> CareerLens empowers employees to share honest company reviews, interview experiences, and salary insights — while giving employers tools to manage and moderate their company presence.

##  Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Features](#features)
- [Project Structure](#project-structure)
- [Domain Model](#domain-model)
- [API Endpoints](#api-endpoints)
- [Design Patterns](#design-patterns)
- [Getting Started](#getting-started)
- [Environment Variables](#environment-variables)

---

##  Overview

CareerLens is a full-featured career platform REST API that allows:

-  **Employees** to submit reviews, interview experiences, and salary reports for companies
-  **Employers** to manage job listings and moderate employee submissions
-  **Real-time notifications** via SignalR when submissions are approved or rejected
-  **Role-based access control** with JWT authentication

---

##  Architecture

CareerLens follows **Clean Architecture** with a strict separation of concerns across 4 layers:
CareerLens/
├── src/
│   ├── CareerLens.Domain          # Entities, Value Objects, Domain Events, Errors
│   ├── CareerLens.Application     # CQRS Handlers, Validators, DTOs, Interfaces
│   ├── CareerLens.Infrastructure  # EF Core, Repositories, SignalR, JWT, Services
│   └── CareerLens.API             # Controllers, Request Models, API Validators
└── tests/
    ├── CareerLens.Domain.Tests

### Dependency Flow
API → Application → Domain ← Infrastructure

> The Domain layer has **zero dependencies**. Infrastructure and API depend inward, never outward.

---

## 🛠️ Tech Stack

| Layer          | Technology                          |
|----------------|-------------------------------------|
| Framework      | .NET 8 / ASP.NET Core               |
| ORM            | Entity Framework Core 8             |
| Database       | PostgreSQL                          |
| CQRS           | MediatR                             |
| Validation     | FluentValidation                    |
| Auth           | JWT Bearer Tokens                   |
| Real-Time      | SignalR                             |
| Error Handling | ErrorOr                             |
| Mapping        | Manual Extension Methods            |
| API Docs       | Swagger / Scalar                    |

---

## ✨ Features

### 👨‍💼 Employee Features
-  Submit, update, and delete **company reviews**
-  Submit, update, and delete **interview experiences**
-  Submit, update, and delete **salary reports**
-  Browse **published job listings**
-  Receive **real-time notifications** for approvals/rejections
-  Manage personal **notification center** (read, mark all read, delete)

### 🏢 Employer Features
-  Post, update, and manage **job listings** (Draft → Published → Closed)
-  Moderate **reviews**, **interviews**, and **salary reports** (Approve / Reject)
-  Role-based company access (**HR** for jobs, **Moderator** for content moderation)

###  Notification System
-  Real-time push notifications via **SignalR**
-  Persistent notifications stored in the database
-  Triggered by domain events on review/interview/salary approval and rejection

---

## 📁 Project Structure

---

##  Domain Model

### Status State Machines

**Review / Interview**
Pending ──► Approved
Pending ──► Rejected

**Job**
Draft ──► Published ──► Closed
Published ──► Draft


**Salary**
Pending ──► Approved
Pending ──► Rejected

### Value Objects
| Value Object       | Fields                          | Validation                          |
|--------------------|---------------------------------|-------------------------------------|
| `Money`            | `Amount`, `Currency`            | Amount ≥ 0, Currency not null       |
| `Currency`         | `Code`                          | ISO 4217 format (e.g. USD, EUR)     |
| `InterviewDate`    | `Year`, `Month`                 | Year 2000–current, Month 1–12       |
| `InterviewDuration`| `Value`, `Unit`                 | Value > 0, valid unit enum          |

---


---

##  Design Patterns

###  CQRS with MediatR
Every use case is a self-contained `Command` or `Query` handler with its own validator, keeping logic isolated and testable.

###  ErrorOr Result Pattern
All operations return `Result<T>` using the **ErrorOr** library. No exceptions for business logic — errors flow naturally through the pipeline.

###  Domain-Driven Design
- **Rich domain entities** with private setters and factory methods (`Create()`, `Update()`)
- **Value objects** (`Money`, `Currency`, `InterviewDate`) with built-in validation
- **Domain events** (`ReviewApproved`, `SalaryRejected`) decoupled via MediatR `INotification`
- **State machines** enforced at the domain level for status transitions

###  Pipeline Behaviors


###  Two-Layer Validation
| Layer         | Tool              | Purpose                                    |
|---------------|-------------------|--------------------------------------------|
| API Layer     | FluentValidation  | Shape, format, range, required fields      |
| Domain Layer  | Guard clauses     | Business rules, state transitions, logic   |

###  Clean Route Design
Every resource follows a consistent `/manage` pattern:

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/)
- [Docker](https://www.docker.com/) (optional)

### Run Locally

```bash
# 1. Clone the repository
git clone https://github.com/markmokhlies1/CareerLens.git
cd CareerLens


# 3. Apply migrations
dotnet ef database update --project src/CareerLens.Infrastructure --startup-project src/CareerLens.API

# 4. Run the API
dotnet run --project src/CareerLens.API


