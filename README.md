Future Champions Model School Portal
Fcmsportal is a web application that handles the complete educational lifecycle — from student enrolment and class sessions through grading, attendance tracking, calendar management, payments, and semester transitions.
Built within the .NET framework, the portal serves administrators, teachers, students, and guardians with role-based access control authorization and account setup through email invitation. It is hand crafted to be reliable, scalable, maintainable with clear separation of concern, abiding by software development best principles.
fcmsportal is already deployed to production on a vps, equipped with SSL and reverse proxy. 

Tech Stack
Frontend: Blazor Server (.NET 8), Bootstrap 5, Font Awesome
Backend: ASP.NET Core, Entity Framework Core
Database: MySQL
Authentication: ASP.NET Core Identity with role-based authorization
Email: SMTP integration (Papercut for development)

Core Features
Student Enrollment & Learning Paths
Students are enrolled into learning paths — semester-bound containers that group students by education level and class. Course access is granted upon confirmation of at least 50% fee payment. Each learning path carries its own schedule, attendance log, grading configuration, and curriculum.
Class Sessions & Collaboration
Teachers create class sessions with lesson plans, study materials (file uploads), and YouTube video lessons. Students and teachers interact through threaded discussions directly on each session. Teachers submit remarks per session, and homework can be assigned, submitted, and graded within the portal.
Grading System
Teachers record homework, quiz, and exam grades throughout the semester. Each course has configurable weight percentages (defaulting to 20% homework, 20% quiz, 60% exam). The system automatically computes weighted course totals, semester overall grades, and class rankings. Detailed grade reports show per-course breakdowns with individual test scores, teacher names, and remarks.
Grade Finalization & Semester Transitions
At semester end, teachers review report cards and submit the learning path for principal review. The principal can restore for corrections or finalize all grades, which triggers a chain of events: grade recalculation, data archiving (grades, attendance, payments, report cards), and report card delivery. Third semester finalization includes promotion/graduation workflows with cumulative grade calculations across all three terms.
Attendance Tracking
Daily attendance is recorded per learning path with present/absent tracking for each student. Semester attendance reports show per-student rates with date-by-date breakdowns, daily totals, and color-coded rate indicators.
Payments
The portal tracks school fees per student with support for multiple payment methods (cash, bank transfer, online). Payment completion rates, outstanding balances, and timely payment metrics are calculated at student, learning path, and school-wide levels. Automated email reminders notify guardians of upcoming or overdue payments.
School Calendar & Scheduling
A unified calendar consolidates class sessions, events, and meetings from all learning paths. Scheduling supports one-time and recurring entries with conflict detection. Calendar views include monthly, weekly, and agenda formats.
Curriculum Management
Lesson notes are automatically compiled from class sessions across all active learning paths, filterable by education level, class, and semester — giving administrators a real-time view of what's being taught.
Data Archiving
Finalized semester data is archived independently: student grades with full test-level detail, attendance records, payment histories, and report cards. Archives are browsable by academic year, semester, education level, and class, preserving a complete historical record even after students progress or graduate.
Role-Based Dashboards
Each role sees a tailored home page:

Principal/Admin: Submitted learning paths for review, reports, announcements, school-wide metrics
Teacher: Today's class schedule, homework submissions, session remarks
Student: Class schedule, homework due, recently graded work, report cards
Guardian: Ward's schedule, homework, grades, and report cards

Notifications & Communication
Email notifications support account invitations, payment reminders, and event alerts. Announcements and motivational quotes are managed by administrators and displayed on dashboards.
