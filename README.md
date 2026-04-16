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
- [Initial Setup](#initial-setup)
- [User Management](#user-management)
- [Enrolment](#enrolment)
- [Payment System](#payment-system)
- [Attendance System](#attendance-system)
- [Course Structure](#course-structure)
- [Calendar & Scheduling System](#calendar--scheduling-system)
- [Class Session & Collaboration](#class-session--collaboration)
- [Curriculum](#curriculum)
- [Grading System](#grading-system)
- [Reporting](#reporting)
- [Archiving](#archiving)
- [Authentication & Authorization](#authentication--authorization)
- [Application Services](#application-services)
- [Deployment](#deployment)
- [CI/CD](#cicd)
- [Logging & Monitoring](#logging--monitoring)
- [Roadmap](#roadmap)
- [Acknowledgement](#acknowledgement)

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

View the full design document: [System Design & Planning](https://docs.google.com/document/d/135yFhGdzWKA6NaqIRQNLMLLHV-wnmvP2DCmF0vR9j3U/edit?pli=1&tab=t.0)

---

## Initial Setup

On first access to the application at [fcmsportal.com](https://fcmsportal.com), an initialization wizard guides the setup process:

- Create the school profile, including basic details and logo
- Create the Principal entity and corresponding account
- At this stage, only the Principal account can be created, with the highest level of authorization
- Complete the setup wizard to initialize the application
- Configure the academic period

> ⚠️ The academic period configuration is critical, as it determines the structure and behavior of all subsequent academic activities.

---

## User Management

### Roles
- Principal
- Admin
- Teacher
- Staff
- Guardian
- Student

### Rules
- Only Principal and Admin can create users
- Each role has defined authorization levels
- Students must be linked to a Guardian

---

## Enrolment

The enrolment process defines how students are assigned to structured academic units known as Learning Paths.

- Learning Paths must first be created. Each Learning Path represents a unique combination of:
  - Education Level
  - Class Level
  - Academic Year
  - Term
  - Associated school fees

- This combination is strictly unique and cannot be duplicated within the system

- When a student is enrolled in a Learning Path, the student:
  - is assigned the corresponding academic structure
  - becomes liable for the associated school fees
  - gains access to all academic resources within that Learning Path

---

## Payment System

The payment system manages all student-related financial transactions and integrates directly with academic access and reporting.

### Key Features

- Payments are managed within each student’s profile
- All transactions are tracked in real-time and reflected in the school-wide payment dashboard
- Supports multiple payment methods:
  - Online payments (via external APIs for debit/credit cards)
  - Bank transfers
  - Cash payments
- Automated payment reminders are sent via email, mobile app, and SMS
- Invoices and receipts are generated and delivered to parents/guardians and students
- Real-time payment reports are available, including:
  - Individual student transactions
  - Learning Path financial summaries
  - School-wide KPIs and financial insights

---

## Attendance System


---

## Course Structure


---

## Calendar & Scheduling System


---

## Class Session & Collaboration


---

## Curriculum


---

## Grading System


---

## Reporting


---

## Archiving


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

## Application Services


---

## Deployment

The application is deployed to a **Virtual Private Server (VPS)** with:

- MySQL database
- SSL encryption (HTTPS)
- Reverse proxy configuration
- Docker support (for database and future scaling)

---

## CI/CD

---

## Logging & Monitoring

---

## Roadmap

---

## Acknowledgement

---
