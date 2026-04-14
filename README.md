# Future Champions Model School Portal (fcmsportal)

Fcmsportal is a comprehensive school management web application designed to handle the full educational lifecycle of students, from kindergarten through senior secondary levels. It was developed to meet the specific operational needs of Future Champions Model School, an institution within the Nigerian educational system.

The system is composed of modular components including **class session management**, **scheduling**, **attendance tracking**, **grading**, **payment processing**, **dynamic curriculum generation**, **reporting**, **adaptive academic period management**, and a **structured semester transition process**.

Built on the .NET framework, the portal serves principals, administrators, teachers, staff, students, and guardians through a robust role-based access control system powered by .NET Identity. Secure account provisioning via email invitation enhances identity verification, helping to protect against unauthorized access and ensuring data integrity. 

The application is designed with a strong emphasis on reliability, scalability, and maintainability, following clean architecture principles and clear separation of concerns. It is backed by a MySQL database using Entity Framework Core and is deployed to production, secured with SSL and a reverse proxy.


---

## Table of Contents

- [Goals](#goals)
- [Core Features](#core-features)
- [Architecture](#architecture)
- [Data Model Overview](#data-model-overview)
- [Modules](#modules)
- [Authentication & Authorization](#authentication--authorization)
- [Payment System](#payment-system)
- [Curriculum System](#curriculum-system)
- [Reporting](#reporting)
- [Deployment](#deployment)
- [Security](#security)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Setup & Installation](#setup--installation)
- [Roadmap](#roadmap)
- [Contributing](#contributing)

---

## Goals

- Provide a centralized system for managing school operations
- Digitize academic workflows from enrollment to graduation
- Ensure secure and role-based access to data
- Support real-time academic tracking and reporting
- Maintain clean architecture and scalability for future expansion

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

## Architecture

Fcmsportal is built using a **Blazor Server architecture** with a strong emphasis on **separation of concerns**.

### Key Principles

- Business logic is centralized in services (`ISchoolDataService`)
- UI logic is kept minimal and contained within Razor components
- No direct data manipulation in UI components
- Clean layering between:
  - UI (Blazor)
  - Services (Business Logic)
  - Data Access (EF Core)

### Backend

- Entity Framework Core with MySQL
- Scoped services for domain logic
- Identity system integrated into the same DbContext

---

## Data Model Overview

The system revolves around core entities such as:

- `Student`
- `Staff`
- `Guardian`
- `ClassSession`
- `LearningPath`
- `Attendance`
- `Payment`
- `Grades`

Relationships are designed to reflect real-world school structures, with dynamic associations rather than rigid schemas where necessary.

---

## Modules

### Academic Management
- Class sessions
- Lesson planning
- Homework and submissions
- Semester transitions

### User Management
- Students
- Staff
- Guardians

### Communication
- Discussion threads
- Notifications (email-based)

### Finance
- Payment tracking
- Fee assignment
- Access control based on payment status

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

## Tech Stack

- **Frontend:** Blazor Server
- **Backend:** .NET (ASP.NET Core)
- **Database:** MySQL
- **ORM:** Entity Framework Core
- **Authentication:** ASP.NET Identity
- **Email:** SMTP-based service
- **Hosting:** VPS with reverse proxy

---

## Project Structure
