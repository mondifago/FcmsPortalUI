# Future Champions Model School Portal (fcmsportal)

Fcmsportal is a comprehensive school management web application designed to handle the full educational lifecycle of students, from kindergarten through senior secondary levels. It was developed to meet the specific operational needs of Future Champions Model School, an institution within the Nigerian educational system.

The system is composed of modular components including **class session management**, **scheduling**, **attendance tracking**, **grading**, **payment processing**, **dynamic curriculum generation**, **reporting**, **adaptive academic period management**, and a **structured semester transition process**.

Built on the .NET framework, the portal distinguishes users through a robust role-based access control system, and a secured account provisioning system via email invitation which enhances identity verification. 

The application is designed with a strong emphasis on reliability, scalability, and maintainability, following clean architecture principles and clear separation of concerns. It is already deployed to production and secured with SSL and a reverse proxy.


---

## Table of Contents

- [Goal](#goal)
- [Core Features](#core-features)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Data Model Overview](#data-model-overview)
- [Design & Planning](#design--planning)
- [Authentication & Authorization](#authentication--authorization)
- [Payment System](#payment-system)
- [Curriculum System](#curriculum-system)
- [Reporting](#reporting)
- [Deployment](#deployment)
- [Security](#security)
- [Project Structure](#project-structure)
- [Setup & Installation](#setup--installation)
- [Roadmap](#roadmap)
- [Contributing](#contributing)

---

## Goal

To build a unified school management system where each operational component functions as a modular, scalable and maintainable unit, yet works together seamlessly as a cohesive, intelligent system with shared contextual awareness.

---

## Core Features

- Student, Staff, and Guardian management
- Class session and scheduling system
- Attendance tracking
- Grading and report card generation
- Payment and financial tracking system
- Dynamic curriculum generation (no static curriculum storage)
- Interactive class session platform that enhances collaboration between students, guardians and teachers
- Email-based account invitation system
- Role-based access control

---

## Tech Stack

- **Frontend:** Blazor Server
- **Backend:** .NET (ASP.NET Core)
- **Database:** MySQL
- **ORM:** Entity Framework Core
- **Authentication:** ASP.NET Identity
- **Email:** SMTP-based service
- **logging:** Serilog
- **CI/CD:** Docker
- **Hosting:** VPS with reverse proxy

---

## Architecture

Fcmsportal is built using a **Blazor Server** with a strong emphasis on **separation of concerns**.

- Domain: 
   - `Class Model`  ([FcmsPortal](https://github.com/mondifago/FcmsPortal))
- Business Logic:
  - `LogicMethods`
- Application:
  - `ISchoolDataService`
  - `IPermissionService`
  - `IEmailService`
  - `IAccountInvitationService`
  - `IEmailNotificationService`
  - `ExceptionHandlerService`
  - `ValidationService`
- Data Infrastructure:
  - `FcmsPortalUIContext`
- UI:
  - Blazor Server

---

## Data Model Overview

The system revolves around core entities such as:

- `School`
- `AcademicPeriod`
- `Person`
- `Student`
- `Staff`
- `Guardian`
- `ClassSession`
- `ScheduleEntry`
- `LearningPath`
- `CourseDefaults`
- `Curriculum`
- `Attendance`
- `Payment`
- `Grades`
- `AccountInvitation`
- `Archives`

Relationships are designed to reflect future champion school structure, with dynamic, responsive and cognitive associations rather than rigid schemas.

---

## Design & Planning

Before implementation, the system was thoroughly designed to define:

- Core domain models
- Relationships between entities
- Functional modules and responsibilities
- Testing of system-wide interactions and workflows

This planning phase ensured a clean, scalable architecture and reduced technical debt during development.

📄 View the full design document: [System Design & Planning](https://docs.google.com/document/d/135yFhGdzWKA6NaqIRQNLMLLHV-wnmvP2DCmF0vR9j3U/edit?pli=1&tab=t.0)

---

## Authentication & Authorization

- Built using **.NET Identity**
- Role-based access control (RBAC)
- Supported roles include:
  - Principal
  - Administrator
  - Teacher
  - Staff
  - Student
  - Guardian

### Account Provisioning

- Accounts are created via **email invitation**
- Enforces:
  - Verified email ownership
  - Controlled onboarding
  - Reduced risk of unauthorized access

---

## Payment System

The payment system is tightly integrated with academic access control.

### Features

- Assign fees to students
- Track payments
- Determine access eligibility based on payment thresholds
- Generate payment reports

### Design Principle

All payment logic is handled within the service layer — never in the UI.

---

## Curriculum System

Unlike traditional systems, fcmsportal:

- **Does NOT store curriculum in the database**
- Generates curriculum dynamically from:
  - Lesson plans
  - Ongoing class sessions

This ensures:
- Real-time accuracy
- No redundancy
- Flexible academic structure

---

## Reporting

- Student academic reports
- Payment reports
- Attendance summaries
- Ranking and performance insights

Reports are generated dynamically based on current system state.

---

## Deployment

The application is deployed to a **Virtual Private Server (VPS)** with:

- MySQL database
- SSL encryption (HTTPS)
- Reverse proxy configuration
- Docker support (for database and future scaling)

---

## Security

- ASP.NET Identity for authentication
- Role-based authorization
- Email verification system
- Anti-forgery protection
- Secure password reset and confirmation flows

---

## Project Structure
