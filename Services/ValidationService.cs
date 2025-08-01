﻿using FcmsPortal.Enums;
using FcmsPortal.Models;
using FcmsPortal.Services;
using Microsoft.AspNetCore.Components.Forms;

namespace FcmsPortalUI.Services
{
    public class ValidationService
    {
        public bool ValidateGuardian(EditContext context, Guardian guardian, ValidationMessageStore messageStore)
        {
            messageStore.Clear();
            bool isValid = context.Validate();
            bool personValid = ValidatePerson(context, guardian.Person, messageStore);

            if (guardian.RelationshipToStudent == default)
            {
                var field = new FieldIdentifier(guardian, nameof(guardian.RelationshipToStudent));
                messageStore.Add(field, "Relationship to student is required.");
                isValid = false;
            }

            context.NotifyValidationStateChanged();
            return isValid && personValid;
        }

        public bool ValidateStudent(EditContext context, Student student, ValidationMessageStore messageStore)
        {
            messageStore.Clear();
            bool isValid = context.Validate();
            bool personValid = ValidatePerson(context, student.Person, messageStore);

            if (student.Person.EducationLevel == default)
            {
                var field = new FieldIdentifier(student.Person, nameof(student.Person.EducationLevel));
                messageStore.Add(field, "Education level is required.");
                isValid = false;
            }

            if (student.Person.ClassLevel == default)
            {
                var field = new FieldIdentifier(student.Person, nameof(student.Person.ClassLevel));
                messageStore.Add(field, "Class level is required.");
                isValid = false;
            }

            if (student.Person.DateOfBirth == default || student.Person.DateOfBirth == DateTime.Today)
            {
                var field = new FieldIdentifier(student.Person, nameof(student.Person.DateOfBirth));
                messageStore.Add(field, "Date of birth is required.");
                isValid = false;
            }

            context.NotifyValidationStateChanged();
            return isValid && personValid;
        }

        public bool ValidateStaff(EditContext context, Staff staff, ValidationMessageStore messageStore)
        {
            messageStore.Clear();
            bool isValid = context.Validate();
            bool personValid = ValidatePerson(context, staff.Person, messageStore);

            if (staff.Person.DateOfBirth == default || staff.Person.DateOfBirth == DateTime.Today)
            {
                var field = new FieldIdentifier(staff.Person, nameof(staff.Person.DateOfBirth));
                messageStore.Add(field, "Date of birth is required.");
                isValid = false;
            }

            if (staff.JobRole == JobRole.None)
            {
                var field = new FieldIdentifier(staff, nameof(staff.JobRole));
                messageStore.Add(field, "Job role is required.");
                isValid = false;
            }

            if (staff.Person.EducationLevel == default)
            {
                var field = new FieldIdentifier(staff.Person, nameof(staff.Person.EducationLevel));
                messageStore.Add(field, "Education level is required.");
                isValid = false;
            }

            context.NotifyValidationStateChanged();
            return isValid && personValid;
        }

        private bool ValidatePerson(EditContext context, Person person, ValidationMessageStore messageStore)
        {
            bool isValid = true;

            void Require(string propertyName, string value, string error)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    var field = new FieldIdentifier(person, propertyName);
                    messageStore.Add(field, error);
                    isValid = false;
                }
            }

            Require(nameof(person.FirstName), person.FirstName, "First name is required.");
            Require(nameof(person.LastName), person.LastName, "Last name is required.");
            Require(nameof(person.Email), person.Email, "Email is required.");
            Require(nameof(person.PhoneNumber), person.PhoneNumber, "Phone number is required.");
            Require(nameof(person.StateOfOrigin), person.StateOfOrigin, "State of origin is required.");
            Require(nameof(person.LgaOfOrigin), person.LgaOfOrigin, "LGA of origin is required.");

            if (person.Sex == Gender.None)
            {
                var field = new FieldIdentifier(person, nameof(person.Sex));
                messageStore.Add(field, "Gender is required.");
                isValid = false;
            }

            if (person.Addresses == null || person.Addresses.Count == 0)
            {
                var field = new FieldIdentifier(person, nameof(person.Addresses));
                messageStore.Add(field, "At least one address is required.");
                isValid = false;
            }

            return isValid;
        }

        public bool ValidateLearningPath(EditContext context, LearningPath learningPath, ValidationMessageStore messageStore, ISchoolDataService schoolDataService)
        {
            messageStore.Clear();
            bool isValid = context.Validate();

            var existingPaths = schoolDataService.GetAllLearningPaths();
            bool isDuplicate = existingPaths.Any(lp =>
                lp.Id != learningPath.Id &&
                lp.EducationLevel == learningPath.EducationLevel &&
                lp.ClassLevel == learningPath.ClassLevel &&
                lp.Semester == learningPath.Semester &&
                lp.AcademicYear == learningPath.AcademicYear);

            if (isDuplicate)
            {
                var field = new FieldIdentifier(learningPath, nameof(learningPath.EducationLevel));
                messageStore.Add(field, "A learning path with this combination already exists.");
                isValid = false;
            }

            if (learningPath.EducationLevel == default)
            {
                var field = new FieldIdentifier(learningPath, nameof(learningPath.EducationLevel));
                messageStore.Add(field, "Education level is required.");
                isValid = false;
            }

            if (learningPath.ClassLevel == default)
            {
                var field = new FieldIdentifier(learningPath, nameof(learningPath.ClassLevel));
                messageStore.Add(field, "Class level is required.");
                isValid = false;
            }

            if (learningPath.FeePerSemester <= 0)
            {
                var field = new FieldIdentifier(learningPath, nameof(learningPath.FeePerSemester));
                messageStore.Add(field, "Fee per semester must be greater than zero.");
                isValid = false;
            }

            context.NotifyValidationStateChanged();
            return isValid;
        }

        public bool ValidateScheduleEntry(EditContext context, ScheduleEntry scheduleEntry, ScheduleType scheduleType, ValidationMessageStore messageStore)
        {
            messageStore.Clear();
            bool isValid = context.Validate();

            if (string.IsNullOrWhiteSpace(scheduleEntry.Title))
            {
                var field = new FieldIdentifier(scheduleEntry, nameof(scheduleEntry.Title));
                messageStore.Add(field, "Title is required.");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(scheduleEntry.Venue))
            {
                var field = new FieldIdentifier(scheduleEntry, nameof(scheduleEntry.Venue));
                isValid = false;
            }

            if (scheduleType == ScheduleType.Event && string.IsNullOrWhiteSpace(scheduleEntry.Event))
            {
                var field = new FieldIdentifier(scheduleEntry, nameof(scheduleEntry.Event));
                messageStore.Add(field, "Event details are required.");
                isValid = false;
            }

            if (scheduleType == ScheduleType.Meeting && string.IsNullOrWhiteSpace(scheduleEntry.Meeting))
            {
                var field = new FieldIdentifier(scheduleEntry, nameof(scheduleEntry.Meeting));
                messageStore.Add(field, "Meeting details are required.");
                isValid = false;
            }

            if (scheduleType == ScheduleType.ClassSession && scheduleEntry.ClassSession == null)
            {
                var field = new FieldIdentifier(scheduleEntry, nameof(scheduleEntry.ClassSession));
                messageStore.Add(field, "A class session must be added for class schedules.");
                isValid = false;
            }

            context.NotifyValidationStateChanged();
            return isValid;
        }
    }
}