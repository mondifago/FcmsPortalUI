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
   - Class Models  ([FcmsPortal](https://github.com/mondifago/FcmsPortal))
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

> The academic period configuration is critical, as it determines the structure and behavior of all subsequent academic activities.

---

## User Management

### Roles
- **Principal:** Full administrative control, grade finalization, access to all reports and analytics 
- **Admin:** School operations — enrolment, staff management, scheduling
- **Teacher:** Class sessions management, grading, attendance, learning path management
- **Staff:** General staff with calendar and announcement access
- **Student:** Access class sessions resources, accesses dedicated class schedules, submit homework, participate in discussions
- **Guardian:** Monitor ward's academics activities, accesses ward's attendance, payments and grade reports

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

The attendance system tracks and evaluates student attendance based on the configured academic period (term start and end dates).

### Key Features

- Attendance is recorded once per day for each Learning Path
- Submitted attendance is immediately available in the reporting system
- Access to attendance reports is restricted to Principal and Admin roles
- Supports both:
  - Daily attendance reports
  - Term-based attendance summaries
- At the end of each term, individual student attendance rates are included in their report cards

---

## Course Structure

- Courses are standardized across each Education Level, meaning all class levels within the same education level share an identical course set.
- Each Learning Path automatically inherits its courses based on its assigned education level. 
- At the beginning of each term, the Teacher is required to configure the assessment weight distribution for each course. 
- These weightings determine how assessments contribute to the final grade calculation for that term.
  
---

## Calendar & Scheduling System

The scheduling system provides a centralized and structured approach to managing all school activities across different contexts.

### Key Features

- A centralized calendar aggregates all scheduled items across the system, forming the school-wide calendar
- Schedules can represent one of the following:
  - Events
  - Meetings
  - Class sessions
- Each Learning Path maintains its own dedicated calendar:
  - Only class session schedules are allowed within Learning Path calendars
  - This forms the class timetable for that Learning Path
- All class sessions, along with events and meetings, are synchronized to the centralized calendar

---

## Class Session & Collaboration

A Class Session is the smallest unit of the learning model. It represents the complete set of activities involved in delivering a specific topic within a course.

When creating a Class Session, the following information is defined:

- Course and topic being taught
- Target Learning Path (class)
- Scheduled date, time, and duration
- Venue
- Assigned teacher responsible for the session

### Class Session Components

Each Class Session is supported by a set of integrated resources:

- Basic session information
- List of students expected to attend
- Study materials (including embedded YouTube videos)
- Lesson plan
- Homework and assignments
- Discussion/chat section for interaction
- Teacher’s remarks for post-session reporting and evaluation

---

## Curriculum

The curriculum in fcmsportal is dynamically generated.

It is derived from the aggregation of lesson plans across all Class Sessions. As Class Sessions are created and their lesson plans are completed, the system progressively builds the curriculum in real time.

---

## Grading System

The grading system manages continuous assessment and final evaluation of student performance across all courses within a semester, then 

### Key Features

- Teachers record grades for:
  - Homework (homework grades can both be entered manually or automatically via the homework section of class session platform)
  - Quizzes
  - Examinations
- Each course supports configurable weight distribution
- As grades are entered in each instance, the system automatically computes in real-time:
  - Weighted course totals (based on configured course weight for individual courses)
  - Overall term grades
  - Class rankings
- overall term grade is cumulated from First term through Third term to determine the promotion grade
- Detailed grade reports include:
  - Per-course performance breakdown, with individual assessment scores (every grade score is accessible and trackable)
  - Report cards are automatically compiled and posted on each student's profile during term transition

---

## Reporting

Reports are dynamically generated from live system data, ensuring accuracy, consistency, and up-to-date information at all times.

### Key Features

- Centralized reporting interface for:
  - Attendance reports
  - Class session reports
  - Completed Learning Path summaries
- Real-time financial analytics:
  - Payment activity and trends
  - School-wide financial KPIs for decision-making

---

## Archiving

The archiving system ensures that historical academic and operational data of students are preserved without interfering with active system processes.

The active system processes data of the current academic period in real-time, while the data from past academic periods are preserved and can be easily accessed in Archives

- Data Archived are:
  - Historical Payment records of all students
  - Historical grades records of all studnets
  - Historical attendance records of all students 
  - List of all graduated students along with their graduating reports 

### Design Approach

On transition of concluded learning paths, the system is desinged to automatically filter out archivable data and store them at their designated archive locations

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
- Password recovery and change of login credintials are also done via email
---

## Application Services

The application uses a service-oriented architecture to encapsulate business logic, enforce rules, and maintain separation of concerns between the UI and data layers.

These services are internally implemented within the codebase, with no reliance on third-party service implementation at this stage.

The services are registered with scoped lifetimes and injected via dependency injection, ensuring consistency and testability across the application.

### Core Services

- **ISchoolDataService**  
  Handles core business operations, including CRUD operations and database queries across all domain entities.

- **IPermissionService**  
  Manages role-based authorization, determining access to features and data based on user roles.

- **IEmailService**  
  Provides a custom SMTP-based email client for sending system emails.

- **IAccountInvitationService**  
  Manages user account provisioning through secure email invitations.

- **IEmailNotificationService**  
  Handles school-branded email communications, including:
  - Payment notifications and reminders
  - Account invitations
  - Report card notifications
  - Event announcements
  - School newsletters

### Supporting Services

- **ValidationService**  
  Provides centralized validation logic for form inputs across the application.

- **ExceptionHandlerService**  
  Coordinates application-wide exception handling in conjunction with custom exceptions (e.g., `BusinessRuleException`) and UI error boundaries.

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
